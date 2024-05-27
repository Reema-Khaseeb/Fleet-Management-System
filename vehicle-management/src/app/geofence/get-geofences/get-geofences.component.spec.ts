import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GetGeofencesComponent } from './get-geofences.component';

describe('GetGeofencesComponent', () => {
  let component: GetGeofencesComponent;
  let fixture: ComponentFixture<GetGeofencesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GetGeofencesComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(GetGeofencesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
