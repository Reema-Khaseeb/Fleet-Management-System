import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { RouteHistoryService } from '../../../../services/route-history.service';
import { GVAR } from '../../../../models/gvar.model';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-get-route-history',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './get-route-history.component.html',
  styleUrls: ['./get-route-history.component.css']
})
export class GetRouteHistoryComponent implements OnInit {
  routeHistoryForm: FormGroup;
  routeHistory: any[] = [];

  constructor(
    private fb: FormBuilder,
    private routeHistoryService: RouteHistoryService
  ) {
    this.routeHistoryForm = this.fb.group({
      vehicleId: ['', Validators.required],
      startEpoch: ['', Validators.required],
      endEpoch: ['', Validators.required]
    });
  }

  ngOnInit(): void {}

  onSubmit(): void {
    if (this.routeHistoryForm.valid) {
      const vehicleID = this.routeHistoryForm.value.vehicleId;
      const startEpoch = this.routeHistoryForm.value.startEpoch;
      const endEpoch = this.routeHistoryForm.value.endEpoch;

      this.routeHistoryService.getRouteHistory(vehicleID, startEpoch, endEpoch).subscribe(
        (data: GVAR) => {
          if (data && data.DicOfDT && data.DicOfDT['RouteHistory']) {
            this.routeHistory = data.DicOfDT['RouteHistory'].map(record => ({
              ...record,
              GPSTime: this.convertEpochToReadableDate(record.GPSTime) // Convert epoch to readable date
            }));
          } else {
            alert('Error fetching route history');
          }
        },
        (error) => {
          console.error('Error fetching route history', error);
        }
      );
    }
  }

  convertEpochToReadableDate(epoch: number): string {
    return new Date(epoch * 1000).toLocaleString();
  }
}
