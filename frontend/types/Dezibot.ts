class Dezibot {
    ip: string
    components: Component[] = []
    logs: LogEntry[] = [] // message and data

    constructor(ip: string) {
        this.ip = ip
    }

    static fromJson(json: any): Dezibot {

        json.components = json.classes
        delete json.classes        
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
    timestampUtc: string
    level: string
    className: string
    message: string
    data: string

    constructor(
        timestampUtc: string,
        level: string,
        className: string,
        message: string,
        data: string,
    ) {
        this.timestampUtc = timestampUtc
        this.level = level
        this.className = className
        this.message = message
        this.data = data
    }
}

export {Dezibot, Component, LogEntry, Property}