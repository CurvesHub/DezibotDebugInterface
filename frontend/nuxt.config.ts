// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: '2024-04-03',
  devtools: { enabled: true },
  modules: ['@vueuse/nuxt', '@nuxt/ui', '@nuxtjs/i18n'],
  nitro: {
    devProxy: { // TODO: Fix CORS for production
      "/dezibot-hub": {
        target: (process.env.BACKEND_URL || "http://localhost:5160") + "/api/dezibot-hub",
        ws: true, // WebSocket proxying
        changeOrigin: true,
      },
    },
  },
  colorMode: {
    preference: 'light'
  },
  ssr: false,
  i18n: {
    vueI18n: './i18n.config.ts' // if you are using custom path, default
  }
})