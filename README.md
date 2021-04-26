<span align="center">

<a href="https://codeclimate.com/github/halilkocaoz/kafka-response-log/maintainability"><img src="https://api.codeclimate.com/v1/badges/d1364da3e1590a452ab9/maintainability" /></a> <a href="https://www.codacy.com/gh/halilkocaoz/kafka-response-log/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=halilkocaoz/kafka-response-log&amp;utm_campaign=Badge_Grade"><img src="https://app.codacy.com/project/badge/Grade/5d7c3538a0d144beaac9ef265710f613"/></a>

</span>

</hr>

# kafka-response-log
It contains a solution for tracking the request processing time, messaging the tracked data to Kafka, and inserting the Kafka messages into the database. Also, there is a simple UI to see the logs in a chart.

<hr>

## Table of contents
* [Docker](#docker)
* [Automatization of the requests with Shell and curl](#automatization-of-the-requests-with-shell-and-curl)
* [Dashboard](#dashboard)
* [Docs](#docs)
  - [ASP.NET 5 Web API](#aspnet-5-web-api)
  - [Go Project : Kafka Consumer and Database Updater](#go-project--kafka-consumer-and-database-updater)
* [Contact](#contact)

<hr>

## Docker
Runs 5 services;
* Apache ZooKeeper
* Apache Kafka
* ASP.NET 5 Web API
* Go Project : Kafka Consumer and Database Updater 
* Postgres Database

```bash
docker-compose up --build
```
```bash
docker-compose up
```

## Automatization of the requests with Shell and curl
[request.sh](https://github.com/halilkocaoz/kafka-response-log/tree/main/request.sh) provides to automate the requests but it needs shell script runner and curl.

```bash
./request.sh
```

## Dashboard
You can use the `http://localhost:1923/` address to watch the live dashboard. <br> Also, on top of the dashboard, you will see the different HTTP method names. You can click those to filter which method is shown in the chart.


## Docs

## ASP.NET 5 Web API
[ASP.NET 5 Web API, web server project](https://github.com/halilkocaoz/kafka-response-log/tree/main/server/).

The web server has two endpoint,

1. /api/products        [GET, POST, PUT, DELETE]
2. /health/api/products [GET]

### /api/products
`/api/products` endpoint's for creating dummy data to log. This endpoint has [Delayer](https://github.com/halilkocaoz/kafka-response-log/tree/main/server/Filters/Delayer.cs) and [TimeTracker](https://github.com/halilkocaoz/kafka-response-log/tree/main/server/Filters/TimeTracker.cs). It's mean that every request will wait for a random time before processing at the endpoint and elapsed time to response will be logged by TimeTracker.

#### Consuming the /api/products endpoint
The `/api/products` endpoint supports to GET, POST, PUT, DELETE methods. You can use the `http://localhost:1923/api/products` address for your requests. <br>

#### Returns of /api/products endpoint for GET, POST, PUT, DELETE methods
All of the above methods return 204.

<hr>

### /health/api/products
This endpoint presents the following data about requests to `/api/products` in the last hour;
* HTTP Method
* Elapsed time to response
* When was it logged?

#### Consuming the /health/api/products endpoint
This endpoint just support GET method. You can consume the data by using the GET method at the `http://localhost:1923/health/api/products/` address.

#### Returns of /health/api/products endpoint for GET method
* 204 <br>
If there is no request that made to `/api/products` in the last hour, it returns 204. <br>
* 200 <br>
  Returns JSON data that give information about requests to `/api/products` in last hour.


```json
[
  {
    "method": "GET",
    "elapsedTime": 375,
    "timestamp": 1616368665
  }
]
```

## Go Project : Kafka Consumer and Database Updater
[Go Project](https://github.com/halilkocaoz/kafka-response-log/tree/main/consumer) consumes the kafka messages and writes those messages to the database.

### Working Principle of Go Project
It collects the messages from the `response_log` topic as `go-consumer` and it accumulates that data in a fixed size string array called `kafkaMessages`. This array's size is fixed using `maxMessageCountToAccumulate` variable and consumer collects messages until `receivedMessageCount` reaches `maxMessageCountToAccumulate`.

If `receivedMessageCount` >= `maxMessageCountToAccumulate`, write to database method runs and it inserts all of the collected messages into the database with **one transaction**. After then, `receivedMessageCount` sets to zero and the consumer resumes to collect data from last offset until again repeat.

### The consumer doesn't immediately insert received messages from Kafka into the database
As I mentioned above, the `receivedMessageCount` must reach `maxMessageCountToAccumulate` to start the database transaction and insert received messages with a single transaction.
