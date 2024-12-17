import { Dezibot } from "~/types/Dezibot"

export default defineEventHandler(async (event) => {
  const data: Dezibot[] = await $fetch("http://localhost:5160/api/dezibots")
  return data
})
