import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { GVAR } from '../models/gvar.model';

@Injectable({
  providedIn: 'root'
})
export class DriverService {
  private apiUrl = 'https://localhost:7129/api/1/drivers';

  constructor(private http: HttpClient) {}

  getDrivers(): Observable<GVAR> {
    return this.http.get<GVAR>(this.apiUrl);
  }

  addDriver(gvar: GVAR): Observable<GVAR> {
    return this.http.post<GVAR>(this.apiUrl, gvar);
  }

  updateDriver(gvar: GVAR): Observable<GVAR> {
    return this.http.patch<GVAR>(this.apiUrl, gvar);
  }
}
