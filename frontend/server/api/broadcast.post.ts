import { addOrUpdateBotProp } from "~/services/botmanager"

export default defineEventHandler(async (event): Promise<void> => {
  const body = await readBody(event)
  if (body["event"] == "propertyChanged") {
    addOrUpdateBotProp(body["ip"], body["class"], body["propName"] ?? "undefined", body["value"])
  }
})
