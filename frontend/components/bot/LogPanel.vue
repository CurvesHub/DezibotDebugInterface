<template>
    <UCard class="min-w-[40rem] ml-2 bg-slate-50">
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
                    class="max-h-8"
                />
                <UInput
                    icon="i-heroicons-magnifying-glass-20-solid"
                    size="sm"
                    color="white"
                    :trailing="false"
                    :placeholder="$t('search_placeholder')"
                    class="ml-4 min-w-56 max-h-8"
                    v-model="searchQuery"
                />

                <div class="flex-wrap">
                    <UButton 
                        v-for="(value, key) in logLevels" 
                        class="ml-2 mb-2 min-w-14 justify-center" :color="value.color" 
                        :variant="value.selected ? 'solid' : 'outline'" 
                        :label="value.name" 
                        @click="value.selected = !value.selected"
                        :ui="{ rounded: 'rounded-full' }"    
                    />
                </div>
            </div>
        </template>
        <div class="max-h-[40rem] overflow-x-auto overflow-y-auto">
            <div v-for="log in logs" class="flex flex-row min-w-[40rem] w-auto">
                <div class="mr-2 text-nowrap" :class="getColor(log.level)">{{ log.timestampUtc }}</div>
                <div class="mr-2 text-nowrap" :class="getColor(log.level)">{{ log.level }}:</div>
                <div class="mr-2 text-nowrap">{{ log.className }}:</div>
                <div class="mr-2 text-nowrap">{{ log.message }}</div>
                <div class="mr-2 text-nowrap">{{ log.data ? `| ${log.data}` : '' }}</div>
            </div>
        </div>
    </UCard>
</template>

<script setup lang="ts">
import { Dezibot } from '~/types/Dezibot'
const { t } = useI18n()

const props = defineProps({
    bot: { type: Dezibot, required: true },
})

const emit = defineEmits(["hideLogsClick"])

const searchQuery = ref("")

const logs = computed(() => {
    return props.bot.logs
        .filter((l) => {
            return getSelectedLogLevels(Object.values(logLevels.value)).includes(l.level.toLowerCase())
        })
        .filter((l) => {
            if (searchQuery.value.length <= 0) {
                return true
            } else {
                const combinedLogEntry = l.message.concat(l.className).concat(l.timestampUtc).concat(l.data).toLowerCase()
                return combinedLogEntry.includes(searchQuery.value.toLowerCase())
            }
        })
})

const logLevels = ref({
    info: {
        name: t("log_level_info"),
        selected: true,
        color: "sky",
        id: "info"
    },
    warn: {
        name: t("log_level_warn"),
        selected: true,
        color: "yellow",
        id: "warn"
    },
    error: {
        name: t("log_level_error"),
        selected: true,
        color: "red",
        id: "error"
    },
    debug: {
        name: t("log_level_debug"),
        selected: true,
        color: "green",
        id: "debug"
    }
})

function getSelectedLogLevels(levels: {selected: boolean, id: string}[]) {
    return levels.filter(l => l.selected).map((level) => level.id)
}

function getColor(level: string): string {
    switch (level.toLowerCase()) {
        case 'info':
            return 'text-sky-500';
        case 'warn':
            return 'text-yellow-500';
        case 'error':
            return 'text-red-500';
        case 'debug':
            return 'text-green-500';
        default:
            return 'text-black';
    }
}

</script>