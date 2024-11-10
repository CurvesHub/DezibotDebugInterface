<template>
  <NuxtLayout>
    <div>
      <div v-for="bot in data" style="background-color: antiquewhite; max-width: 50%;">
        {{ bot.ip }}
        <div v-for="comp in bot.components" style="background-color: antiquewhite;">
          component: {{ comp }}
        </div>
      </div>
    </div>
  </NuxtLayout>
</template>

<script setup lang="ts">
import {Dezibot} from "~/types/Dezibot"

const {data} = await useFetch('/api/bots')

</script>
