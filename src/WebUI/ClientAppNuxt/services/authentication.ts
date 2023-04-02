import { IdTokenClaims, User, UserManager } from 'oidc-client-ts';
import { Pinia, storeToRefs } from 'pinia';
import { ApplicationName, ApplicationPaths } from '~~/constants/authentication';
import { useAuth } from '~~/stores/auth';
import { useUserStore } from '~~/stores/user.store';

export type IAuthenticationResult =
  SuccessAuthenticationResult |
  FailureAuthenticationResult |
  RedirectAuthenticationResult;

export interface SuccessAuthenticationResult {
  status: AuthenticationResultStatus.Success;
  state: any;
}

export interface FailureAuthenticationResult {
  status: AuthenticationResultStatus.Fail;
  message: string;
}

export interface RedirectAuthenticationResult {
  status: AuthenticationResultStatus.Redirect;
}

export enum AuthenticationResultStatus {
  Success,
  Redirect,
  Fail
}

export interface IUser {
  name?: string;
}

export class AuthorizeService {
  // By default pop ups are disabled because they don't work properly on Edge.
  // If you want to enable pop up authentication simply set this flag to false.

  private userManager?: UserManager;

  private piniaInstance: Pinia = useNuxtApp().$pinia as Pinia;

  constructor(){
    this.ensureUserManagerInitialized();
  }

  public signInRedirect() {
    this.ensureUserManagerInitialized();
    return this.userManager?.signinRedirect()
  }

  public signInCallback() {
    this.ensureUserManagerInitialized();
    return this.userManager?.signinCallback()
  }

  public renewToken() {
    this.ensureUserManagerInitialized();
    return this.userManager?.signinSilentCallback()
  }

  public logout() {
    this.ensureUserManagerInitialized();
    return this.userManager?.signoutRedirect()
  }

  public getUser() {
    this.ensureUserManagerInitialized();
    return this.userManager?.getUser()
  }
  
private async ensureUserManagerInitialized(): Promise<void> {

  const store = useAuth(this.piniaInstance);
    
    if (this.userManager !== undefined) {
      return;
    }

    const response = await fetch(ApplicationPaths.ApiAuthorizationClientConfigurationUrl);
    if (!response.ok) {
      throw new Error(`Could not load settings for '${ApplicationName}'`);
    }

    const settings: any = await response.json();
    settings.automaticSilentRenew = true;
    settings.includeIdTokenInSilentRenew = true;
    this.userManager = new UserManager(settings);

    this.userManager.events.addUserSignedOut(async () => {
      await this.userManager!.removeUser();
      store.clearUserSession();
    });
  }
  
}
