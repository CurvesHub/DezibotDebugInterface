# Dezibot Debug Interface

This project includes a backend and frontend web server to store and display debugging and log data of the Dezibot.
The Dezibot Code is a separate project and can be found [here](https://github.com/CurvesHub/dezibot).

## Goals

- A Dezibot debugger to collect data and broadcast it to a web sever. The program should be callable by or runs 
  parallel to the executing program, but is independent.
- A backend sever to receive, organise and save the data of various bots. It provides one endpoint for the dezibots 
  to broadcast to and two endpoints for the frontend to receive the data.
- A frontend server which provides a web GUI displaying the data of the bots in a grid and a details view.

## Installation

All services are dockerized and can be started with docker-compose. Follow the instructions below to start the services.

- Clone the repository
- Run `docker-compose up` in the root directory of the project
- The frontend is available at `http://localhost:3000`
- The backend is available at `http://localhost:5160`
- The backend API documentation is available at `http://localhost:5160/api`

## Usage

- Start the Dezibot code on a device
- Start the backend server
- Start the frontend server
- Open the frontend in a browser
- The frontend will display the data of the Dezibots

### Attention

When writing a dezibot main program, make sure to call `Log::begin(ssid, password, url)` before calling 
`dezibot.begin()`. Otherwise, the initial setup logs and debug data will not be sent to the backend server.

If `Log::begin(ssid, password, url)` is not called all called methods of the `Log` class will return immediately.

## Backend API

The backend provides the following endpoints:

- `/api` - A UI to view the open api documentation of the API and test the endpoints
- `GET /api/dezibot` - Get all Dezibots
- `GET /api/dezibot/[ip]` - Get a Dezibot by ip address
- `WS /api/dezibot-hub` - "ws://localhost:port/api/dezibot-hub" - Websocket for the frontend to receive data
- `PUT /api/dezibot/update` - Receives state data or logs from dezibot classes

### Models

#### GET Dezibot 

- `GET /api/dezibot`
- `GET /api/dezibot/[ip]`
- `/api/dezibot-hub`

```json
[
  {
    "ip": "111.222.333.444",
    "lastConnectionUtc": "2024-01-02T00:00:00",
    "logs": [
      {
        "timestampUtc": "2024-01-01T00:00:00",
        "level": "INFO",
        "className": "DISPLAY",
        "message": "My first message",
        "data": "Some data"
      },
      {
        "timestampUtc": "2024-01-02T00:00:00",
        "level": "INFO",
        "className": "DISPLAY",
        "message": "My second message",
        "data": "Some data"
      },
      {
        "timestampUtc": "2024-01-03T00:00:00",
        "level": "INFO",
        "className": "DISPLAY",
        "message": "My second message",
        "data": null
      }
    ],
    "classes": [
      {
        "name": "DISPLAY",
        "properties": [
          {
            "name": "currentLine",
            "values": [
              {
                "timestampUtc": "2024-01-01T00:00:00",
                "value": "12"
              },
              {
                "timestampUtc": "2024-01-02T00:00:00",
                "value": "12"
              }
            ]
          },
          {
            "name": "isFlipped",
            "values": [
              {
                "timestampUtc": "2024-01-01T00:00:00",
                "value": "true"
              },
              {
                "timestampUtc": "2024-01-02T00:00:00",
                "value": "true"
              }
            ]
          }
        ]
      },
      {
        "name": "className",
        "properties": [
          {
            "name": "propertyName1",
            "values": [
              {
                "timestampUtc": "2024-01-01T00:00:00",
                "value": "value1"
              },
              {
                "timestampUtc": "2024-01-02T00:00:00",
                "value": "value1"
              }
            ]
          },
          {
            "name": "propertyName2",
            "values": [
              {
                "timestampUtc": "2024-01-01T00:00:00",
                "value": "value2"
              },
              {
                "timestampUtc": "2024-01-02T00:00:00",
                "value": "value2"
              }
            ]
          }
        ]
      }
    ]
  }
]
```

#### `PUT /api/dezibot/update`

##### State Data

```json
{
  "Ip": "111.222.333.444",
  "Data": {
    "className": {
      "propertyName1": "value1",
      "propertyName2": "value2"
    },
    "DISPLAY": {
      "isFlipped": "true",
      "currentLine": "12"
    }
  }
}
```

##### Log Data

```json
{
  "Ip": "111.222.333.444",
  "logLevel": "INFO",
  "className": "DISPLAY",
  "message": "My first message",
  "data": "Some data"
}
```