import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { IndicatorGaugeComponent } from './indicator-gauge.component';

describe('IndicatorGaugeComponent', () => {
  let component: IndicatorGaugeComponent;
  let fixture: ComponentFixture<IndicatorGaugeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ IndicatorGaugeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(IndicatorGaugeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
