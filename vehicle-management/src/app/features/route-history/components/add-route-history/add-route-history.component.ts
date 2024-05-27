import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { RouteHistoryService } from '../../../../services/route-history.service';
import { VehicleService } from '../../../../services/vehicle.service';
import { GVAR } from '../../../../models/gvar.model';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-add-route-history',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './add-route-history.component.html',
  styleUrls: ['./add-route-history.component.css']
})
export class AddRouteHistoryComponent implements OnInit {
  routeHistoryForm: FormGroup;
  vehicles: any[] = [];

  constructor(
    private fb: FormBuilder,
    private routeHistoryService: RouteHistoryService,
    private vehicleService: VehicleService,
    private router: Router
  ) {
    this.routeHistoryForm = this.fb.group({
      VehicleID: ['', Validators.required],
      VehicleDirection: ['', Validators.required],
      Status: ['', Validators.required],
      VehicleSpeed: ['', Validators.required],
      Epoch: ['', Validators.required],
      Address: ['', Validators.required],
      Latitude: ['', Validators.required],
      Longitude: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.vehicleService.getAllVehicles().subscribe(response => {
      this.vehicles = response.DicOfDT['Vehicles'];
    });
  }

  addRouteHistory(): void {
    const formValues = this.routeHistoryForm.value;
    const gvar: GVAR = {
      DicOfDic: {
        Tags: {
          VehicleID: formValues.VehicleID.toString(),
          VehicleDirection: formValues.VehicleDirection.toString(),
          Status: formValues.Status.toString(),
          VehicleSpeed: formValues.VehicleSpeed,
          Epoch: formValues.Epoch.toString(),
          Address: formValues.Address,
          Latitude: formValues.Latitude.toString(),
          Longitude: formValues.Longitude.toString()
        }
      },
      DicOfDT: {}
    };

    this.routeHistoryService.addRouteHistory(gvar).subscribe(response => {
      if (response.DicOfDic['Tags']['STS'] === '1') {
        this.router.navigate(['/get-route-history']);
      } else {
        alert('Error adding route history');
      }
    });
  }
}
