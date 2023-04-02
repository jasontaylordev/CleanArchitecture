<template>
    <h3>Loading...</h3>
  </template>
  
  <script lang="ts" setup>import { useAuthServices } from '~~/composables/useAuthServices';

  
  const services = useAuthServices()
  const router = useRouter()
  
  const silentRefreshOidc = async () => {
    try {
      await services.$auth.renewToken()
      router.push('/')
    } catch (error) {
      console.log(error)
    }
  }
  
  await silentRefreshOidc()
  </script>