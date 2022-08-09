import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { IconDashboardComponent } from './icon-dashboard.component';

describe('IconDashboardComponent', () => {
  let component: IconDashboardComponent;
  let fixture: ComponentFixture<IconDashboardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ IconDashboardComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(IconDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
