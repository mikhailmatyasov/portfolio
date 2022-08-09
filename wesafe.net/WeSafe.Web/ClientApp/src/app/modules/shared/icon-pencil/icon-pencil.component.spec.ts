import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { IconPencilComponent } from './icon-pencil.component';

describe('IconPencilComponent', () => {
    let component: IconPencilComponent;
    let fixture: ComponentFixture<IconPencilComponent>;

    beforeEach(async(() => {
        TestBed.configureTestingModule({
                   declarations: [IconPencilComponent]
               })
               .compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(IconPencilComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
