import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateVehicleInfoComponent } from './update-vehicle-info.component';

describe('UpdateVehicleInfoComponent', () => {
  let component: UpdateVehicleInfoComponent;
  let fixture: ComponentFixture<UpdateVehicleInfoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UpdateVehicleInfoComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(UpdateVehicleInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
