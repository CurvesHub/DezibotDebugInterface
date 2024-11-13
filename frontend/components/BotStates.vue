<template>
    <UTable :rows="properties"/>
</template>

<script setup lang="ts">
import { Component } from '~/types/Dezibot';

const props = defineProps({
  component: { type: Component, required: true },
})

const properties = props.component.properties.keys().map((key) => {
    return {
        Property: key, 
        Value: props.component.properties.get(key)
    }
}).toArray()

</script>

