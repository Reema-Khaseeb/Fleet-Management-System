import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { VehicleService } from '../../services/vehicle.service';
import { GVAR } from '../../models/gvar.model';
import { VehicleDetailComponent } from '../vehicle-detail/vehicle-detail.component';
import { Router } from '@angular/router';

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

  constructor(private vehicleService: VehicleService, private router: Router) {}

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

  deleteVehicle(vehicleID: number): void {
    const gvar: GVAR = {
      DicOfDic: { Tags: { VehicleID: vehicleID.toString() } },
      DicOfDT: {}
    };

    this.vehicleService.deleteVehicle(gvar).subscribe(response => {
      if (response.DicOfDic['Tags']['STS'] === '1') {
        this.vehicles = this.vehicles.filter(vehicle => vehicle.VehicleID !== vehicleID);
      } else {
        alert('Error deleting vehicle');
      }
    });
  }

  navigateToAddVehicle(): void {
    this.router.navigate(['/add-vehicle']);
  }

  navigateToUpdateVehicle(vehicleID: number): void {
    this.router.navigate(['/update-vehicle', vehicleID]);
  }
}
