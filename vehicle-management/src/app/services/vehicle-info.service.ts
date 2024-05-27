import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { GVAR } from '../models/gvar.model';

@Injectable({
  providedIn: 'root'
})
export class VehicleInfoService {
  private apiUrl = 'https://localhost:7129/api/1/vehicles-Informations';

  constructor(private http: HttpClient) {}

  addVehicleInfo(gvar: GVAR): Observable<GVAR> {
    return this.http.post<GVAR>(this.apiUrl, gvar);
  }

  updateVehicleInfo(gvar: GVAR): Observable<GVAR> {
    return this.http.patch<GVAR>(this.apiUrl, gvar);
  }

  deleteVehicleInfo(gvar: GVAR): Observable<GVAR> {
    return this.http.delete<GVAR>(this.apiUrl, { body: gvar });
  }

  getAllVehicleInfos(): Observable<GVAR> {
    return this.http.get<GVAR>(this.apiUrl);
  }

  getVehicleInfoDetails(id: string): Observable<GVAR> {
    return this.http.get<GVAR>(`${this.apiUrl}/${id}`);
  }
}
