# Endpoints

## Resources

### Dezibot

- `GET /api/dezibot` - Get all Dezibots
- `GET /api/dezibot/[serialNumber]` - Get a Dezibot by serial number
- `POST /api/dezibot` - Create a new Dezibot
- `POST /api/dezibot/broadcast` - Receive data from Dezibots
- `POST /api/dezibot/command` - Send a command to all Dezibots
- `POST /api/dezibot/command/[serialNumber]` - Send a command to a specific Dezibot

### Sensor

- `GET /api/sensor` - Get all sensors
- `GET /api/sensor/[serialNumber]` - Get all sensors for a specific Dezibot

### Command

- `GET /api/command` - Get all commands
- `GET /api/command/[serialNumber]` - Get all commands for a specific Dezibot

## Requests and Responses

### HTTP Status Codes
- 200: OK
- 201: Created
- 400: Bad Request
- 404: Not Found
- 500: Internal Server Error

### Dezibot model

```json
[
    {
        "serialNumber": "string",   // Unique identifier
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