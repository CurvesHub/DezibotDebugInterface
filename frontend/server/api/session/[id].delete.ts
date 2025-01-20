export default defineEventHandler(async (event) => {
    const server = process.env.BACKEND_URL || "http://localhost:5160"
    const id = getRouterParam(event, "id")
    const data = await $fetch(`${server}/api/session/${id}`, {method: "delete"} as object)
    return data
})
  