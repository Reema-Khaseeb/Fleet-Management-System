import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddRouteHistoryComponent } from './add-route-history.component';

describe('AddRouteHistoryComponent', () => {
  let component: AddRouteHistoryComponent;
  let fixture: ComponentFixture<AddRouteHistoryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddRouteHistoryComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(AddRouteHistoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
