<template>
    <div class="flex flex-row m-4">
        <UCard class="min-w-96">
            <template #header>
                <div class="flex flex-row justify-between items-center">
                    <div class="text-xl font-bold">
                        {{ bot.ip }}
                    </div>
                    <div>
                        <UButton
                            icon="i-heroicons-document-text"
                            size="sm"
                            color="white"
                            variant="solid"
                            label="Logs"
                            :trailing="false"
                            @click="isLogsOpen = true"
                        />
                    </div>
                </div>
            </template>

            <div v-for="comp in bot.components" class="mt-3">
                {{ getCompName(comp.name) }}
                <BotProperties :component="comp"/>
            </div>

            <template #footer>
                <UProgress :value="bot.battery*100" indicator class="" :color="getBotBatColor()"/>
            </template>
        </UCard>
        
        <BotLogPanel :bot="bot" v-if="isLogsOpen" @hide-logs-click="console.log(bot); isLogsOpen = false"/>
    </div>
</template>

<script setup lang="ts">
import { Dezibot } from '~/types/Dezibot';

const props = defineProps({
    bot: { type: Dezibot, required: true },
})

const isLogsOpen = ref(false)


function getBotBatColor(): string {
    switch(true) {
        case props.bot.battery < 0.1: return "red"
        case props.bot.battery < 0.3: return "orange"
        default: return "green"
    }
}
</script>