import { Session } from "~/types/Session"

export default defineEventHandler(async (event) => {
    const server = process.env.BACKEND_URL || "http://localhost:5160"
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
  