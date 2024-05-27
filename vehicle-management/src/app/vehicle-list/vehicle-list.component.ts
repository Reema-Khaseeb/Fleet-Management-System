import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { VehicleService } from '../vehicle.service';
import { GVAR } from '../models/gvar.model';
import { VehicleDetailComponent } from '../vehicle-detail/vehicle-detail.component';

@Component({
  selector: 'app-vehicle-list',
  standalone: true,
  imports: [CommonModule, VehicleDetailComponent],
  templateUrl: './vehicle-list.component.html',
  styleUrls: ['./vehicle-list.component.css']
})
export class VehicleListComponent implements OnInit {
  vehicles: any[] = [];
  selectedVehicleID: number | null = null;
  selectedVehicleDetails: any = null;

  constructor(private vehicleService: VehicleService) {}

  ngOnInit(): void {
    this.vehicleService.getAllVehicles().subscribe(
      (data: GVAR) => {
        console.log('API response received:', data);
        if (data && data.DicOfDT && data.DicOfDT['Vehicles']) {
          this.vehicles = data.DicOfDT['Vehicles'];
          console.log('Vehicles:', this.vehicles);
        } else {
          console.error('Unexpected data structure:', data);
        }
      },
      (error) => {
        console.error('Error fetching vehicle data:', error);
      }
    );
  }

  showMore(vehicleID: number): void {
    this.selectedVehicleID = vehicleID;
  }
}

