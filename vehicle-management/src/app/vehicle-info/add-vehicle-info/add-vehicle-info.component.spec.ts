import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddVehicleInfoComponent } from './add-vehicle-info.component';

describe('AddVehicleInfoComponent', () => {
  let component: AddVehicleInfoComponent;
  let fixture: ComponentFixture<AddVehicleInfoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddVehicleInfoComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(AddVehicleInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
