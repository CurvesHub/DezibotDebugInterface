<template>
<UCard class="min-w-96  ml-2 bg-slate-50">
            <template #header>
                <div class="text-xl font-bold">
                    <UButton
                    icon="i-heroicons-arrow-left-circle"
                    size="sm"
                    color="white"
                    variant="solid"
                    label=""
                    :trailing="false"
                    @click="emit('hideLogsClick')"
                    />
                </div>
            </template>
            <div class="max-h-[40rem] overflow-y-auto" ref="logsContainer">
                <div v-for="log in bot.logs" class="flex flex-row">
                    <div class="mr-2">{{ log.timestampUtc }}</div>
                    <div class="mr-2">{{ log.className }}:</div>
                    <div class="mr-2">{{ log.message }}</div>
                    <div>{{ log.data }}</div>
                </div>
            </div>
        </UCard>
</template>

<script setup lang="ts">
import { Dezibot, LogEntry } from '~/types/Dezibot';

const props = defineProps({
  bot: { type: Dezibot, required: true },
})
const emit = defineEmits(["hideLogsClick"])

onUpdated(() => {
    // TODO not working
    useTemplateRef("logsContainer").value?.scrollIntoView()
})

function sortEntries(): LogEntry[] {
    return props.bot.logs
}



</script>