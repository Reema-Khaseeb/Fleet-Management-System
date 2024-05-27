import { TestBed } from '@angular/core/testing';

import { VehicleInfoService } from './vehicle-info.service';

describe('VehicleInfoService', () => {
  let service: VehicleInfoService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(VehicleInfoService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
