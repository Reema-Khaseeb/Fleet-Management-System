import { Component, OnInit } from '@angular/core';
import { VehicleInfoService } from '../../services/vehicle-info.service';
import { GVAR } from '../../models/gvar.model';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-vehicle-info-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './vehicle-info-list.component.html',
  styleUrls: ['./vehicle-info-list.component.css']
})
export class VehicleInfoListComponent implements OnInit {
  vehicleInfos: any[] = [];

  constructor(private vehicleInfoService: VehicleInfoService, private router: Router) {}

  ngOnInit(): void {
    this.vehicleInfoService.getAllVehicleInfos().subscribe(
      (data: GVAR) => {
        if (data && data.DicOfDT && data.DicOfDT['VehiclesInformations']) {
          this.vehicleInfos = data.DicOfDT['VehiclesInformations'].map(info => ({
            ...info,
            PurchaseDate: new Date(info.PurchaseDate).toLocaleDateString() // Convert the date
          }));
        }
      },
      (error) => {
        console.error('Error fetching vehicle information:', error);
      }
    );
  }

  navigateToAddVehicleInfo(): void {
    this.router.navigate(['/add-vehicle-info']);
  }

  navigateToUpdateVehicleInfo(id: number): void {
    this.router.navigate(['/update-vehicle-info', id]);
  }

  deleteVehicleInfo(id: number): void {
    const gvar: GVAR = {
      DicOfDic: { Tags: { ID: id.toString() } },
      DicOfDT: {}
    };

    this.vehicleInfoService.deleteVehicleInfo(gvar).subscribe(response => {
      if (response.DicOfDic['Tags']['STS'] === '1') {
        this.vehicleInfos = this.vehicleInfos.filter(info => info.ID !== id);
      } else {
        alert('Error deleting vehicle information');
      }
    });
  }
}
