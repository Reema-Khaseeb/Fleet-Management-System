import { Routes } from '@angular/router';
import { VehicleListComponent } from './vehicle/vehicle-list/vehicle-list.component';
import { AddVehicleComponent } from './vehicle/add-vehicle/add-vehicle.component';
import { UpdateVehicleComponent } from './vehicle/update-vehicle/update-vehicle.component';
import { VehicleInfoListComponent } from './vehicle-info/vehicle-info-list/vehicle-info-list.component';
import { AddVehicleInfoComponent } from './vehicle-info/add-vehicle-info/add-vehicle-info.component';
import { UpdateVehicleInfoComponent } from './vehicle-info/update-vehicle-info/update-vehicle-info.component';
import { GetDriversComponent } from './driver/get-drivers/get-drivers.component';
import { AddDriverComponent } from './driver/add-driver/add-driver.component';
import { UpdateDriverComponent } from './driver/update-driver/update-driver.component';
import { GetGeofencesComponent } from './geofence/get-geofences/get-geofences.component';
import { GetRouteHistoryComponent } from './features/route-history/components/get-route-history/get-route-history.component';

export const routes: Routes = [
  { path: '', component: VehicleListComponent },
  { path: 'add-vehicle', component: AddVehicleComponent },
  { path: 'update-vehicle/:id', component: UpdateVehicleComponent },
  { path: 'vehicle-info-list', component: VehicleInfoListComponent },
  { path: 'add-vehicle-info', component: AddVehicleInfoComponent },
  { path: 'update-vehicle-info/:id', component: UpdateVehicleInfoComponent },
  { path: 'get-drivers', component: GetDriversComponent },
  { path: 'add-driver', component: AddDriverComponent },
  { path: 'update-driver/:id', component: UpdateDriverComponent },
  { path: 'get-geofences', component: GetGeofencesComponent },
  { path: 'get-route-history', component: GetRouteHistoryComponent },
  { path: '**', redirectTo: '' }
];
