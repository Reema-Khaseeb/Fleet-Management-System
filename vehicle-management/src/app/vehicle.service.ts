import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { GVAR } from './models/gvar.model';

@Injectable({
  providedIn: 'root'
})
export class VehicleService {
  private baseUrl = 'https://localhost:7129/api/1';

  constructor(private http: HttpClient) {}

  getAllVehicles(): Observable<GVAR> {
    return this.http.get<GVAR>(`${this.baseUrl}/vehicles`);
  }

  getVehicleDetails(vehicleID: number): Observable<GVAR> {
    return this.http.get<GVAR>(`${this.baseUrl}/vehicles/${vehicleID}`);
  }

  getDrivers(): Observable<GVAR> {
    return this.http.get<GVAR>(`${this.baseUrl}/drivers`);
  }
}
