import { Session } from "~/types/Session"

export default defineEventHandler(async (event) => {
    const server = useRuntimeConfig().internalServerUrl
    const data : {id: number, createdUtc: string, name: string}[] = await $fetch(`${server}/api/sessions/available`)
    return data.map((e) => {
        let name = ""
        if (e.name.trim().length > 0) {
            name = e.name
        } else {
            name = e.createdUtc
        }
        return {id: e.id, timestampUtc: e.createdUtc, name: name}
    }) as Session[]
})
  