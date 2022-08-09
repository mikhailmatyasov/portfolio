import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RoiEditorVer2Component } from './roi-editor-ver2.component';

describe('RoiEditorVer2Component', () => {
  let component: RoiEditorVer2Component;
  let fixture: ComponentFixture<RoiEditorVer2Component>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RoiEditorVer2Component ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RoiEditorVer2Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
