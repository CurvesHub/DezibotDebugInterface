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

### PUT Dezibot model broadcast

```json
    {
        "ip": "string",             // Unique identifier
        "components": [             // List of components
            {
                "name": "string",   // Unique identifier (like light sensor)
                "values": [         // List of values
                    {
                        "name": "string",     // Value name
                        "value": "string",    // Value
                    }
                ]
            }
        ]
    }
```

### GET Dezibots


```json
[
    {
        "ip": "string",             // Unique identifier
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
                ]
            }
        ]
    }
]
```
