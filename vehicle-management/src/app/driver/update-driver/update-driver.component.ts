import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DriverService } from '../../services/driver.service';
import { GVAR } from '../../models/gvar.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-update-driver',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './update-driver.component.html',
  styleUrls: ['./update-driver.component.css']
})
export class UpdateDriverComponent implements OnInit {
  driver = {
    DriverID: '',
    DriverName: '',
    PhoneNumber: ''
  };

  constructor(
    private route: ActivatedRoute,
    private driverService: DriverService,
    private router: Router
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.driver.DriverID = id;
    }
  }

  updateDriver(): void {
    const gvar: GVAR = {
      DicOfDic: { Tags: { ...this.driver } },
      DicOfDT: {}
    };

    this.driverService.updateDriver(gvar).subscribe((response: any) => {
      if (response.DicOfDic.Tags.STS === '1') {
        this.router.navigate(['/get-drivers']);
      } else {
        alert('Error updating driver');
      }
    });
  }
}
