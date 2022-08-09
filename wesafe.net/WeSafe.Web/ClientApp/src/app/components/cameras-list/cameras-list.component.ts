import { Component, Input, OnChanges, OnInit, SimpleChanges, ViewEncapsulation } from '@angular/core';

@Component({
    selector: 'app-cameras-list',
    templateUrl: './cameras-list.component.html',
    styleUrls: ['./cameras-list.component.scss'],
    encapsulation: ViewEncapsulation.None
})
export class CamerasListComponent implements OnInit, OnChanges {
    @Input()
    deviceId: number;

    constructor() {}

    ngOnInit() {
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (changes && changes.deviceId) {
        }
    }
}
