import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EventVideoDialogComponent } from './event-video-dialog.component';

describe('EventVideoDialogComponent', () => {
  let component: EventVideoDialogComponent;
  let fixture: ComponentFixture<EventVideoDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EventVideoDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EventVideoDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
