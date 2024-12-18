<template>
<div class="flex flex-row">
    <UCard v-if="bots.length == 0" class="m-6">
        Start a Dezibot with logging enabled to see it here.
    </UCard>
    <div v-else v-for="bot in bots" class="flex flex-row m-4">
        <BotCard :bot="bot"/>
    </div>
</div>
</template>

<script setup lang="ts">
import * as signalR from '@microsoft/signalr'
import { Dezibot } from '~/types/Dezibot'

const bots = ref<Dezibot[]>([])

onMounted(async () => {
    let connection = new signalR.HubConnectionBuilder()
    .withUrl("/dezibot-hub")
    .build()

    connection.on("SendDezibotUpdateAsync", data => {
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
    } catch (err) {
        console.error("Could not start SignalR connection", err)
    }

    await fetchInitialData()
})

async function fetchInitialData() {
    const { data } = await useFetch("/api/bots")
    console.log(data.value)

    bots.value = data.value?.map(e => Dezibot.fromJson(e)) ?? []
}
</script>