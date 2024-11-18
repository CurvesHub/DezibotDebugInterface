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
- The back-end is available at `http://localhost:5160`
- The back-end API documentation is available at `http://localhost:5160/api`

## Usage

- Start the Dezibot code on a device
- Start the back-end server
- Start the front-end server
- Open the front-end in a browser
- The front-end will display the data of the Dezibots

## Back-end API

The back-end provides the following endpoints:

- `/api` - A UI to view the open api documentation of the API and test the endpoints


- `GET /api/dezibot` - Get all Dezibots
- `GET /api/dezibot/[ip]` - Get a Dezibot by ip address
- `WS /dezibot-hub` - "ws://localhost:port/dezibot-hub" - Websocket for the front-end to receive data


- `PUT /api/broadcast/states` - Receives periodically state data from debuggables (components) including properties 
  and their values from Dezibots
- `PUT /api/broadcast/logs` - Receives log messages from debuggables (components) from Dezibots


### Models

#### GET Dezibot 

- `GET /api/dezibot`
- `GET /api/dezibot/[ip]`
- `/dezibot-hub`

```json
[
  {
    "ip": "1.1.1.1",
    "lastConnectionUtc": "2024-11-18T16:44:58.9018651Z",
    "debuggables": [
      {
        "name": "Display",
        "properties": [
          {
            "name": "currentLine",
            "values": [
              {
                "timestampUtc": "2024-01-01T12:00:00Z",
                "value": "12"
              },
              {
                "timestampUtc": "2024-01-01T12:00:10Z",
                "value": "18"
              },
              {
                "timestampUtc": "2024-01-01T12:00:20Z",
                "value": "0"
              }
            ]
          },
          {
            "name": "isFlipped",
            "values": [
              {
                "timestampUtc": "2024-01-01T12:00:00Z",
                "value": "false"
              },
              {
                "timestampUtc": "2024-01-01T12:00:10Z",
                "value": "false"
              },
              {
                "timestampUtc": "2024-01-01T12:00:20Z",
                "value": "true"
              }
            ]
          }
        ]
      },
      {
        "name": "Motor",
        "properties": [
          {
            "name": "speed",
            "values": [
              {
                "timestampUtc": "2024-01-01T12:00:00Z",
                "value": "25"
              }
            ]
          },
          {
            "name": "voltage",
            "values": [
              {
                "timestampUtc": "2024-01-01T12:00:00Z",
                "value": "1.5"
              }
            ]
          }
        ]
      }
    ],
    "logs": [
      {
        "timestampUtc": "2024-01-01T12:00:00Z",
        "logLevel": "INFO",
        "message": "This is a test message."
      },
      {
        "timestampUtc": "2024-01-02T12:00:00Z",
        "logLevel": "INFO",
        "message": "This is a test message one day later."
      }
    ]
  }
]
```

#### `PUT /api/broadcast/states`

```json
{
  "ip": "1.1.1.1",
  "debuggables": [
    {
      "name": "Display",
      "properties": [
        {
          "name": "currentLine",
          "values": [
            {
              "timestampUtc": "2024-01-01T12:00:00.000Z",
              "value": "12"
            }
          ]
        }
      ]
    }
  ]
}
```

#### `PUT /api/broadcast/logs`

```json
{
  "ip": "1.1.1.1",
  "timestampUtc": "2024-01-01T12:00:00.000Z",
  "logLevel": "INFO",
  "message": "This is a test message."
}
```
