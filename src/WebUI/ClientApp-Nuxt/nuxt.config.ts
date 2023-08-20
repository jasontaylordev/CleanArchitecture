// @ts-ignore
export default defineNuxtConfig({
    modules: ['@bootstrap-vue-next/nuxt'],
    css: ['bootstrap/dist/css/bootstrap.min.css'],
    devServer: {
        port: process.env.PORT || 44447,
        https: {
          key: process.env.SSL_KEY_FILE,
          cert: process.env.SSL_CRT_FILE
        }
      }
})