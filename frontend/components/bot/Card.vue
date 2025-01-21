<template>
    <div class="flex flex-row m-4">
        <UCard class="min-w-[30rem]">
            <template #header>
                <div class="flex flex-row justify-between items-center">
                    <div class="text-xl font-bold">
                        {{ bot.ip }}
                    </div>
                    <div class="flex flex-row">
                        <UButton
                            icon="i-heroicons-document-text"
                            size="sm"
                            :color="isLogsOpen ? 'primary' : 'white'"
                            variant="solid"
                            label="Logs"
                            :trailing="false"
                            @click="isLogsOpen = !isLogsOpen"
                        />

                        <UButton
                            icon="i-heroicons-arrow-trending-up"
                            size="sm"
                            :color="isGraphsOpen? 'primary' : 'white'"
                            variant="solid"
                            label="Graphs"
                            :trailing="false"
                            @click="isGraphsOpen = !isGraphsOpen"
                            class="ml-2"
                        />
                    </div>
                </div>
            </template>
            <UAccordion :items="items" color="gray" variant="solid" multiple>
                <template #item="{item}">
                    <BotProperties :component="JSON.parse(item.comp)" @propsSelected="(propNames : string[]) => propsSelected(JSON.parse(item.comp), propNames)"/>
                </template>

            </UAccordion>

            <template #footer>
                <UProgress :value="bot.battery*100" indicator class="h-10" :color="getBotBatColor()"/>
            </template>
        </UCard>
        
        <BotLogPanel :bot="bot" v-if="isLogsOpen" @hide-logs-click="isLogsOpen = !isLogsOpen"/>

        <BotGraph :props="propsState" v-if="isGraphsOpen" @hide-graph-click="isGraphsOpen = !isGraphsOpen"/>
    </div>
</template>

<script setup lang="ts">
import { Component, Dezibot, Property } from '~/types/Dezibot';

const props = defineProps({
    bot: { type: Dezibot, required: true },
})

const items = computed(() => {
    return props.bot.components.map(e => { return {label: getCompName(e.name), comp: JSON.stringify(e)} })
})

const isLogsOpen = ref(false)
const isGraphsOpen = ref(false)
const selectedProperties = ref<Map<string, Property[]>>(new Map()) // map of compname to selected properties
const propsState = computed(() => {
    const botProps = props.bot.components.flatMap((c) => c.properties)
    const result: Property[] = []
    for (const [compname, properties] of selectedProperties.value) {
        result.push(...botProps.filter((p) => properties.map((e : Property) => e.name).includes(p.name)))
    }
    return result
})


function propsSelected(comp: Component, propNames: string[]) {
    const filtered = comp.properties.filter(e => propNames.includes(e.name))
    const copy = selectedProperties.value
    copy.set(comp.name, filtered)
    selectedProperties.value = copy
    const selectedPropertiesCount = Array.from(selectedProperties.value.values()).reduce((sum, arr) => sum+arr.length, 0)
    if (selectedPropertiesCount > 0) {
        isGraphsOpen.value = true
    } else {
        isGraphsOpen.value = false
    }
}

function getBotBatColor(): string {
    switch(true) {
        case props.bot.battery < 0.1: return "red"
        case props.bot.battery < 0.3: return "orange"
        default: return "green"
    }
}
</script>