<template>
    <UTable :rows="properties" v-model="selected" by="Property" @select="select" @select:all="b => properties.forEach(e => select(e))" class="min-w-[30rem]" />
</template>

<script setup lang="ts">
import { Component } from '~/types/Dezibot'

const props = defineProps({
    component: { type: Component, required: true },
})

const emit = defineEmits(["propsSelected"])

const selected = ref<{Property: string, Value: string}[]>([])

const properties = computed(() => {
    return props.component.properties.map((prop) => {
        return {
            temp: false, // this does nothing
            Property: prop.name, 
            Value: prop.values.slice(-1)[0].value,
        }
    })
})

function select(row: any) {
    if (isNaN(Number(row.Value))) return

    const index = selected.value.findIndex(item => item.Property === row.Property)
    if (index === -1) {
        selected.value.push(row)
    } else {
        selected.value.splice(index, 1)
    }
    emit("propsSelected", selected.value.map(e => e.Property))
}

</script>

