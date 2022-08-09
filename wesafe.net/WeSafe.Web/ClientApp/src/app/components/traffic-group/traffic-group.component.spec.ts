import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TrafficGroupComponent } from './traffic-group.component';

describe('TrafficGroupComponent', () => {
  let component: TrafficGroupComponent;
  let fixture: ComponentFixture<TrafficGroupComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TrafficGroupComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TrafficGroupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
