<template>
  <NuxtLayout>
    <NuxtPage/>
  </NuxtLayout>
</template>

<script setup lang="ts">
import type { Component, Dezibot } from './types/Dezibot'
import { useIntervalFn } from '@vueuse/core'

const { data, error, refresh } = await useFetch<Dezibot[]>('/api/bots')
useIntervalFn(async () => {
  refresh()
}, 300);

</script>
