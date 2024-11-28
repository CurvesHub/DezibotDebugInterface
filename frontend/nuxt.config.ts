// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: '2024-04-03',
  devtools: { enabled: true },
  modules: ['@vueuse/nuxt', '@nuxt/ui'],
  nitro: {
    devProxy: {
      "/dezibot-hub": {
        target: "http://localhost:5160/dezibot-hub",
        ws: true, // WebSocket proxying
        changeOrigin: true,
      },
    },
  },
  colorMode: {
    preference: 'light'
  },
  ssr: false,
})