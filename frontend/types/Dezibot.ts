class Dezibot {
    ip: string
    components: Map<string, Component> = new Map(
        //[
        //["LIGHT_DETECT", new Component("LIGHT_DETECT")],
        //["DISPLAY", new Component("DISPLAY")]
        // [...]
        //]
    )
    battery: number = 1.0

    constructor(ip: string) {
        this.ip = ip
    }

    toJSON() {
        return {
          ip: this.ip,
          components: Object.fromEntries(this.components) // Convert Map to object
        }
    }
}

class Component {
    name: string
    properties: Map<string, string> = new Map()
    messages: [string, string][] = [] // message and data
    
    constructor(name: string) {
        this.name = name
    }

    toJSON() {
        return {
          name: this.name,
          properties: Object.fromEntries(this.properties),
          messages: this.messages
        }
    }
}

export {Dezibot, Component}