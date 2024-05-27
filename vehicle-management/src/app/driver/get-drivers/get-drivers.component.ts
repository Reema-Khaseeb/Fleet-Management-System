import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { DriverService } from '../../services/driver.service';
import { GVAR } from '../../models/gvar.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-get-drivers',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './get-drivers.component.html',
  styleUrls: ['./get-drivers.component.css']
})
export class GetDriversComponent implements OnInit {
  drivers: any[] = [];

  constructor(private driverService: DriverService, private router: Router) {}

  ngOnInit(): void {
    this.driverService.getDrivers().subscribe((response: any) => {
      if (response.DicOfDic.Tags.STS === '1') {
        this.drivers = response.DicOfDT.Drivers;
      } else {
        alert('Error fetching drivers');
      }
    });
  }

  navigateToAddDriver(): void {
    this.router.navigate(['/add-driver']);
  }

  navigateToUpdateDriver(driverID: string): void {
    this.router.navigate([`/update-driver/${driverID}`]);
  }
}
