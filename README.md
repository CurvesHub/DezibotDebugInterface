# DeziBot Debug Interface

## Goal
Display senor data of the DeziBot in a Web UI to assist debugging.

## Features

- A Dezibot program to collect senor data and broadcast it to a web sever. The program should be callable by or runs parallel to the executing program, but is independent.
- A back-end sever to receive, organzise and save the sensor data of various bots. It provides different endpoints for the front-end.
- A front-end server which provides a web GUI displaying the senor data of the bots as well as an status.

## Roadmap

**MVP:**
- [ ] Read all senor data
- [ ] Send data to an back-end endpoint (as json objects or data stream)
- [ ] Save sensor data with timestamps and provide a GET back-end endpoint for the front-end
- [ ] Display common sensor data of two or more bots in a grid and provide a details view


**MVP V2:**
- [ ] The bot can receive a debug command
- [ ] Send a commad to the bot (e.g. set LEDs to Blue)