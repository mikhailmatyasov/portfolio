import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RecognitionObjectFormComponent } from './recognition-object-form.component';

describe('RecognitionObjectFormComponent', () => {
  let component: RecognitionObjectFormComponent;
  let fixture: ComponentFixture<RecognitionObjectFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RecognitionObjectFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RecognitionObjectFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
