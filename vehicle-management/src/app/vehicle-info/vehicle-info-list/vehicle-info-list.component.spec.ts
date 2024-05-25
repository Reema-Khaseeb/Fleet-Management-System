import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VehicleInfoListComponent } from './vehicle-info-list.component';

describe('VehicleInfoListComponent', () => {
  let component: VehicleInfoListComponent;
  let fixture: ComponentFixture<VehicleInfoListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VehicleInfoListComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(VehicleInfoListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
