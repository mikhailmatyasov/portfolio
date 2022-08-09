import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RoiEditorVer1Component } from './roi-editor-ver1.component';

describe('RoiEditorVer1Component', () => {
  let component: RoiEditorVer1Component;
  let fixture: ComponentFixture<RoiEditorVer1Component>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RoiEditorVer1Component ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RoiEditorVer1Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
