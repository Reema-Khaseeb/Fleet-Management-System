import { Component, Input, OnChanges, SimpleChanges, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { VehicleService } from '../vehicle.service';
import { GVAR } from '../models/gvar.model';

@Component({
  selector: 'app-vehicle-detail',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './vehicle-detail.component.html',
  styleUrls: ['./vehicle-detail.component.css']
})
export class VehicleDetailComponent implements OnChanges {
  @Input() vehicleID!: number;
  @Output() close = new EventEmitter<void>();
  vehicle: any;
  drivers: any[] = [];

  constructor(private vehicleService: VehicleService) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['vehicleID'] && changes['vehicleID'].currentValue) {
      console.log(`VehicleDetailComponent detected vehicleID change: ${this.vehicleID}`);
      this.loadVehicleDetails();
    }
  }

  loadVehicleDetails(): void {
    this.vehicleService.getVehicleDetails(this.vehicleID).subscribe(
      (data: GVAR) => {
        console.log('Vehicle details fetched:', data);
        if (data && data.DicOfDT && data.DicOfDT['VehicleInformation']) {
          this.vehicle = data.DicOfDT['VehicleInformation'][0];
          console.log('Assigned vehicle details:', this.vehicle);
        } else {
          console.error('Vehicle information not found in response:', data);
        }
      },
      (error) => {
        console.error('Error fetching vehicle details:', error);
      }
    );
  }

  closeModal(): void {
    this.close.emit();
  }
}
