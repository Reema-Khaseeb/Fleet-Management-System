import { Component, OnInit } from '@angular/core';
import { GeofencesService } from '../../services/geofences.service';
import { GVAR } from '../../models/gvar.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-get-geofences',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './get-geofences.component.html',
  styleUrls: ['./get-geofences.component.css']
})
export class GetGeofencesComponent implements OnInit {
  geofences: any[] = [];

  constructor(private geofencesService: GeofencesService) {}

  ngOnInit(): void {
    this.geofencesService.getAllGeofences().subscribe((response: GVAR) => {
      if (response.DicOfDic['Tags']['STS'] === '1') {
        this.geofences = response.DicOfDT['Geofences'];
      } else {
        alert('Error fetching geofences');
      }
    });
  }
}
