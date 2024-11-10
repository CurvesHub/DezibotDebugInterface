import { Dezibot, Component } from "~/types/Dezibot"
export const bots: Dezibot[] = []

export const addOrUpdateBotProp = (ip: string, comp: string, property: string, value: string) => {
    const foundBot = bots.find((bot) => bot.ip == ip)
    if(foundBot) {
        const foundComponent = foundBot.components.get(comp)
        if (foundComponent) {
            foundComponent.properties.set(property, value) 
        }else {
            const newComp = new Component(comp)
            newComp.properties.set(property, value)
            foundBot.components.set(comp, newComp)
        }
    } else {
        const newBot = new Dezibot(ip)
        const newComp = new Component(comp)
        newComp.properties.set(property, value)
        newBot.components.set(comp, newComp)
        bots.push(newBot)
    }
}

export const getBots = () => bots
