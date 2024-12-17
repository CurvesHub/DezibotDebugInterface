class Dezibot {
    ip: string
    components: Component[] = []
    logs: LogEntry[] = [] // message and data
    battery: number = 1.0

    constructor(ip: string) {
        this.ip = ip
    }

    static fromJson(json: any): Dezibot {
        console.log(json);

        json.components = json.classes
        json.classes = undefined
        json.battery = 0.14
        json.logs = json.logs.map((l: any) => {l.level = "info"; return l}) // TODO remove when backend is ready
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
    level: string

    constructor(
        className: string,
        message: string,
        data: string,
        timestampUtc: string,
        level: string
    ) {
        this.className = className
        this.data = data
        this.message = message
        this.timestampUtc = timestampUtc
        this.level = level
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

export {Dezibot, Component, LogEntry, Property}