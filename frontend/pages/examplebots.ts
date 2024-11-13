import { Component, Dezibot } from '~/types/Dezibot';

const exampleBot = new Dezibot("123.456.789.12")
const display = new Component("DISPLAY")
display.properties.set("text", "Hello Dezibot")
display.properties.set("currentLine", "1")
display.properties.set("charsOnCurreLine", "16")
display.messages.push(["init display", ""])
display.messages.push(["read register", "12:40"])
display.messages.push(["written register", "1:12"])
display.messages.push(["read register", "12:40"])
display.messages.push(["written register", "1:12"])
display.messages.push(["written register", "1:12"])
display.messages.push(["read register", "12:40"])

const lightDetect = new Component("LIGHT_DETECT")
lightDetect.properties.set("irpt1", "1234")
lightDetect.properties.set("irpt2", "2446745")
lightDetect.properties.set("dlpt1", "274534")
lightDetect.messages.push(["init light detection", ""])
lightDetect.messages.push(["read register", "12:40"])
lightDetect.messages.push(["read register", "12:40"])
lightDetect.messages.push(["written register", "1:12"])
lightDetect.messages.push(["read register", "12:40"])
lightDetect.messages.push(["written register", "1:12"])
lightDetect.messages.push(["read register", "12:40"])
lightDetect.messages.push(["written register", "1:12"])
exampleBot.components.set("LIGHT_DETECT", lightDetect)
exampleBot.components.set("DISPLAY", display)

exampleBot.battery = 0.73

export const bots = [exampleBot, exampleBot]