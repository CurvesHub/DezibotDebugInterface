<template>
    <div style="display: flex;">
        <div v-for="bot in data" style="background-color: antiquewhite; min-width: 20%; max-width: 20%;margin: 10px" :key="bot.ip">
            Bot: {{ bot.ip }}
            <div v-for="(comp, compname, index) in bot.components" style="background-color: aquamarine; padding: 5px; margin: 10px;">
            {{ compname }}
            <br>
            <div v-for="(value, prop, index) in comp.properties" style="margin-left: 5px;">
                {{ prop }}: {{ value }}
            </div>
            </div>
            Logs:
            <div v-for="(comp, compname, index) in bot.components" style="margin: 10px;">
            <div v-if="comp.messages.length > 0">
                {{ compname }}
                <div v-for="[mess, value] in comp.messages.slice(-5)" style="margin-left: 5px;">
                {{ mess }}: {{ value }}
                </div>
            </div>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
    import type { Dezibot } from './types/Dezibot'
    import { useIntervalFn } from '@vueuse/core'

    const { data, error, refresh } = await useFetch<Dezibot[]>('/api/bots')
    useIntervalFn(async () => {
    refresh()
    }, 300);

</script>
