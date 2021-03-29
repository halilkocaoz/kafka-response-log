package main

import (
	"database/sql"
	"fmt"
	"strings"
	"time"

	_ "github.com/lib/pq"
	"gopkg.in/confluentinc/confluent-kafka-go.v1/kafka"
)

var kafkaTopics = []string{"response_log"}
var kafkaMessages [maxMessageCountToAccumulate]string
var receivedMessageCount = 0

var consumer *kafka.Consumer
var db *sql.DB

const (
	maxMessageCountToAccumulate = 10
	insertStatement             = `INSERT INTO net_logs (method, elapsed_time, timestamputc) VALUES ($1, $2, $3)`
	database                    = "postgres"
	databaseUser                = "postgres"
	databasePass                = "psqlpass"
	databaseServer              = "postgres:5432"
	kafkaServer                 = "kafka:9092"
)

func checkDatabase() {
	fmt.Println("SQL Open")
	var err error
	db, err = sql.Open("postgres", "postgres://"+databaseUser+":"+databasePass+"@"+databaseServer+"/"+database+"?sslmode=disable")
	if err != nil {
		panic(err)
	}

	fmt.Println("SQL Open: OK")

	err = db.Ping()
	if err != nil {
		panic(err)
	}

	fmt.Println("Database PING: OK")
}

func setDatabase() {
	checkDatabase()

	_, err := db.Exec(`create table if not exists net_logs
	(
		method varchar,
		elapsed_time int,
		timestamputc bigint
	);`)

	if err != nil {
		fmt.Println(err.Error())
	}
}

func createConsumer() {
	consumerCreatingRepeatTime := 0

consumerCreateStatement:
	var err error
	consumer, err = kafka.NewConsumer(&kafka.ConfigMap{
		"bootstrap.servers": kafkaServer,
		"group.id":          "go-consumer",
		"auto.offset.reset": "earliest",
	})

	if err != nil {
		consumerCreatingRepeatTime++
		if consumerCreatingRepeatTime <= 5 {
			fmt.Println("Retrying to create consumer")
			time.Sleep(3 * time.Second)
			goto consumerCreateStatement
		} else {
			fmt.Printf("Tried %d times but", consumerCreatingRepeatTime)
			panic(err)
		}
	}
}

func startConsuming() {
	for {
		kafkaMessage, err := consumer.ReadMessage(-100)
		if err != nil {
			fmt.Println(err.Error())
		} else {
			kafkaMessages[receivedMessageCount] = string(kafkaMessage.Value)
			fmt.Println(kafkaMessages[receivedMessageCount])
			receivedMessageCount++

			if receivedMessageCount >= maxMessageCountToAccumulate {
				commitMessagesDatabase()
			}
		}
	}
}

func commitMessagesDatabase() {
	dbTransaction, _ := db.Begin()

	for i := 0; i < maxMessageCountToAccumulate; i++ {
		splittedMsg := strings.Split(kafkaMessages[i], " ")
		dbTransaction.Exec(insertStatement,
			splittedMsg[0], // method
			splittedMsg[1], // elapsed time
			splittedMsg[2], // utc timestamp
		)
	}

	err := dbTransaction.Commit()

	if err != nil {
		fmt.Println(err.Error())
	} else {
		fmt.Printf("The last %d messages that received has been committed.\n", maxMessageCountToAccumulate)
		receivedMessageCount = 0
	}
}

func main() {
	time.Sleep(10 * time.Second)
	setDatabase()
	defer db.Close()

	createConsumer()

subscribeStatement:
	err := consumer.SubscribeTopics(kafkaTopics, nil)

	if err == nil {
		fmt.Println("Consumer subscribed the topic")
	} else {
		fmt.Println(err.Error() + "\n Trying again")
		goto subscribeStatement
	}

	startConsuming()
}
