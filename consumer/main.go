package main

import (
	"database/sql"
	"fmt"
	"strings"

	_ "github.com/lib/pq"
	"gopkg.in/confluentinc/confluent-kafka-go.v1/kafka"
)

const (
	insertStatement = `INSERT INTO net_logs (method, elapsed_time, timestamputc) VALUES ($1, $2, $3)`
	database        = "postgres"
	databaseUser    = "postgres"
	databasePass    = "psqlpass"
	databaseServer  = "localhost:15432"
)

func checkDatabase() *sql.DB {
	fmt.Println("SQL Open")
	db, err := sql.Open("postgres", "postgres://"+databaseUser+":"+databasePass+"@"+databaseServer+"/"+database+"?sslmode=disable")
	if err != nil {
		panic(err)
	}

	fmt.Println("SQL Open: OK")

	err = db.Ping()
	if err != nil {
		panic(err)
	}

	fmt.Println("Database PING: OK")

	return db
}

func setDatabase() *sql.DB {
	db := checkDatabase()

	_, err := db.Exec(`create table if not exists net_logs
	(
		method varchar,
		elapsed_time int,
		timestamputc bigint
	);`)

	if err != nil {
		panic(err)
	}

	return db
}

func main() {
	db := setDatabase()
	defer db.Close()

	consumer, err := kafka.NewConsumer(&kafka.ConfigMap{
		"bootstrap.servers": "localhost:19092", //kartaca-kafka:9092 = localhost:19092
		"group.id":          "go-consumer",
		"auto.offset.reset": "earliest",
	})

	if err != nil {
		panic(err)
	}

	consumer.SubscribeTopics([]string{"response_log"}, nil)

	receivedMessageCount := 0
	maxMessageCountToAccumulate := 10
	dbTransaction, _ := db.Begin()

	for {
		kafkaMessage, err := consumer.ReadMessage(-100)
		if err != nil {
			fmt.Println(err.Error())
		} else {
			receivedMessageCount++
			strKafkaMessage := strings.Split(string(kafkaMessage.Value), " ")
			fmt.Println(strKafkaMessage)

			dbTransaction.Exec(insertStatement,
				strKafkaMessage[0], // method
				strKafkaMessage[1], // elapsed time
				strKafkaMessage[2], // utc timestamp
			)

			if receivedMessageCount >= maxMessageCountToAccumulate {
				fmt.Println("DB UPDATING")
				dbTransaction.Commit()
				receivedMessageCount = 0
				defer db.Close()
			}
		}
	}
}
