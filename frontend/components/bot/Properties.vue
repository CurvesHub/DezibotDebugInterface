<template>
    <UTable :rows="properties"/>
</template>

<script setup lang="ts">
import { Component } from '~/types/Dezibot';

const props = defineProps({
  component: { type: Component, required: true },
})

const properties = computed(() => {
    return props.component.properties.map((prop) => {
        return {
            Property: prop.name, 
            Value: prop.values.slice(-1)[0].value
        }
    })
})

</script>

