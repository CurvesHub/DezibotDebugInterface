<template>
<!-- Name of the Session -->
<div class="text-2xl font-bold mt-4 ml-4">{{ sessionData?.name }}</div>
<div class="flex flex-row">
    <UCard v-if="bots.length == 0" class="m-6">
        {{ $t("info_empty_bots") }}
    </UCard>

    <!-- For each Dezibot, display a card with its details -->
    <div v-else class="flex flex-row flex-wrap max-w-full">
        <div v-for="bot in bots" class="mb-4">
            <BotCard :bot="bot" @delete-bot="deleteBot(bot.ip)"/>
        </div>
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
const config = useRuntimeConfig()

const snackbarRef = ref<VNodeRef | null>(null)
const bots = ref<Dezibot[]>([])
const { data: sessionData } = await useFetch<Session>(`/api/session/${route.params.id}`)

let connection: signalR.HubConnection
onMounted(async () => {
    await setupSignlaRConnection()
})

/**
 * Updates the data of a bot in the bots array
 * @param data The data of the bot that was updated
 */
function updateDezibotData(data: any) {
    const newState = bots.value
    const bot = newState.find((bot) => bot.ip == data.ip)
    const newBot = Dezibot.fromJson(data)
    if (!bot) {
        newState.push(newBot)
    }
    else {
        newState[newState.indexOf(bot)] = newBot
    }
    
    bots.value = newState
}

/**
 * Sets up the SignalR connection to the backend
 */
async function setupSignlaRConnection() {
    const server = config.public.serverUrl
    
    connection = new signalR.HubConnectionBuilder()
        .withUrl(`${server}/api/dezibot-hub`)
        .build()

    connection.on("DezibotUpdated", updateDezibotData)

    try {
        await connection.start()
        const shouldReceiveUpdatesForSession = route.params.viewmode == "edit"
        await connection.send("JoinSession", parseInt(route.params.id), shouldReceiveUpdatesForSession)
    } catch (err) {
        console.error("Could not start SignalR connection", err)
    }
}

/**
 * Deletes a bot from the session
 * @param botIp The IP of the bot to delete
 */
async function deleteBot(botIp: string) {
    const sessionId = route.params.id
    try {
        await $fetch(`/api/session/${sessionId}/deletebot/${botIp}`, {method: "delete"}) 
        const index = bots.value.findIndex(bot => bot.ip == botIp)
        if (index > -1) {
            bots.value.splice(index, 1)
        }
    } catch (error) {
        snackbarRef.value?.showSnackbar(t("error"), t("delete_bot_error"), 5000)
    }
}

onBeforeUnmount(async () => {
    if (connection) {
        await connection.stop()
    }
})
</script>