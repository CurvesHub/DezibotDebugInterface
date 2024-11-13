<template>
<div class="flex flex-row">
    <div v-for="(bot, i) in bots" class="flex flex-row m-4">
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
                            @click="isLogOpenState[i] = true"
                        />
                    </div>
                </div>
            </template>

            <div v-for="(comp, compname, index) in bot.components.values()" class="mt-3">
                {{ getCompName(comp.name) }}
                <BotStates :component="comp"/>
            </div>

            <template #footer>
                <UProgress :value="bot.battery*100" indicator class="" :color="getBotBatColor(i)"/>
            </template>
        </UCard>
        
        <LogCard :bot="bot" v-if="isLogOpenState[i]" @hide-logs-click="isLogOpenState[i] = false"/>
    </div>
</div>
</template>

<script setup lang="ts">
import { bots } from './examplebots';

const isLogOpenState = ref(bots.map(() => false))

function getBotBatColor(index: number): string {
    const bot = bots[index]
    switch(true) {
        case bot.battery < 0.1: return "red"
        case bot.battery < 0.3: return "orange"
        default: return "green"
    }
}
</script>