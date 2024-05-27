import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { DriverService } from '../../services/driver.service';
import { GVAR } from '../../models/gvar.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-add-driver',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './add-driver.component.html',
  styleUrls: ['./add-driver.component.css']
})
export class AddDriverComponent {
  driver = {
    DriverName: '',
    PhoneNumber: ''
  };

  constructor(private driverService: DriverService, private router: Router) {}

  addDriver(): void {
    const gvar: GVAR = {
      DicOfDic: { Tags: { ...this.driver } },
      DicOfDT: {}
    };

    this.driverService.addDriver(gvar).subscribe((response: any) => {
      if (response.DicOfDic.Tags.STS === '1') {
        this.router.navigate(['/get-drivers']);
      } else {
        alert('Error adding driver');
      }
    });
  }
}
