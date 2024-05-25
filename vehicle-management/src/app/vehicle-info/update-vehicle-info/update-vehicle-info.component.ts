import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { VehicleInfoService } from '../../services/vehicle-info.service';
import { GVAR } from '../../models/gvar.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-update-vehicle-info',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './update-vehicle-info.component.html',
  styleUrls: ['./update-vehicle-info.component.css']
})
export class UpdateVehicleInfoComponent {
  vehicleInfo = {
    ID: '',
    VehicleID: '',
    DriverID: '',
    VehicleMake: '',
    VehicleModel: '',
    PurchaseDate: ''
  };

  constructor(
    private route: ActivatedRoute,
    private vehicleInfoService: VehicleInfoService,
    private router: Router
  ) {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.vehicleInfo.ID = id;
    }
  }

  updateVehicleInfo(): void {
    const gvar: GVAR = {
      DicOfDic: { Tags: { ...this.vehicleInfo } },
      DicOfDT: {}
    };

    this.vehicleInfoService.updateVehicleInfo(gvar).subscribe((response: any) => {
      if (response.DicOfDic.Tags.STS === '1') {
        this.router.navigate(['/vehicle-info-list']);
      } else {
        alert('Error updating vehicle information');
      }
    });
  }
}
