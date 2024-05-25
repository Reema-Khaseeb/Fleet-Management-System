import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { GVAR } from '../models/gvar.model';

@Injectable({
  providedIn: 'root'
})
export class RouteHistoryService {
  private baseUrl = 'https://localhost:7129/api/1';

  constructor(private http: HttpClient) { }

  getRouteHistory(vehicleID: number, startEpoch: number, endEpoch: number): Observable<GVAR> {
    return this.http.get<GVAR>(`${this.baseUrl}/vehicles/${vehicleID}/routehistory`, {
      params: {
        startEpoch: startEpoch.toString(),
        endEpoch: endEpoch.toString()
      }
    });
  }
}
