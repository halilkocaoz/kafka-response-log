<!--- key

gAAAAABgUWKB2jyN8QxVQ8s38TQF553f3CFNzWMFlXCGArb40zwz1sQ757-P5dUa2MGSQKIreeC9K8O2J8SSmQWsmMeLkJzhzCVUhipOJUubLthP9V8QFiKNgV_xMNfz3maUXokRulkGJDmwU69IMngX3DvSA-Q7QfoSRBVFhjM_4JjSWtiHNCgkQonPl33G9Gs_JpKZL3fcTn35J3hyiZZXAzqn

--->

<span align="center">
<a href="https://codeclimate.com/github/halilkocaoz/kafka-response-time-tracking/maintainability"><img src="https://api.codeclimate.com/v1/badges/9dc73c64fdfe2c32418a/maintainability" /></a><a href="https://www.codacy.com/gh/halilkocaoz/kafka-response-time-tracking/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=halilkocaoz/kafka-response-time-tracking&amp;utm_campaign=Badge_Grade"><img src="https://app.codacy.com/project/badge/Grade/5d7c3538a0d144beaac9ef265710f613" /></a>
</span>

</hr>


# kafka-response-log
It contains a solution for tracking the time between request and response, messaging the tracked data to Kafka and inserting the messages which are coming from Kafka into the database, also it contains an simple UI to see the logs in a chart.

<hr>

## Table of contents
* [Docker](#docker)
* [Docs](#docs)
  - [ASP.NET 5 Web API](#aspnet-5-web-api)
  - [Go Project : Kafka Consumer and Database Updater](#go-project--kafka-consumer-and-database-updater)
* [Contact](#contact)

<hr>

## Docker 
Docker compose runs 5 service;
* Apache ZooKeeper
* Apache Kafka
* ASP.NET 5 Web API
* Go Project : Kafka Consumer and Database Updater 
* Postgres Database

First time:
```bash
docker-compose up --build
```
Afer the first time:
```bash
docker-compose up
```

<hr>

## Docs

## ASP.NET 5 Web API
You can see the ASP.NET 5 Web API project in [this directory](https://github.com/halilkocaoz/kafka-response-time-tracking/tree/main/server/Kartaca.Intern).

The project has two end-point paths,

1. /health/api/products [GET]
2. /api/products   [GET, POST, PUT, DELETE]

### /health/api/products
This endpoint path presents the following data about requests to `/api/products` in the last hour;
* HTTP Method
* Elapsed time to response
* When was it requested?


#### Using the /health/api/products endpoint
GET : `http://localhost:1923/health/api/products`

#### Returns of GET: /health/api/products endpoint
* 204 <br>
If there is no request that made to `/api/products` in the last hour, you get 204. <br>
* 200 <br>
  Returns JSON data that give information about requests to `/api/products` in last hour.
```json
[
  {
    "method": "GET",
    "elapsedTime": 375,
    "timestamp": 1616368665
  }
  {
    "method": "GET",
    "elapsedTime": 200,
    "timestamp": 1616368655
  }
  {
    "method": "PUT",
    "elapsedTime": 1200,
    "timestamp": 1616368610
  }
]
```

### /api/products
A end-point path that presents GET, POST, PUT and DELETE methods. That endpoints are for create dummy data to log and all of the methods have [delayer](https://github.com/halilkocaoz/kafka-response-time-tracking/tree/main/server/Kartaca.Intern/Filters/Delayer.cs) middleware (known as ActionFilterAttribute in .NET ecosystem).

#### Using the /api/products endpoint
`/api/products` path supports to GET, POST, PUT, DELETE methods. You can use `http://localhost:1923/api/products` address to your requests. <br>

`curl -X GET http://localhost:1923/api/products` <br>
`curl -X POST http://localhost:1923/api/products` <br>
`curl -X PUT http://localhost:1923/api/products` <br>
`curl -X DELETE http://localhost:1923/api/products` <br>

#### Returns of /api/products endpoint
All of the above methods return 204, other 405.

#### Life-cycle of a request to /api/products endpoint as flowchart
[TimeTrackerMiddleware](https://github.com/halilkocaoz/kafka-response-time-tracking/tree/main/server/Kartaca.Intern/Middlewares/TimeTrackerMiddleware.cs)
![life-cycle](https://github.com/halilkocaoz/kafka-response-time-tracking/blob/main/assets/life-cycle-request.png "life-cycle")

<hr>

## Go Project : Kafka Consumer and Database Updater 
You can look at the Go Project from [here](https://github.com/halilkocaoz/kafka-response-time-tracking/tree/main/consumer). It consumes the messages which are coming from ASP.NET Web API and write that messages to database.

### Working Principle of Go Project
It collects the messages from `response_log` topic as `go-consumer` and it accumulates that data in a fixed size string array called `kafkaMessages`. This array's size fixed using `maxMessageCountToAccumulate` const variable and consumer collects messages until `receivedMessageCount` reaches `maxMessageCountToAccumulate`.

If `receivedMessageCount` >= `maxMessageCountToAccumulate`, write to database method runs and it inserts all of the collected messages into the database with **one transaction**. After then, `receivedMessageCount` sets to zero and the consumer continues to collect data from last offset until again `receivedMessageCount` reaches `maxMessageCountToAccumulate`.


### The consumer immediately doesn't insert received messages from Kafka into the database
As I mentioned above, the `receivedMessageCount` must reaches `maxMessageCountToAccumulate` to start the database transaction and insert received messages with a single transaction.

<hr>

<span align="center">

## Contact
If you have any question about the repository or you just want to say something be free to reach me <br>


halilkocaoz (Telegram)<br>
halil.i.kocaoz@gmail.com

</span>