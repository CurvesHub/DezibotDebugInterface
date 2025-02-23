import { Session } from "~/types/Session"

export default defineEventHandler(async (event) => {
    const server = useRuntimeConfig().internalServerUrl
    const id = getRouterParam(event, "id")
    const data : {id: number, createdUtc: string, name: string}[] = await $fetch(`${server}/api/sessions/available`)
    const session = data.find(session => session.id.toString() == id)
    if (session) {
        return {
            id: session.id,
            timestampUtc: session.createdUtc,
            name: session.name
        } as Session
    } else {
        setResponseStatus(event, 404, "Session Not Found")
    }
})
  