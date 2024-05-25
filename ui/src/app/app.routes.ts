import { Routes } from '@angular/router';
import { StatusRoute } from '../pages/status';

export const routes: Routes = [
  {path: '', pathMatch: 'full', redirectTo: '/status'},
  {path: 'status', component: StatusRoute},
];
