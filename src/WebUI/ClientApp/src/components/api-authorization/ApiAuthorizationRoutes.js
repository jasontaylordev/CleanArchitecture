import React from 'react';
import { Login } from './Login'
import { Logout } from './Logout'
import { ApplicationPaths, LoginActions, LogoutActions } from './ApiAuthorizationConstants';

const ApiAuthorizationRoutes = [
  {
    path: ApplicationPaths.Login,
    element: loginAction(LoginActions.Login)
  },
  {
    path: ApplicationPaths.LoginFailed,
    element: loginAction(LoginActions.LoginFailed)
  },
  {
    path: ApplicationPaths.LoginCallback,
    element: loginAction(LoginActions.LoginCallback)
  },
  {
    path: ApplicationPaths.Profile,
    element: loginAction(LoginActions.Profile)
  },
  {
    path: ApplicationPaths.Register,
    element: loginAction(LoginActions.Register)
  },
  {
    path: ApplicationPaths.LogOut,
    element: logoutAction(LogoutActions.Logout)
  },
  {
    path: ApplicationPaths.LogOutCallback,
    element: logoutAction(LogoutActions.LogoutCallback)
  },
  {
    path: ApplicationPaths.LoggedOut,
    element: logoutAction(LogoutActions.LoggedOut)
  }
];

function loginAction(name){
  return <Login action={name}></Login>;
}

function logoutAction(name) {
  return <Logout action={name}></Logout>;
}

export default ApiAuthorizationRoutes;
