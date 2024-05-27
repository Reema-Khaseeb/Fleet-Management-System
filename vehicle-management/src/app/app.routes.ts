import { Routes } from '@angular/router';
import { VehicleListComponent } from './vehicle/vehicle-list/vehicle-list.component';
import { AddVehicleComponent } from './vehicle/add-vehicle/add-vehicle.component';
import { UpdateVehicleComponent } from './vehicle/update-vehicle/update-vehicle.component';
import { VehicleInfoListComponent } from './vehicle-info/vehicle-info-list/vehicle-info-list.component';
import { AddVehicleInfoComponent } from './vehicle-info/add-vehicle-info/add-vehicle-info.component';
import { UpdateVehicleInfoComponent } from './vehicle-info/update-vehicle-info/update-vehicle-info.component';


export const routes: Routes = [
  { path: '', component: VehicleListComponent },
  { path: 'add-vehicle', component: AddVehicleComponent },
  { path: 'update-vehicle/:id', component: UpdateVehicleComponent },
  { path: 'vehicle-info-list', component: VehicleInfoListComponent },
  { path: 'add-vehicle-info', component: AddVehicleInfoComponent },
  { path: 'update-vehicle-info/:id', component: UpdateVehicleInfoComponent },
  { path: '**', redirectTo: '' }
];
