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
                @click="emit('hideGraphClick')"
                />
            </div>
        </template>
        <div class="overflow-y-auto">
            <Line v-for="prop in data" :data="prop" :options="chartOptions" class="min-w-64 min-h-64 max-h-[20rem]"/>
        </div>
    </UCard>
</template>

<script setup lang="ts">
import { Property } from '~/types/Dezibot'
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
  type ChartOptions,
  type ChartData
} from 'chart.js'
import { Line } from 'vue-chartjs'

ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend
)

const chartOptions: ChartOptions = {
    responsive: true,
    maintainAspectRatio: false,
}

const props = defineProps({
    props: { type: Array<Property>, required: true },
})

const data: ComputedRef<ChartData[]> = computed(() => {
    return props.props.map((prop) => {
        return {
            labels: prop.values.slice(-80).map(e => prop.values.indexOf(e)),
            datasets: [
                { data: prop.values.slice(-80).map(e => +e.value), label: prop.name, backgroundColor: "green"}
            ],
        }
    })
})

const emit = defineEmits(["hideGraphClick"])
</script>