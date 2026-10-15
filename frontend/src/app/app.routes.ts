import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'companies', pathMatch: 'full' },
  {
    path: 'companies',
    loadComponent: () =>
      import('./features/companies/pages/companies-page/companies-page').then((m) => m.CompaniesPage),
  },
  {
    path: 'companies/new',
    loadComponent: () =>
      import('./features/companies/pages/company-create-page/company-create-page').then(
        (m) => m.CompanyCreatePage,
      ),
  },
  {
    path: 'companies/search',
    loadComponent: () =>
      import('./features/companies/pages/company-search-page/company-search-page').then(
        (m) => m.CompanySearchPage,
      ),
  },
  {
    path: 'companies/:id',
    loadComponent: () =>
      import('./features/companies/pages/company-detail-page/company-detail-page').then(
        (m) => m.CompanyDetailPage,
      ),
  },
  { path: '**', redirectTo: 'companies' },
];
