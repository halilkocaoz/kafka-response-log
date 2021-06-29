package main

import (
	"database/sql"
	"fmt"
	"log"
	"strings"
	"time"

	_ "github.com/lib/pq"
	"gopkg.in/confluentinc/confluent-kafka-go.v1/kafka"
)

const (
	maxMessageCountToAccumulate = 10
	insertStatement             = `INSERT INTO net_logs (method, elapsed_time, timestamp) VALUES ($1, $2, $3)`
	database                    = "postgres"
	databaseUser                = "postgres"
	databasePass                = "psqlpass"
	databaseServer              = "postgres:5432"
	kafkaServer                 = "kafka:9092"
)

var (
	kafkaTopics          = []string{"response_log"}
	kafkaMessages        [maxMessageCountToAccumulate]string
	receivedMessageCount = 0
)

func initDatabase(db *sql.DB) {
	db.Exec(`create table if not exists net_logs
	(
		method varchar,
		elapsed_time int,
		timestamp bigint
	);`)
}

func consumeAndInsertMessages(consumer *kafka.Consumer, db *sql.DB) error {
	var (
		err          error
		kafkaMessage *kafka.Message
	)

	for {
		kafkaMessage, err = consumer.ReadMessage(-1)
		kafkaMessages[receivedMessageCount] = string(kafkaMessage.Value)
		fmt.Println(kafkaMessages[receivedMessageCount])
		receivedMessageCount++

		if receivedMessageCount >= maxMessageCountToAccumulate {
			err = insertMessages(db)
		}

		if err != nil {
			return err
		}
	}
}

func insertMessages(db *sql.DB) error {
	dbTransaction, _ := db.Begin()

	for i := 0; i < maxMessageCountToAccumulate; i++ {
		splittedMsg := strings.Split(kafkaMessages[i], " ")
		dbTransaction.Exec(insertStatement,
			splittedMsg[0], // method
			splittedMsg[1], // elapsed time
			splittedMsg[2], // timestamp
		)
	}

	receivedMessageCount = 0
	return dbTransaction.Commit()
}

func main() {
	var (
		consumer *kafka.Consumer
	)

	db, _ := sql.Open("postgres", "postgres://"+databaseUser+":"+databasePass+"@"+databaseServer+"/"+database+"?sslmode=disable")
	initDatabase(db)
	defer db.Close()
	time.Sleep(10 * time.Second)

	consumer, _ = kafka.NewConsumer(&kafka.ConfigMap{
		"bootstrap.servers": kafkaServer,
		"group.id":          "go-consumer",
		"auto.offset.reset": "earliest",
	})
	consumer.SubscribeTopics(kafkaTopics, nil)

consuming:
	err := consumeAndInsertMessages(consumer, db)

	if err != nil {
		log.Fatal(err)
		goto consuming
	}
}
