<template>
<div class="text-2xl font-bold mt-4 ml-4">{{ sessionData?.name }}</div>
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
import type { Session } from '~/types/Session'

const route = useRoute()
const bots = ref<Dezibot[]>([])
let connection: signalR.HubConnection
const { data: sessionData } = await useFetch<Session>(`/api/session/${route.params.id}`)
onMounted(async () => {
  connection = new signalR.HubConnectionBuilder()
    .withUrl("/dezibot-hub")
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

onBeforeUnmount(async () => {
    if (connection) {
        await connection.stop()
    }
})
</script>