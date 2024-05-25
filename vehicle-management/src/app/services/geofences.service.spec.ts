import { TestBed } from '@angular/core/testing';

import { GeofencesService } from './geofences.service';

describe('GeofencesService', () => {
  let service: GeofencesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GeofencesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
