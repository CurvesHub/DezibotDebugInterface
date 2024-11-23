# Dezibot Debug Interface

This project includes a back-end and front-end web server to store and display debugging and log data of the Dezibot.
The Dezibot Code is a separate project and can be found [here](https://github.com/CurvesHub/dezibot).

## Goals

- A Dezibot debugger to collect data and broadcast it to a web sever. The program should be callable by or runs 
  parallel to the executing program, but is independent.
- A back-end sever to receive, organise and save the data of various bots. It provides one endpoint for the dezibots 
  to broadcast to and two endpoints for the front-end to receive the data.
- A front-end server which provides a web GUI displaying the data of the bots in a grid and a details view.

## Installation

All services are dockerized and can be started with docker-compose. Follow the instructions below to start the services.

- Clone the repository
- Run `docker-compose up` in the root directory of the project
- The front-end is available at `...`
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
- `PUT /api/dezibot/update` - Receives state data or logs from dezibot classes

### Models

#### GET Dezibot 

- `GET /api/dezibot`
- `GET /api/dezibot/[ip]`
- `/dezibot-hub`

```json5
// Work in progress
```

#### `PUT /api/dezibot/update`

```json5
// Work in progress
```
