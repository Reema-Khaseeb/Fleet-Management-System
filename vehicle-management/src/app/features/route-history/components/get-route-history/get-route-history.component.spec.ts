import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GetRouteHistoryComponent } from './get-route-history.component';

describe('GetRouteHistoryComponent', () => {
  let component: GetRouteHistoryComponent;
  let fixture: ComponentFixture<GetRouteHistoryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GetRouteHistoryComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(GetRouteHistoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
