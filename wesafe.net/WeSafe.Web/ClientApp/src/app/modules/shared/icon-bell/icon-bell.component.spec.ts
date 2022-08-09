import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { IconBellComponent } from './icon-bell.component';

describe('IconBellComponent', () => {
    let component: IconBellComponent;
    let fixture: ComponentFixture<IconBellComponent>;

    beforeEach(async(() => {
        TestBed.configureTestingModule({
                   declarations: [IconBellComponent]
               })
               .compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(IconBellComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
