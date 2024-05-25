import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateDriverComponent } from './update-driver.component';

describe('UpdateDriverComponent', () => {
  let component: UpdateDriverComponent;
  let fixture: ComponentFixture<UpdateDriverComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UpdateDriverComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(UpdateDriverComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
