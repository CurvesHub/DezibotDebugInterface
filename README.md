# Dezibot Debug Interface

This project includes a back-end and front-end web server to store and display debugging and log data of the Dezibot.
The Dezibot Code is a separate project and can be found [here](https://github.com/CurvesHub/dezibot).

## Features

- A Dezibot debugger to collect data and broadcast it to a web sever. The program should be callable by or runs 
  parallel to the executing program, but is independent.
- A back-end sever to receive, organise and save the data of various bots. It provides one endpoint for the dezibots 
  to broadcast to and two endpoints for the front-end to receive the data.
- A front-end server which provides a web GUI displaying the data of the bots in a grid and a details view.

## Installation

All services are dockerized and can be started with docker-compose. Follow the instructions below to start the services.

- Clone the repository
- Run `docker-compose up` in the root directory of the project
- The front-end is available at `http://localhost:3000`
- The back-end is available at `http://localhost:5000`
- The back-end API documentation is available at `http://localhost:5000/api/scalar`

## Back-end API

The back-end provides the following endpoints:

- `GET /api/scalar` - A UI to view the openapi documentation of the API and test the endpoints


- `GET /api/dezibot` - Get all Dezibots
- `GET /api/dezibot/[ip]` - Get a Dezibot by ip address


- `PUT /api/dezibot/broadcast` - Receive data from Dezibots (Websocket)

**HTTP Status Codes:**
- 200: OK
- 400: Bad Request
- 404: Not Found
- 500: Internal Server Error

### GET Dezibots

**Model:**

```json5
[
    {
        "ip": "string", // Unique identifier
        "lastConnectionUtc": "string",
        "components": [
            {
                "properties": [
                    {
                        "propertyName": "value"
                    }
                ],
                "messages": [
                    {
                        "message": "string",
                        "data": "string"
                    }
                ]
            }
        ]
    }
]
```

**Example:**

```json5
[
    {
        "ip": "1.2.3.4",
        "lastConnectionUtc": "01.01.2024 12.00.000Z",
        "components": [
            {
                "name": "DISPLAY",
                "properties": [
                    {
                        "updatedAtUtc": "01.01.2024 11.50.000Z",
                        "displayFlipped": "true",
                        "currentLine": "10",
                        "...": "..."
                    },
                    {
                        "updatedAtUtc": "01.01.2024 11.51.000Z",
                        "displayFlipped": "false",
                        "currentLine": "12", 
                        "...": "..."
                    }
                ],
                "messages": [
                    {   // Example color detection log
                        "message": "read double register",
                        "data": "1:14"
                    },
                    {   // Example color detection log 2
                        "message": "read double register",
                        "data": "1:18"
                    }
                ]
            }
        ]
    }
]
```

### PUT Dezibot broadcast

**Model:**

```json5
{
  "ip": "string", // Unique identifier
  "event": "string",
  "class": "string",
  "propName": "string", // nullable
  "message": "string", // nullable 
  "value": "string"
}
```

**Example:**

Event: `propertyChanged`
```json5
{
  "ip": "1.2.3.4",
  "event": "propertyChanged",
  "class": "DISPLAY",
  "propName": "displayText",
  // "message": "string",
  "value": "Hello from dezibot!"
}
```

Event: `message`
```json5 
{
  "ip": "1.2.3.4",
  "event": "message",
  "class": "DISPLAY",
  // "propName": "string",
  "message": "read double register",
  "value": "1:18"
}
```