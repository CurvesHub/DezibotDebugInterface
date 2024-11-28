class Dezibot {
    ip: string
    components: Component[] = []
    logs: LogEntry[] = [] // message and data
    battery: number = 1.0

    constructor(ip: string) {
        this.ip = ip
    }

    static fromJson(json: any): Dezibot {
        console.log(json)
        json.components = json.classes
        json.classes = undefined
        return json
    }
}

class Component {
    name: string
    properties: Property[] = []
    
    constructor(name: string) {
        this.name = name
    }
}

class Property {
    name: string
    values: {timestampUtc: string, value: string}[] = []

    constructor(name: string) {
        this.name = name
    }
}

class LogEntry { 
    className: string
    message: string
    data: string
    timestampUtc: string

    constructor(
        className: string,
        message: string,
        data: string,
        timestampUtc: string
    ) {
        this.className = className
        this.data = data
        this.message = message
        this.timestampUtc = timestampUtc
    }
}

type ApiDezibot = {
    ip: string
    logs: any[]
    classes: ApiComponent[]
}

type ApiComponent = {
    name: string
    properties: ApiProperty[]
}

type ApiProperty = {
    name: string
    values: {timestampUtc: string, value: string}[]
}

export {Dezibot, Component, LogEntry}