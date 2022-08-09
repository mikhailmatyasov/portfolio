import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CamerasListComponent } from './cameras-list.component';

describe('CamerasListComponent', () => {
    let component: CamerasListComponent;
    let fixture: ComponentFixture<CamerasListComponent>;

    beforeEach(async(() => {
        TestBed.configureTestingModule({
                   declarations: [CamerasListComponent]
               })
               .compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(CamerasListComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
