export default defineEventHandler(async (event) => {
    const server = useRuntimeConfig().internalServerUrl
    const id = getRouterParam(event, "id")
    const data = await $fetch(`${server}/api/session/${id}`, {method: "delete"} as object)
    return data
})
  