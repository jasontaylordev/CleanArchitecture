import { Routes } from '@angular/router';

export const requestsRoutes: Routes = [
  {
    path: '',
    loadComponent: () => import('./pages/request-list/request-list').then((m) => m.RequestListComponent),
    providers: [],
  },
];
