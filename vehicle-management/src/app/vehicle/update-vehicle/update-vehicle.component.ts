// import { Component, Input } from '@angular/core';
// import { VehicleService } from '../vehicle.service';
// import { GVAR } from '../models/gvar.model';
// import { Router } from '@angular/router';

// @Component({
//   selector: 'app-update-vehicle',
//   // standalone: true,
//   // imports: [],
//   templateUrl: './update-vehicle.component.html',
//   styleUrls: ['./update-vehicle.component.css']
// })
// export class UpdateVehicleComponent {
//   @Input() vehicle: any;

//   constructor(private vehicleService: VehicleService, private router: Router) {}

//   updateVehicle(): void {
//     const gvar: GVAR = {
//       DicOfDic: { Tags: { VehicleID: this.vehicle.VehicleID, VehicleNumber: this.vehicle.VehicleNumber, VehicleType: this.vehicle.VehicleType } },
//       DicOfDT: {}
//     };

//     this.vehicleService.updateVehicle(gvar).subscribe(response => {
//       if (response.DicOfDic['Tags']['STS'] === '1') {
//         this.router.navigate(['/']);
//       } else {
//         alert('Error updating vehicle');
//       }
//     });
//   }
// }


import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { VehicleService } from '../../vehicle.service';
import { GVAR } from '../../models/gvar.model';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-update-vehicle',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './update-vehicle.component.html',
  styleUrls: ['./update-vehicle.component.css']
})
export class UpdateVehicleComponent implements OnInit {
  vehicle = { VehicleID: 0, VehicleNumber: '', VehicleType: '' };

  constructor(
    private route: ActivatedRoute,
    private vehicleService: VehicleService,
    private router: Router
  ) {}

  ngOnInit(): void {
    const vehicleID = this.route.snapshot.paramMap.get('id');
    if (vehicleID) {
      this.vehicle.VehicleID = +vehicleID;
      this.vehicleService.getVehicleDetails(this.vehicle.VehicleID).subscribe(response => {
        if (response.DicOfDic['Tags']['STS'] === '1' && response.DicOfDT['VehicleInformation']) {
          const vehicleData = response.DicOfDT['VehicleInformation'][0];
          this.vehicle.VehicleNumber = vehicleData.VehicleNumber;
          this.vehicle.VehicleType = vehicleData.VehicleType;
        }
      });
    }
  }

  updateVehicle(): void {
    const gvar: GVAR = {
      DicOfDic: { Tags: { VehicleID: this.vehicle.VehicleID.toString(), VehicleNumber: this.vehicle.VehicleNumber, VehicleType: this.vehicle.VehicleType } },
      DicOfDT: {}
    };

    this.vehicleService.updateVehicle(gvar).subscribe(response => {
      if (response.DicOfDic['Tags']['STS'] === '1') {
        this.router.navigate(['/']);
      } else {
        alert('Error updating vehicle');
      }
    });
  }
}
