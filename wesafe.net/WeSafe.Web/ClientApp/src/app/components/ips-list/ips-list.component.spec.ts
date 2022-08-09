import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { IpsListComponent } from './ips-list.component';

describe('IpsListComponent', () => {
  let component: IpsListComponent;
  let fixture: ComponentFixture<IpsListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ IpsListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(IpsListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
