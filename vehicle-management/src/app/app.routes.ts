import { Routes } from '@angular/router';
import { VehicleListComponent } from './vehicle-list/vehicle-list.component';

export const routes: Routes = [
  { path: '', component: VehicleListComponent },
  { path: '**', redirectTo: '' }
];
