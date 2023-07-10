<template>
    <h3>Logging Out...</h3>
  </template>
  
  <script lang="ts" setup>
  import { useAuth } from '@/stores/auth'
import { useAuthServices } from '~~/composables/useAuthServices';
  
  const services = useAuthServices()
  const authStore = useAuth()
  
  const logOutOidc = async () => {
    try {
      authStore.clearUserSession()
      await services.$auth.logout()
    } catch (error) {
      console.log(error)
    }
  }
  
  await logOutOidc()
  </script>