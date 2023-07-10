import { AuthorizeService } from "~~/services/authentication"

export const useAuthServices = () => {
    return {
      $auth: new AuthorizeService(),
    }
  }