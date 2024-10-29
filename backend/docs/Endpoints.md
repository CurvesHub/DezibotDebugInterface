# Endpoints

## Resources

### Dezibot

- `GET /api/dezibot` - Get all Dezibots
- `GET /api/dezibot/[ip]` - Get a Dezibot by ip adress


- `PUT /api/dezibot/broadcast` - Receive data from Dezibots (Websocket)

## Requests and Responses

### HTTP Status Codes
- 200: OK
- 400: Bad Request
- 404: Not Found
- 500: Internal Server Error

### Dezibot model

```json
[
    {
        "ip": "string",   // Unique identifier
        "status": "string",         // online or offline
        "lastConnection": "string", // Last connection timestamp
        "components": [             // List of components
            {
                "id": "string",     // Unique identifier
                "name": "string",   // Component name
                "type": "string",   // Component type (sensor, light, motor, etc.)
                "status": "string", // Component status (on, off, etc.)
                "values": [         // List of values
                    {
                        "name": "string",         // Value name
                        "lastValue": "string",    // Value
                        "overTime": [             // List of values over time
                            {
                                "timestamp": "string",  // Timestamp
                                "value": "string"       // Value
                            }
                        ]
                    }
                ],
                "commands": [       // List of commands
                    {
                        "name": "string",   // Command name
                        "value": "string"   // Command value
                    }
                ]
            }
        ]
    }
]
```
