export default defineEventHandler(async (event) => {
    const server = useRuntimeConfig().internalServerUrl
    const sessionId = getRouterParam(event, "sessionid")
    const botIp = getRouterParam(event, "botip")
    const data = await $fetch(`${server}/api/session/${sessionId}/dezibot/${botIp}`, {method: "delete"} as object)
    return data
})
  