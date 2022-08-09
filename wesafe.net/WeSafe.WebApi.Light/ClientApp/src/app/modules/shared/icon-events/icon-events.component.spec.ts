import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { IconEventsComponent } from './icon-events.component';

describe('IconEventsComponent', () => {
  let component: IconEventsComponent;
  let fixture: ComponentFixture<IconEventsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ IconEventsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(IconEventsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
