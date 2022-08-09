import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ConnectCameraModalComponent } from './connect-camera-modal.component';

describe('ConnectCameraModalComponent', () => {
  let component: ConnectCameraModalComponent;
  let fixture: ComponentFixture<ConnectCameraModalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ConnectCameraModalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConnectCameraModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
