import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AllCamerasListComponent } from './all-cameras-list.component';

describe('AllCamerasListComponent', () => {
  let component: AllCamerasListComponent;
  let fixture: ComponentFixture<AllCamerasListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AllCamerasListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AllCamerasListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
