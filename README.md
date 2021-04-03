<span align="center">

<a href="https://codeclimate.com/github/halilkocaoz/kafka-response-log/maintainability"><img src="https://api.codeclimate.com/v1/badges/d1364da3e1590a452ab9/maintainability" /></a> <a href="https://www.codacy.com/gh/halilkocaoz/kafka-response-log/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=halilkocaoz/kafka-response-log&amp;utm_campaign=Badge_Grade"><img src="https://app.codacy.com/project/badge/Grade/5d7c3538a0d144beaac9ef265710f613"/></a>

</span>

</hr>

# kafka-response-log
It contains a solution for tracking the time between request and response, messaging the tracked data to Kafka, and inserting the Kafka messages into the database. Also, there is a simple UI to see the logs in a chart.

<hr>

## Table of contents
* [Docker](#docker)
* [Automatization of the requests with Shell and curl](#automatization-of-the-requests-with-shell-and-curl)
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

<hr>

## Automatization of the requests with Shell and curl
[request.sh](https://github.com/halilkocaoz/kafka-response-log/tree/main/request.sh) provides to automate the requests but it needs shell script runner and curl.

```bash
./request.sh
```

<hr>

## Dashboard
You can use the `http://localhost:1923/` address for reaching the dashboard.

<hr>

## Docs

## ASP.NET 5 Web API
You can see the ASP.NET 5 Web API project in [this directory](https://github.com/halilkocaoz/kafka-response-log/tree/main/server/).

The project has two end-point paths,

1. /health/api/products [GET]
2. /api/products   [GET, POST, PUT, DELETE]

### /health/api/products
This endpoint path presents the following data about requests to `/api/products` in the last hour;
* HTTP Method
* Elapsed time to response
* When was it logged?

#### Using the /health/api/products endpoint
GET : `http://localhost:1923/health/api/products`

#### Returns of GET: /health/api/products endpoint
* 204 <br>
If there is no request that made to `/api/products` in the last hour, it returns 204. <br>
* 200 <br>
  Returns JSON data that give information about requests to `/api/products` in last hour.
```json
[
  {
    "method": "GET",
    "elapsedTime": 375,
    "timestamputc": 1616368665
  },
  {
    "method": "GET",
    "elapsedTime": 200,
    "timestamputc": 1616368655
  },
  {
    "method": "PUT",
    "elapsedTime": 1200,
    "timestamputc": 1616368610
  }
]
```

### /api/products
An endpoint path that presents GET, POST, PUT and DELETE methods. That endpoints are for creating dummy data to log. All of the methods have [delayer](https://github.com/halilkocaoz/kafka-response-log/tree/main/server/Filters/Delayer.cs) middleware (known as ActionFilterAttribute in the .NET ecosystem).

#### Using the /api/products endpoint
The `/api/products` path supports to GET, POST, PUT, DELETE methods. You can use the `http://localhost:1923/api/products` address for your requests. <br>

#### Returns of /api/products endpoint
All of the above methods return 204, other 405.

<hr>

## Go Project : Kafka Consumer and Database Updater
You can look at the Go Project from [here](https://github.com/halilkocaoz/kafka-response-log/tree/main/consumer). It consumes the kafka messages and writes those messages to the database.

### Working Principle of Go Project
It collects the messages from the `response_log` topic as `go-consumer` and it accumulates that data in a fixed size string array called `kafkaMessages`. This array's size is fixed using `maxMessageCountToAccumulate` variable and consumer collects messages until `receivedMessageCount` reaches `maxMessageCountToAccumulate`.

If `receivedMessageCount` >= `maxMessageCountToAccumulate`, write to database method runs and it inserts all of the collected messages into the database with **one transaction**. After then, `receivedMessageCount` sets to zero and the consumer resumes to collect data from last offset until again repeat.

### The consumer doesn't immediately insert received messages from Kafka into the database
As I mentioned above, the `receivedMessageCount` must reach `maxMessageCountToAccumulate` to start the database transaction and insert received messages with a single transaction.

<hr>

<span align="center">

## Contact

halilkocaoz (Telegram)<br>
halil.i.kocaoz@gmail.com

</span>

```bash
gAAAAABgW4L5MsXduWXqf5NlCjwLsU20YHjR5NbmlUMppaEVlbBM1YlgAuJuqP9YKHggi4E7LWxsZ-cQv4kOl29BaKCPAr4TsP1mlZKe01AuMsujr3npPfVle4W2icQgNF5h_VIIBqPSsFK_50m7lBRJUpeO8iFQgD2YO9STT6nivjULUDSuR3t0U-4S2OC36cornhgmf0ZdgN5Sbh4Oi78jCeo1UnJu3w==
```
