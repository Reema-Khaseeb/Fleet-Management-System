import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { VehicleListComponent } from './vehicle-list/vehicle-list.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, VehicleListComponent],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'vehicle-management';
}
