import { Session } from "~/types/Session";

export default defineEventHandler(async (event) => {
    const server = useRuntimeConfig().internalServerUrl
    const body = await readBody(event)
    const data : {id: number, createdUtc: string, name: string} = await $fetch(`${server}/api/session`, {method: "post", body: {name: body.name}} as object)
    return {id: data.id, timestampUtc: data.createdUtc, name: data.name} as Session
})
  