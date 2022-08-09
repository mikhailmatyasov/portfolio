import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TrafficHourlyChartComponent } from './traffic-hourly-chart.component';

describe('TrafficHourlyChartComponent', () => {
  let component: TrafficHourlyChartComponent;
  let fixture: ComponentFixture<TrafficHourlyChartComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TrafficHourlyChartComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TrafficHourlyChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
