import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RecognitionObjectsComponent } from './recognition-objects.component';

describe('RecognitionObjectsComponent', () => {
  let component: RecognitionObjectsComponent;
  let fixture: ComponentFixture<RecognitionObjectsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RecognitionObjectsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RecognitionObjectsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
