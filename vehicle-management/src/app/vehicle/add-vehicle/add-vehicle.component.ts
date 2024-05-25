// import { Component } from '@angular/core';
// import { VehicleService } from '../vehicle.service';
// import { GVAR } from '../models/gvar.model';
// import { Router } from '@angular/router';

// @Component({
//   selector: 'app-add-vehicle',
//   // standalone: true,
//   // imports: [],
//   templateUrl: './add-vehicle.component.html',
//   styleUrls: ['./add-vehicle.component.css']
// })
// export class AddVehicleComponent {
//   vehicle = { VehicleNumber: '', VehicleType: '' };

//   constructor(private vehicleService: VehicleService, private router: Router) {}

//   addVehicle(): void {
//     const gvar: GVAR = {
//       DicOfDic: { Tags: { VehicleNumber: this.vehicle.VehicleNumber, VehicleType: this.vehicle.VehicleType } },
//       DicOfDT: {}
//     };

//     this.vehicleService.addVehicle(gvar).subscribe(response => {
//       if (response.DicOfDic['Tags']['STS'] === '1') {
//         this.router.navigate(['/']);
//       } else {
//         alert('Error adding vehicle');
//       }
//     });
//   }
// }
import { Component } from '@angular/core';
import { VehicleService } from '../../vehicle.service';
import { GVAR } from '../../models/gvar.model';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-add-vehicle',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './add-vehicle.component.html',
  styleUrls: ['./add-vehicle.component.css']
})
export class AddVehicleComponent {
  vehicle = { VehicleNumber: '', VehicleType: '' };

  constructor(private vehicleService: VehicleService, private router: Router) {}

  addVehicle(): void {
    const gvar: GVAR = {
      DicOfDic: { Tags: { VehicleNumber: this.vehicle.VehicleNumber, VehicleType: this.vehicle.VehicleType } },
      DicOfDT: {}
    };

    this.vehicleService.addVehicle(gvar).subscribe(response => {
      if (response.DicOfDic['Tags']['STS'] === '1') {
        this.router.navigate(['/']);
      } else {
        alert('Error adding vehicle');
      }
    });
  }
}
