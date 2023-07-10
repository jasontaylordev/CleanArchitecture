<template>
  <h3>Loading...</h3>
</template>

<script lang="ts" setup>
import { useAuthServices } from '~~/composables/useAuthServices';

const services = useAuthServices()
const router = useRouter()

const authenticateOidc = async () => {
  try {
    await services.$auth.signInCallback()
    router.push('/')
  } catch (error) {
    console.log(error)
  }
}

await authenticateOidc()
</script>