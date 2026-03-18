import React, { Component } from 'react';
import { Route, Routes } from 'react-router-dom';
import AppRoutes from './AppRoutes';
import { Layout } from './components/Layout';
import { AuthProvider } from './components/api-authorization/AuthContext';
import { ThemeProvider } from './components/ThemeContext';

export default class App extends Component {
  static displayName = App.name;

  render() {
    return (
      <ThemeProvider>
      <AuthProvider>
        <Layout>
        <Routes>
          {AppRoutes.map((route, index) => {
            const { element, ...rest } = route;
            return <Route key={index} {...rest} element={element} />;
          })}
        </Routes>
        </Layout>
      </AuthProvider>
      </ThemeProvider>
    );
  }
}
