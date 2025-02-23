<template>
<div class="text-2xl font-bold mt-4 ml-4">{{ sessionData?.name }}</div>
<div class="flex flex-row">
    <UCard v-if="bots.length == 0" class="m-6">
        {{ $t("info_empty_bots") }}
    </UCard>
    <div v-else v-for="bot in bots" class="flex flex-row m-4">
        <BotCard :bot="bot" @delete-bot="deleteBot(bot.ip)"/>
    </div>
</div>
<Snackbar ref="snackbarRef" color="red" variant="subtle"/>
</template>

<script setup lang="ts">
import * as signalR from '@microsoft/signalr'
import type { VNodeRef } from 'vue'
import { Dezibot } from '~/types/Dezibot'
import type { Session } from '~/types/Session'

const { t } = useI18n()
const route = useRoute()
const snackbarRef = ref<VNodeRef | null>(null)

const bots = ref<Dezibot[]>([])
let connection: signalR.HubConnection
const { data: sessionData } = await useFetch<Session>(`/api/session/${route.params.id}`)
onMounted(async () => {
    const server = process.env.BACKEND_URL_OUTSIDE_DOCKER || "http://thisdidntexist:5160"
    connection = new signalR.HubConnectionBuilder()
        .withUrl(`${server}/api/dezibot-hub`)
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
        const shouldReceiveUpdatesForSession = route.params.viewmode == "edit"
        await connection.send("JoinSession", parseInt(route.params.id), shouldReceiveUpdatesForSession)
    } catch (err) {
        console.error("Could not start SignalR connection", err)
    }
})

async function deleteBot(botIp: string) {
    const sessionId = route.params.id
    const {error} = await useFetch(`/api/session/${sessionId}/deletebot/${botIp}`, {method: "delete"})
    const statusCode = error.value?.statusCode ?? -1
    
    if (statusCode < 300) {
        const index = bots.value.findIndex(bot => bot.ip == botIp)
        if (index > -1) {
            bots.value.splice(index, 1)
        }
    } else {
        snackbarRef.value?.showSnackbar(t("error"), t("delete_bot_error"), 5000)
    }
}

onBeforeUnmount(async () => {
    if (connection) {
        await connection.stop()
    }
})
</script>