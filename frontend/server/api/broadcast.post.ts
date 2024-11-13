import { addLogToBot, addOrUpdateBotProp } from "~/services/botmanager"

export default defineEventHandler(async (event): Promise<void> => {
  const body = await readBody(event)
  if (body["event"] == "propertyChanged") {
    addOrUpdateBotProp(body["ip"], body["class"], body["propName"] ?? "undefined", body["value"])
    addOrUpdateBotProp(body["ip"] + ".test", body["class"], body["propName"] ?? "undefined", body["value"])
  } else if (body["event"] == "message") {
    addLogToBot(body["ip"], body["class"], body["message"] ?? "undefined", body["value"])
    addLogToBot(body["ip"] + ".test", body["class"], body["message"] ?? "undefined", body["value"])
  }
})
