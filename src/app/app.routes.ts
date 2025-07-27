import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/search',
    pathMatch: 'full',
  },
  {
    path: 'search',
    loadComponent: () => import('./pages/search/search.page').then((m) => m.SearchPage),
  },
];
