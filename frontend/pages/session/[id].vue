<template>
<div class="text-2xl font-bold">{{ route.params.id }}</div>
<div class="flex flex-row">
    <UCard v-if="bots.length == 0" class="m-6">
        {{ $t("info_empty_bots") }}
    </UCard>
    <div v-else v-for="bot in bots" class="flex flex-row m-4">
        <BotCard :bot="bot"/>
    </div>
</div>
</template>

<script setup lang="ts">
import * as signalR from '@microsoft/signalr'
import { Dezibot } from '~/types/Dezibot'
const route = useRoute()
const bots = ref<Dezibot[]>([])

onMounted(async () => {
  const server = "http://host.docker.internal:8080"//"http://dezibotdebuginterface.api:8080" //process.env.BACKEND_URL || "http://localhost:5160"
  console.log("Backend URL:", server)
  const url = `${server}/api/dezibot-hub`
    let connection = new signalR.HubConnectionBuilder()
    .withUrl(url)
    .build()

    connection.on("DezibotUpdated", data => {
        const newState = bots.value
        const bot = newState.find((bot) => bot.ip == data.ip)
        if (!bot) {
            newState.push(Dezibot.fromJson(data))
        }
        else {
            newState[newState.indexOf(bot)] = Dezibot.fromJson(data)
        }
        
        bots.value = newState
    })

    try {
        await connection.start()
        // TODO implement the continue session logic
        await connection.send("JoinSession", parseInt(route.params.id), true)
    } catch (err) {
        console.error("Could not start SignalR connection", err)
    }
})
</script>