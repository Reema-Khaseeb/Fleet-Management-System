import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GetDriversComponent } from './get-drivers.component';

describe('GetDriversComponent', () => {
  let component: GetDriversComponent;
  let fixture: ComponentFixture<GetDriversComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GetDriversComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(GetDriversComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
