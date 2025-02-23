export default defineEventHandler(async (event) => {
    const server = process.env.BACKEND_URL || "http://localhost:5160"
    const sessionId = getRouterParam(event, "sessionid")
    const botIp = getRouterParam(event, "botip")
    const data = await $fetch(`${server}/api/session/${sessionId}/dezibot/${botIp}`, {method: "delete"} as object)
    return data
})
  