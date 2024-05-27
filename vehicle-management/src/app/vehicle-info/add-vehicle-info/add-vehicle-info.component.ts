import { Component } from '@angular/core';
import { VehicleInfoService } from '../../services/vehicle-info.service';
import { GVAR } from '../../models/gvar.model';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-add-vehicle-info',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './add-vehicle-info.component.html',
  styleUrls: ['./add-vehicle-info.component.css']
})
export class AddVehicleInfoComponent {
  vehicleInfo = {
    VehicleID: '',
    DriverID: '',
    VehicleMake: '',
    VehicleModel: '',
    PurchaseDate: ''
  };

  constructor(private vehicleInfoService: VehicleInfoService, private router: Router) {}

  addVehicleInfo(): void {
    const gvar: GVAR = {
      DicOfDic: { Tags: { ...this.vehicleInfo } },
      DicOfDT: {}
    };

    this.vehicleInfoService.addVehicleInfo(gvar).subscribe(response => {
      if (response.DicOfDic['Tags']['STS'] === '1') {
        this.router.navigate(['/vehicle-info-list']);
      } else {
        alert('Error adding vehicle information');
      }
    });
  }
}
