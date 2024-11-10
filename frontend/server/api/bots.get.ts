import { bots } from "~/services/botmanager"

export default defineEventHandler(async (event) => {
  return bots
})
