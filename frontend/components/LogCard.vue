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
                <div v-for="[message, value, name] in accumulateLogs(bot)" class="flex flex-row">
                    <div class="mr-2">{{ name }}:</div>
                    <div class="mr-2">{{ message }}</div>
                    <div>{{ value }}</div>
                </div>
            </div>
        </UCard>
</template>

<script setup lang="ts">
import { Dezibot } from '~/types/Dezibot';

const props = defineProps({
  bot: { type: Dezibot, required: true },
})
const emit = defineEmits(["hideLogsClick"])

onUpdated(() => {
    // TODO not working
    useTemplateRef("logsContainer").value?.scrollIntoView()
})

// returns tuples of message, data and component name
// TODO: needs to be changed once we collect logs outside of components in realtime
function accumulateLogs(bot: Dezibot): any[] {
    const comps = bot.components.values()
    return comps
        .flatMap((comp) => comp.messages.map((mess) => {
            mess.push(getCompName(comp.name))
            return mess
        })).toArray()
}



</script>