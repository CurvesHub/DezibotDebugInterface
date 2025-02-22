<template>
    <transition
        enter-active-class="transition-opacity duration-300"
        enter-from-class="opacity-0"
        enter-to-class="opacity-100"
        leave-active-class="transition-opacity duration-300"
        leave-from-class="opacity-100"
        leave-to-class="opacity-0"
    >
        <UAlert 
            v-if="visible"
            :title="title"
            :color="props.color ?? 'white'"
            :variant="props.variant ?? 'solid'"
            :close-button="{ icon: 'i-heroicons-x-mark-20-solid', color: `${props.color ?? 'gray'}`, variant: 'link', padded: false }"
            @close="closeButtonClicked()"
            class="fixed bottom-8 left-1/2 transform -translate-x-1/2 max-w-[50vw]"
        >
            <template #description>
                {{ description }}
                <UProgress 
                    :value="100*timeLeft/duration"
                    class="-mb-2 mt-2"
                    :color="props.color ?? 'primary'"
                />
            </template>
        </UAlert>
    </transition>
</template>

<script setup lang="ts">
const visible = ref(false)
const title = ref("")
const description = ref("")
const duration = ref(0)

const props = defineProps<{
    color?: string
    variant?: string
}>()

let timer : any = null
const timeLeft = ref(1)
const isRunning = ref(false)

const showSnackbar = (_title: string, _description: string, _duration = 3000) => {
    title.value = _title
    description.value = _description
    visible.value = true
    duration.value = _duration
    startTimer(_duration)

    setTimeout(() => {
        visible.value = false
    }, _duration)
}


function startTimer(duration: number) {
    if (isRunning.value) return
    isRunning.value = true
    timeLeft.value = duration

    timer = setInterval(() => {
        if (timeLeft.value > 0) {
            timeLeft.value -= 10
        } else {
            clearInterval(timer)
            isRunning.value = false
        }
    }, 10)
}

function closeButtonClicked() {
    visible.value = false
    isRunning.value = false
    clearInterval(timer)
}

// Cleanup when component is unmounted
onUnmounted(() => {
    if (timer) clearInterval(timer)
})

defineExpose({ showSnackbar })
</script>