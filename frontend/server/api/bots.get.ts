import { Dezibot } from "~/types/Dezibot"

export default defineEventHandler(async (event) => {
  const server = process.env.BACKEND_URL || "http://localhost:5160"
  const data: Dezibot[] = await $fetch(`${server}/api/dezibots`)
  return data
})
