import { User } from 'oidc-client-ts'
import { useAuth } from '@/stores/auth'
import { useAuthServices } from '~~/composables/useAuthServices'

const authFlowRoutes = ['/auth', '/silent-refresh', '/logout']

export default defineNuxtRouteMiddleware(async (to, from) => {
  const authStore = useAuth()
  const services = useAuthServices()
  const user = (await services.$auth.getUser()) as User

  if (!user && !authFlowRoutes.includes(to.path)) {
    services.$auth.signInRedirect()
  } else {
    authStore.setUpUserCredentials(user)
  }
})