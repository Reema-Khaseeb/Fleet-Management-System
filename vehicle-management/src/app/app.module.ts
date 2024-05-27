import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { VehicleListComponent } from './vehicle/vehicle-list/vehicle-list.component';
import { VehicleDetailComponent } from './vehicle/vehicle-detail/vehicle-detail.component';
import { AddVehicleComponent } from './vehicle/add-vehicle/add-vehicle.component';
import { UpdateVehicleComponent } from './vehicle/update-vehicle/update-vehicle.component';
import { VehicleInfoListComponent } from './vehicle-info/vehicle-info-list/vehicle-info-list.component';
import { AddVehicleInfoComponent } from './vehicle-info/add-vehicle-info/add-vehicle-info.component';
import { UpdateVehicleInfoComponent } from './vehicle-info/update-vehicle-info/update-vehicle-info.component';
import { GetDriversComponent } from './driver/get-drivers/get-drivers.component';
import { AddDriverComponent } from './driver/add-driver/add-driver.component';
import { UpdateDriverComponent } from './driver/update-driver/update-driver.component';
import { GetGeofencesComponent } from './geofence/get-geofences/get-geofences.component';
import { CommonModule } from '@angular/common'; 
import { FormsModule } from '@angular/forms';

@NgModule({
  declarations: [
    AppComponent,
    VehicleListComponent,
    VehicleDetailComponent,
    AddVehicleComponent,
    UpdateVehicleComponent,
    VehicleInfoListComponent,
    AddVehicleInfoComponent,
    UpdateVehicleInfoComponent,
    GetDriversComponent,
    AddDriverComponent,
    UpdateDriverComponent,
    GetGeofencesComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    CommonModule,
    AppRoutingModule,
    FormsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
