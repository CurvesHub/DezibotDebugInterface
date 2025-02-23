# Dezibot Debug Interface

The Dezibot Debug Interface provides a way to log data from a Dezibot to a backend server and display it in a web 
interface. This repository contains the code for the backend and frontend server. The code for the Dezibot can be 
found [here](https://github.com/CurvesHub/dezibot).

## Table of Contents

1. [Overview](#overview)
2. [Installation](#installation)
3. [Usage](#usage)
    - [Attention](#attention)
4. [Dezibot Log Class](#dezibot-log-class)
   - [Methods](#methods)
   - [Example](#example)
5. [Backend API](#backend-api)
6. [Session Handling](#session-handling)
7. [Models](#models)
8. [License](#license)

## Overview

The Dezibot Debug Interface consists of three main components:

1. **Dezibot**: The Dezibot is a small robot, with a lot of sensors, that can be programmed to perform various tasks.
   The Dezibot code is responsible for sending data to the backend server.
2. **Backend Server**: The backend server receives data from the Dezibot and stores it in a database. It also 
   provides an API for the frontend to retrieve the data.
3. **Frontend Server**: The frontend server displays the data from the Dezibot in a web interface. It allows users 
   to view the data in real-time and analyze it.

The main goal of the Dezibot Debug Interface is to provide a way to log data from the Dezibot and display it in a 
user-friendly interface. This allows users to monitor the behavior of the Dezibot and debug any issues that may arise 
during operation. The interface supports multiple Dezibots and user sessions, ensuring that each user can view the 
data in real-time without interfering with other users.

## Installation

All services are dockerized and can be started with docker-compose. Follow the instructions below to start the services.

1. Clone the repository
2. Configure the `docker-compose.yml` file with the desired environment variables

## Usage

This is a step-by-step guide on how to use the Dezibot Debug Interface with the example code provided in the 
`log_demo_simple.ino` file.

1. Prepare Dezibot Code Example
   - Start a mobile hotspot or use an active WiFi network
   - Connect the host machine (where docker runs) to the WiFi network
   - Enter the Wifi SSID and password in the `log_demo_simple.ino` file
   - Enter the IP address of the host machine where the backend server will be running
   - Load the code onto a Dezibot
2. Run `docker-compose up` in the root directory of the project
   - The frontend is available at `http://localhost:3000`
   - The backend is available at `http://localhost:5160`
   - The backend API documentation is available at `http://localhost:5160/api`
3. Open the frontend in a browser
4. Start a new session or join an existing one
   - Set the continue session toggle to `true` to receive new data from dezibots into this session
   - Set the continue session toggle to `false` to only view this session without receiving new data
5. The frontend will display the data of the session with its Dezibots

### Attention

When writing a Dezibot main program, make sure to call `Log::begin(ssid, password, url)` before calling `dezibot.begin()`. Otherwise, the initial setup logs and debug data will not be sent to the backend server.

If `Log::begin(ssid, password, url)` is **not** called, all called methods of the `Log` class will return immediately.

## Dezibot Log Class

The `Log` class provides methods to log events and state changes to a web server over WiFi. It supports different 
log levels and can send data periodically or on-demand.

### Methods

1. **Initialization**
    - **`Log::begin(const char* wifiSSID, const char* wifiPassword, String url)`**
        - Connects to the specified WiFi network and sets up the logging server URL.
        - **Parameters:**
            - `wifiSSID`: The SSID of the WiFi network.
            - `wifiPassword`: The password for the WiFi network.
            - `url`: The URL of the web server for logging data.

2. **Logging Property Changes**
    - **`Log::propertyChanged(String className, String propertyName, String newValue)`**
        - Logs a property change event for a given class and property.
        - **Parameters:**
            - `className`: The name of the class where the property changed.
            - `propertyName`: The name of the property that changed.
            - `newValue`: The new value of the property.

3. **Logging Messages**
    - **`Log::d(DezibotLogLevel level, String className, String message, String data = "")`**
        - Logs a message with a specified log level and associated data payload.
        - **Parameters:**
            - `level`: The log level of the message (`DEBUGLOG`, `INFOLOG`, `WARNLOG`, `ERRORLOG`).
            - `className`: The name of the class generating the message.
            - `message`: A short message describing the event.
            - `data`: Additional data to log (optional).

4. **Sending State Data**
    - **`Log::update()`**
        - Sends the current state data to the server.

### Example

```cpp
#include <Dezibot.h>
#include <../src/log/Log.h>

const char* ssid = "hotspot";
const char* password = "password";
String ipAdress = "192.168.1.100";

Dezibot dezibot = Dezibot();

void setup() {
    Serial.begin(115200);
    Log::begin(ssid, password, "http://" + ipAdress + ":5160/api/dezibot/update");
    dezibot.begin();
}

void loop() {
    Log::d(DEBUGLOG, MAIN_PROGRAM, "Debug log from main");
    Log::d(INFOLOG, MAIN_PROGRAM, "Info log from main");
    Log::d(WARNLOG, MAIN_PROGRAM, "Warnings log from main");
    Log::d(ERRORLOG, MAIN_PROGRAM, "Error log from main");

    Log::propertyChanged(MAIN_PROGRAM, "someProperty", "someValue");

    Log::update();
    delay(3000);
}
```

This example demonstrates how to initialize the `Log` class, log messages, log property changes, and send state data to the server.

## Backend API

...

### Endpoints

The backend provides the following endpoints:

- `/api` - A UI to view the open API documentation of the API and test the endpoints
- `GET /api/dezibot` - Get all Dezibots
- `GET /api/dezibot/[ip]` - Get a Dezibot by IP address
- `WS /api/dezibot-hub` - WebSocket for the frontend to receive data
- `PUT /api/dezibot/update` - Receives state data or logs from Dezibot classes

### Session Handling

TODO: Describe the session handling mechanism, how it ensures multiple clients can view different Dezibot sessions, and 
any relevant configuration or usage details.

### Example Requests for `PUT /api/dezibot/update`

#### State Data

```json
{
  "Ip": "111.222.333.444",
  "Data": {
    "DISPLAY": {
      "isFlipped": "true",
      "currentLine": "12",
      "lastText": "Hello World"
    }
  }
}
```

#### Log Data

```json
{
  "Ip": "111.222.333.444",
  "logLevel": "INFO",
  "className": "DISPLAY",
  "message": "My first message",
  "data": "Some data"
}
```

## Frontend

...

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
TODO: Create LICENSE file
