import { Routes } from '@angular/router';
import { VehicleListComponent } from './vehicle-list/vehicle-list.component';
import { AddVehicleComponent } from './vehicle/add-vehicle/add-vehicle.component';
import { UpdateVehicleComponent } from './vehicle/update-vehicle/update-vehicle.component';

export const routes: Routes = [
  { path: '', component: VehicleListComponent },
  { path: 'add-vehicle', component: AddVehicleComponent },
  { path: 'update-vehicle/:id', component: UpdateVehicleComponent },
  { path: '**', redirectTo: '' }
];
