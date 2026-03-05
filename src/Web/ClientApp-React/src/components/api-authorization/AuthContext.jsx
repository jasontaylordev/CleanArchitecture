import { createContext, useContext, useState, useEffect } from 'react';
import { UsersClient, LoginRequest, RegisterRequest } from '../../web-api-client';

const AuthContext = createContext(null);

const client = new UsersClient();

export function AuthProvider({ children }) {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    client.infoGET()
      .then(() => setIsAuthenticated(true))
      .catch(() => setIsAuthenticated(false))
      .finally(() => setIsLoading(false));
  }, []);

  const login = (email, password) =>
    client.login(true, undefined, new LoginRequest({ email, password }))
      .then(() => setIsAuthenticated(true));

  const register = (email, password) =>
    client.register(new RegisterRequest({ email, password }));

  const logout = () =>
    client.logout({})
      .then(() => setIsAuthenticated(false));

  return (
    <AuthContext.Provider value={{ isAuthenticated, isLoading, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export const useAuth = () => useContext(AuthContext);