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
	databaseServer  = "localhost"
)

func getDatabase() *sql.DB {
	fmt.Println("SQL Open")
	db, err := sql.Open("postgres", "postgres://"+databaseUser+":"+databasePass+"@"+databaseServer+"/"+database+"?sslmode=disable")

	if err != nil {
		panic(err)
	} else {
		fmt.Println("SQL Open: OK")
	}
	err = db.Ping()
	if err != nil {
		panic(err)
	} else {
		fmt.Println("Database PING: OK")
	}

	_, err = db.Exec(`create table if not exists net_logs
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
	db := getDatabase()
	defer db.Close()

	c, err := kafka.NewConsumer(&kafka.ConfigMap{
		"bootstrap.servers": "localhost:19092", //kartaca-kafka:9092 = localhost:19092
		"group.id":          "go-consumer",
		"auto.offset.reset": "earliest",
	})
	if err != nil {
		panic(err)
	}
	c.SubscribeTopics([]string{"response_log"}, nil)

	var messages []string
	receivedMessageCount := 0
	for {
		kafkaMessage, err := c.ReadMessage(-100)
		if err != nil {
			fmt.Println(err.Error())
		} else {
			receivedMessageCount++
			strKafkaMessage := string(kafkaMessage.Value)
			messages = append(messages, strKafkaMessage)
			fmt.Println(strKafkaMessage)

			if receivedMessageCount >= 10 /*len(messages) >= 10*/ {
				fmt.Println("DB UPDATING")
				for i := 0; i < len(messages); i++ {
					splittedMessage := strings.Split(messages[i], " ")
					db.QueryRow(insertStatement, splittedMessage[0], splittedMessage[1], splittedMessage[2]).Scan()
				}
				messages = nil
				receivedMessageCount = 0
				defer db.Close()
			}
		}
	}
}
