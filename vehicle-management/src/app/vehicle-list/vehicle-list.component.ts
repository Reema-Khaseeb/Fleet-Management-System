import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { VehicleService } from '../vehicle.service';
import { GVAR } from '../models/gvar.model';

@Component({
  selector: 'app-vehicle-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './vehicle-list.component.html',
  styleUrls: ['./vehicle-list.component.css']
})
export class VehicleListComponent implements OnInit {
  vehicles: any[] = [];
  selectedVehicle: any = null;

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
    this.vehicleService.getVehicleDetails(vehicleID).subscribe(
      (data: GVAR) => {
        if (data && data.DicOfDT && data.DicOfDT['VehicleDetails']) {
          this.selectedVehicle = data.DicOfDT['VehicleDetails'][0];
        } else {
          console.error('Unexpected data structure for vehicle details:', data);
        }
      },
      (error) => {
        console.error('Error fetching vehicle details:', error);
      }
    );
  }
}
