<template>
<UCard class="min-w-96  ml-2 bg-slate-50">
            <template #header>
                <div class="flex flex-row">
                    <UButton
                    icon="i-heroicons-arrow-left-circle"
                    size="sm"
                    color="white"
                    variant="solid"
                    label=""
                    :trailing="false"
                    @click="emit('hideLogsClick')"
                    class="text-xl font-bold"
                    />
                    <UInput
                        icon="i-heroicons-magnifying-glass-20-solid"
                        size="sm"
                        color="white"
                        :trailing="false"
                        placeholder="Suche..."
                        class="ml-4 min-w-96"
                        v-model="searchQuery"
                    />

                    <UButton 
                        v-for="(value, key) in logLevels" 
                        class="ml-4 min-w-14 justify-center" :color="value.color" 
                        :variant="value.selected ? 'solid' : 'outline'" 
                        :label="value.name" 
                        @click="value.selected = !value.selected"
                        :ui="{ rounded: 'rounded-full' }"    
                    />
                </div>
            </template>
            <div class="max-h-[40rem] overflow-y-auto" ref="logsContainer">
                <div v-for="log in logs" class="flex flex-row">
                    <div class="mr-2">{{ log.timestampUtc }}</div>
                    <div class="mr-2">{{ log.className }}:</div>
                    <div class="mr-2">{{ log.message }}</div>
                    <div class="mr-2">{{ log.data }}</div>
                </div>
            </div>
        </UCard>
</template>

<script setup lang="ts">
import { Dezibot, LogEntry } from '~/types/Dezibot'

const props = defineProps({
  bot: { type: Dezibot, required: true },
})

const emit = defineEmits(["hideLogsClick"])

const searchQuery = ref("")

const logs = computed(() => {
    return props.bot.logs
        .filter((l) => { console.log(l.level);
         return getSelectedLogLevels(Object.values(logLevels.value)).includes(l.level)})
        .filter((l) => {
            if (searchQuery.value.length <= 0) {
                return true
            } else {
                const combinedLogEntry = l.message.concat(l.className).concat(l.timestampUtc).concat(l.data)
                return combinedLogEntry.includes(searchQuery.value)
            }
        })
})

const logLevels = ref({
    info: {
        name: "Info",
        selected: true,
        color: "sky",
        id: "info"
    },
    warn: {
        name: "Warn",
        selected: true,
        color: "yellow",
        id: "warn"
    },
    error: {
        name: "Error",
        selected: true,
        color: "red",
        id: "error"
    }
})

function getSelectedLogLevels(levels: {selected: boolean, id: string}[]) {
    console.log(levels)
    const result = levels.filter(l => l.selected).map((level) => level.id)
    console.log(result)
    return result
}

onUpdated(() => {
    // TODO not working
    useTemplateRef("logsContainer").value?.scrollIntoView()
})

</script>