package main

import (
	"database/sql"
	"fmt"
	"strings"

	_ "github.com/lib/pq"
	"gopkg.in/confluentinc/confluent-kafka-go.v1/kafka"
)

const insertStatement = `INSERT INTO net_logs (method, elapsed_time, timestamp) VALUES ($1, $2, $3)`

func main() {
	db, err := sql.Open("postgres", "postgres://postgres:psqlpass@localhost/response_log?sslmode=disable")
	if err != nil {
		panic(err)
	}
	defer db.Close()

	c, err := kafka.NewConsumer(&kafka.ConfigMap{
		"bootstrap.servers": "localhost:19093, localhost:19092",
		"group.id":          "test",
		"auto.offset.reset": "earliest",
	})

	if err != nil {
		panic(err)
	}

	c.SubscribeTopics([]string{"response_log"}, nil)
	/* 	think about when you must write to database /
	Where should I prefer to read statistics to show on the dashboard? Kafka or Database > WebAPI.
	*/
	fmt.Println("Listening")
	for {
		message, err := c.ReadMessage(-1)
		if err == nil {
			fmt.Println(string(message.Value))
			m := strings.Split(string(message.Value), " ")
			db.QueryRow(insertStatement, m[0], m[1], message.Timestamp).Scan()
		} else {
			fmt.Println("Error:\n%v (%v)", err, message)
		}
	}
	c.Close()
}
