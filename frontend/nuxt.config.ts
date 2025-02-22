// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: '2024-04-03',
  devtools: { enabled: true },
  modules: ['@vueuse/nuxt', '@nuxt/ui', '@nuxtjs/i18n'],
  colorMode: {
    preference: 'light'
  },
  ssr: false,
  i18n: {
    vueI18n: './i18n.config.ts' // if you are using custom path, default
  }
})