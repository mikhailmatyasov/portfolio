import { Component, Input, OnInit } from '@angular/core';

@Component({
    selector: 'app-traffic-group',
    templateUrl: './traffic-group.component.html',
    styleUrls: ['./traffic-group.component.scss']
})
export class TrafficGroupComponent implements OnInit {
    @Input()
    deviceId;

    constructor() {
    }

    ngOnInit() {
    }
}
