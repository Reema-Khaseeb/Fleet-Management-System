import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { GVAR } from '../models/gvar.model';

@Injectable({
  providedIn: 'root'
})
export class GeofencesService {
  private apiUrl = 'https://localhost:7129/api/1/geofences';

  constructor(private http: HttpClient) {}

  getAllGeofences(): Observable<GVAR> {
    return this.http.get<GVAR>(this.apiUrl);
  }
}
