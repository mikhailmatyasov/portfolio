import { Component, Input, OnInit } from '@angular/core';
import { IDevice } from '../../devices/models/device';

@Component({
    selector: 'app-device-details',
    templateUrl: './device-details.component.html',
    styleUrls: ['./device-details.component.scss']
})
export class DeviceDetailsComponent implements OnInit {
    @Input()
    device: IDevice;

    constructor() {
    }

    ngOnInit() {
    }
}
