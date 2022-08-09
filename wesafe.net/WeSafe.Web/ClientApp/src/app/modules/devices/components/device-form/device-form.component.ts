import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { IDevice, IDeviceType } from '../../models/device';
import { DevicesService } from '../../services/devices.service';

@Component({
    selector: 'app-device-form',
    templateUrl: './device-form.component.html',
    styleUrls: ['./device-form.component.scss']
})
export class DeviceFormComponent implements OnInit {
    @Input()
    device: IDevice;

    @Input()
    create: boolean;

    @Input()
    pending: boolean;

    @Output()
    saved = new EventEmitter<IDevice>();

    @Output()
    cancelled = new EventEmitter();

    @Output()
    deactivated = new EventEmitter<number>();

    @Output()
    resetAuth = new EventEmitter<number>();

    deviceTypes: Array<IDeviceType> = [];
    apiToken: string;

    constructor(private _devicesService: DevicesService ) {
    }

    ngOnInit() {
        this.loadDeviceTypes();
    }

    submit() {
        if (this.device.macAddress) this.device.macAddress = this.device.macAddress.toLowerCase();
        this.saved.emit(this.device);
    }

    cancel() {
        this.cancelled.emit();
    }

    deactivate() {
        if (this.deactivated) this.deactivated.emit(this.device.id);
    }

    onResetAuth() {
        if (this.resetAuth) this.resetAuth.emit(this.device.id);
    }

    loadDeviceTypes() {
        this._devicesService.getDeviceTypes().subscribe(data => {
                this.deviceTypes = Object.keys(data).map(k => {
                    return { 'id': parseInt(k), 'value': data[k] };
                });
            },
            error => {
            });
    }

    getApiToken() {
        this._devicesService.getApiToken(this.device.id).subscribe(data => {
            this.apiToken = data.accessToken;
        });
    }
}
