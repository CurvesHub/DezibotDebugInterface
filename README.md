# Dezibot Debug Interface

The Dezibot Debug Interface provides a way to log data from a Dezibot to a backend server and display it in a web 
interface. This repository contains the code for the backend and frontend server. The code for the Dezibot can be 
found [here](https://github.com/CurvesHub/dezibot).

The README is available in german (Deutsch) [here](README_DE.md).

## Table of Contents

- [Dezibot Debug Interface](#dezibot-debug-interface)
  - [Table of Contents](#table-of-contents)
  - [Overview](#overview)
  - [Getting Started](#getting-started)
    - [Attention](#attention)
  - [Dezibot Log Class](#dezibot-log-class)
    - [Methods](#methods)
    - [Example](#example)
  - [Backend API](#backend-api)
    - [Endpoints](#endpoints)
    - [Session Handling](#session-handling)
      - [Multiple Clients](#multiple-clients)
      - [Deleting Sessions](#deleting-sessions)
      - [Example Scenarios](#example-scenarios)
    - [Example Requests for `PUT /api/dezibot/update`](#example-requests-for-put-apidezibotupdate)
      - [State Data](#state-data)
      - [Log Data](#log-data)
  - [License](#license)

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
data in near real-time without interfering with other users.

## Getting Started

All services are dockerized and can be started with docker-compose. Follow the instructions below to start the 
services. This is a step-by-step guide on how to use the Dezibot Debug Interface with the example code provided in the
`log_demo_simple.ino` file. In the following, the host is the machine running the docker stack. Clients are browsers that open the frontend URL.

1. Clone the repository
2. Configure the `docker-compose.yml` file with the desired environment variables
    - `NUXT_PUBLIC_SERVER_URL`: This is the IP adress of the host machine
      - If the only clients run on the host, you can use `http://localhost:5160`
      - If there are other clients that run on different machines, use the IP address of the host machine
3. Prepare Dezibot Code Example
   - Ensure that you have access to a WiFi capable networkso the Dezibots can connect to it
   - Enter the Wifi SSID and password in the `log_demo_simple.ino` file
   - Enter the IP address of the host machine
   - Load the code onto a Dezibot
4. Run `docker-compose up` in the root directory of the project
   - The frontend is available at `http://localhost:3000`
   - The backend is available at `http://localhost:5160`
   - The backend API documentation is available at `http://localhost:5160/api`
   - If the host is a different machine, use the IP adress of the host instead of `localhost` in the URLs above
5. Open the frontend in a browser
6. Create a new session or join an existing one by selecting a session from the dropdown list
   - Either click the `View` or the `Continue` button to join the session
     - `View` will only show the current session without receiving updates
     - `Continue` will show the current session and receive updates if a Dezibot sends data
7. The frontend will display the data of the session with its Dezibots

### Attention

When writing a Dezibot main program, make sure to call `Log::begin(ssid, password, url)` before calling `dezibot.
begin()`. Otherwise, the initial setup logs and debug data will not be sent to the backend server.

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
String ipAdress = "xxx.xxx.xxx.xxx";

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

This example demonstrates how to initialize the `Log` class, log messages, log property changes, and send state data 
to the server. The `Log::update()` method sends the current state data to the server every 3 seconds. But the `Log::d()`
methods will send the log data immediately.

## Backend API

The backend API provides endpoints to manage sessions and retrieve data from Dezibots. It also includes a WebSocket
connection to send real-time updates to the frontend. The API is documented using Scalar UI with the open API 
specification. It can be accessed at `http://localhost:5160/api`.

### Endpoints

The backend provides the following endpoints:

- `/api` - A UI to view the open API documentation of the API and test the endpoints

- `GET /api/session/available` - Returns a list of all available session identifiers
- `GET /api/session` - Returns a list of all sessions with their Dezibots
- `GET /api/session/{sessionId}` - Returns the session with the specified ID
- `GET /api/session/{sessionId}/dezibot/{ip}` - Returns the Dezibot with the specified IP address from the session 
  with the specified ID

- `POST /api/session` - Creates a new session

- `DELETE /api/session` - Deletes all not used sessions
- `DELETE /api/session/{sessionId}` - Deletes the session with the specified ID if it is not used
- `DELETE /api/session/{sessionId}/dezibot/{ip}` - Deletes the Dezibot with the specified IP address from the 
  session with the specified ID

- `WS /api/dezibot-hub` - Signal R WebSocket for the frontend to receive data
  - Provides the following methods/events:
    - `JoinSession` 
      - Invoked by a Client to join a session with the specified ID
      - The Client can specify a `continueSession` parameter to decide wether to receive updates for the session or not
    - `DezibotUpdated`
      - Invoked by the Backend when a Dezibot sends new data or a Client joins a session
      - Sends the latest dezibot to the client(s)
    - `OnDisconnected`
      - Invoked by the Backend when a client disconnects
      - Removes the client from the sessions he was in

- `PUT /api/dezibot/update` - Receives state data or logs from Dezibots and saves it depending on the available session

### Session Handling

The session handling mechanism ensures that multiple clients can view different Dezibot sessions without interfering 
with each other. Each session has a unique identifier that is used to distinguish between different sessions. When a 
client joins a session, it receives updates for that session only. The session handling mechanism also ensures that 
clients are removed from sessions when they disconnect, preventing stale connections from affecting the system.

#### Multiple Clients

- Each client can join a session with a `continueSession` parameter set to `true` or `false` to indicate whether 
  they want to receive updates.
- As long as there is at least one client in a session that wants updates, Dezibot updates are saved. Otherwise, 
  updates are discarded.
- If multiple clients are in a session, only those clients who have opted to receive updates will get them.

#### Deleting Sessions

A session can only be deleted if no clients are currently using it. This ensures that a session cannot be 
accidentally deleted while it is being viewed by a client. Once all clients have left the session, it becomes 
eligible for deletion. The frontend will display an error message if a client tries to delete a session that is in use.

#### Example Scenarios

1. **Single Client Watching a Session:**
    - Client 1 joins a session with `continueSession` set to `false`. They can view the session data but will not 
      receive updates.

2. **Multiple Clients with Different Preferences:**
    - Client 1 joins a session with `continueSession` set to `false`. They can view the session data but will not 
      receive updates.
    - Client 2 joins the same session with `continueSession` set to `true`. The session is updated with new Dezibot 
      data because Client 2 wants updates. Client 1 does not notice any changes.
    - If Client 1 later joins with `continueSession` set to `true`, they will start receiving updates as well.

3. **Client Disconnects:**
    - If a client disconnects, they are removed from the session. If they were the only client receiving updates, 
      updates for that session will stop.
    - A disconnected event is sent to the backend, when either the page is refreshed, closed or the user navigates 
      away from the page back to the selector.

This mechanism ensures that each client can view and interact with Dezibot sessions according to their preferences 
without affecting other clients.

### Example Requests for `PUT /api/dezibot/update`

This example data can be sent to the backend server to update the state data or log data of a Dezibot. Instead of 
running the example code on the Dezibot itself, the data can be sent to the backend server using the Scalar UI or a 
tool like Postman.

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

## License

This project is licensed under the GPL-3.0 license - see the [LICENSE](LICENSE) file for details.
