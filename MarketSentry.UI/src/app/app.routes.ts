import { Routes } from '@angular/router';
import { Dashboard } from './pages/dashboard/dashboard';
import { Settings } from './pages/settings/settings';

export const routes: Routes = [
  { path: '', component: Dashboard }, // Rota padrão (Home)
  { path: 'settings', component: Settings }
];