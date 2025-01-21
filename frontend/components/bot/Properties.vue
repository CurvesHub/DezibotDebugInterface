<template>
    <UTable 
    :rows="properties" 
    :columns="columns" 
    @select="select" 
    v-model="selected" 
    by="propName" 
    class="min-w-[30rem]" 
/>
</template>

<script setup lang="ts">
import { Component } from '~/types/Dezibot'
const { t } = useI18n()

type Row = {
    propName: string,
    value: string
}
const props = defineProps({
    component: { type: Component, required: true },
})

const emit = defineEmits(["propsSelected"])

const selected = ref<Row[]>([])

const columns = [
    {
        key: "propName",
        label: t("property_table_property")
    } , {
        key: "value",
        label: t("property_table_value")
    }
]

const properties = computed(() => {
    return props.component.properties.map((prop) => {
        return {
            propName: prop.name,
            value: prop.values.slice(-1)[0].value
        } as Row
    })
})

watch(() => selected.value, (newValue, oldValue) => {
    emit("propsSelected", newValue.map(e => e.propName))
}, {deep: true})

function select(row: Row) {
    if (isNaN(Number(row.value))) return 
    const index = selected.value.findIndex(item => item.propName === row.propName)
    if (index === -1) {
        selected.value.push(row)
    } else {
        selected.value.splice(index, 1)
    }
}

</script>

