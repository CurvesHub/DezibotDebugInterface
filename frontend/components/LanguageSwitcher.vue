<template>
    <div >
        <USelect v-model="selectedLocale" :options="locales" option-attribute="name"/>
    </div>
</template>
    
<script setup lang="ts">
const { setLocale, locale, t } = useI18n()
const selectedLocale = ref(locale)
const locales = computed(() => [{
    name: t("language_german"),
    value: "de",
    disabled: locale.value == "de" 
}, {
    name: t("language_english"),
    value: "en",
    disabled: locale.value == "en" 
}])
watch(selectedLocale, async (newLocale) => await setLocale(newLocale))
</script>
    