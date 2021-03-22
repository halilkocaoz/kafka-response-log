<!--- key

gAAAAABgUWKB2jyN8QxVQ8s38TQF553f3CFNzWMFlXCGArb40zwz1sQ757-P5dUa2MGSQKIreeC9K8O2J8SSmQWsmMeLkJzhzCVUhipOJUubLthP9V8QFiKNgV_xMNfz3maUXokRulkGJDmwU69IMngX3DvSA-Q7QfoSRBVFhjM_4JjSWtiHNCgkQonPl33G9Gs_JpKZL3fcTn35J3hyiZZXAzqn

--->

# kartaca-task
This repository contains the my solutions of application and service development from Kartaca's tasks.

## Table of contents
* [Setup](#setup)
* [Docs](#docs)
  - [ASP.NET 5 Web API](#aspnet-5-web-api)
* [Techs](#techs)
* [Contact](#contact)

## Setup

Docker compose runs 5 service;
* Apache ZooKeeper
* Apache Kafka
* ASP.NET 5 Web API
* Go Project : Kafka Consumer and Database Updater 
* Postgres Database

```bash
docker-compose up build
```

## Docs

## ASP.NET 5 Web API
 You can see the ASP.NET 5 Web API project in [this directory](https://github.com/halilkocaoz/kartaca-task/tree/main/server/Kartaca.Intern).

The project has two end-point paths,

1. /health/api/products [GET]
2. /api/products   [GET, POST, PUT, DELETE]

### /health/api/products
This endpoint presents the following data about requests to /api/products in the last hour;
* HTTP Method
* Elapsed time to response
* When was it requested?

<hr>

#### Using the /health/api/products
GET <br>
`http://localhost:1923/health/api/products`

<hr>

#### 1.2 Returns of GET: /health/api/products
* 204 <br>
If there is no request that made to `/api/products` in the last hour, you get 204. <br>
* 200 <br>
  Returns of JSON data that give information about requests that to `/api/products` in last hour.
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
<hr>

### 2. /api/products
A end-point that presents GET, POST, PUT and DELETE methods.

<hr>

#### 2.1 Using the /api/products
GET, POST, PUT, DELETE <br>
`http://localhost:1923/api/products` <br>

<hr>

#### 2.2 curl: /api/products
`curl -X GET http://localhost:1923/api/products` <br>
`curl -X POST http://localhost:1923/api/products` <br>
`curl -X PUT http://localhost:1923/api/products` <br>
`curl -X DELETE http://localhost:1923/api/products` <br>

<hr>

#### 2.3 Returns of /api/products
All of the above methods return 204.

<hr>

#### 2.4 Life-cycle of a request to /api/products as flowchart
![life-cycle](https://github.com/halilkocaoz/kartaca-task/blob/main/assets/life-cycle-request.png "life-cycle")

<hr>

## Go Project : Kafka Consumer and Database Updater 
You can look at the Go Project at [here](https://github.com/halilkocaoz/kartaca-task/tree/main/consumer). It's consuming the messages which are coming from ASP.NET Web API and write that messages to database.

### Working Principle of Go Project
It collects the messages from `response_log` topic as `go-consumer` and it accumulates that data in a fixed size string array called `kafkaMessages`. This array's size fixed using `maxMessageCountToAccumulate` const variable and consumer collects messages until `receivedMessageCount` reachs `maxMessageCountToAccumulate`. 


If `receivedMessageCount` >= `maxMessageCountToAccumulate`, write to database method run and it inserts all of the collected messages to database with **one transaction**. After then, `receivedMessageCount` sets to zero and consumer continues to collect data from last offset until again `receivedMessageCount` reach to `maxMessageCountToAccumulate`.


## Status
Continue

## Contact
If you have any question about the repository or you just want to say something be free to reach me <br>
Telegram: halilkocaoz <br>
Mail: halil.i.kocaoz@gmail.com
