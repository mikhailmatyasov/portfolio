import { Component, OnInit } from '@angular/core';
import { Device, IDevice } from '../../models/device';
import { DevicesService } from '../../services/devices.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Location } from '@angular/common';

@Component({
    selector: 'app-device-edit',
    templateUrl: './device-edit.component.html',
    styleUrls: ['./device-edit.component.scss']
})
export class DeviceEditComponent implements OnInit {
    device = new Device();
    pending: boolean;
    error: string;

    constructor(private _deviceService: DevicesService,
                private _location: Location,
                private _router: Router,
                private _route: ActivatedRoute) {
    }

    ngOnInit() {
        this.pending = true;

        this._deviceService.getDeviceById(this._route.snapshot.params.deviceId).subscribe(data => {
            if (!data) {
                this.cancel();
                return;
            }

            this.device = new Device(data);
        });
    }

    save(model: IDevice) {
        this.pending = true;
        this.error = null;

        this._deviceService.updateDevice(model).subscribe(data => {
            this.pending = false;
            this._router.navigate(['..'], { relativeTo: this._route });
        }, error => {
            this.error = error.error ? (error.error.ErrorMessage || 'Error') : 'Error';
            this.pending = false;
        });
    }

    deactivate(id: number) {
        this.pending = true;
        this.error = null;

        this._deviceService.deactivateDevice(id).subscribe(data => {
            this.pending = false;

            if (data.isSuccess) this._router.navigate(['..'], { relativeTo: this._route });
        }, error => {
            this.pending = false;
        });
    }

    resetAuth(id: number) {
        this.pending = true;
        this.error = null;

        this._deviceService.resetAuthDevice(id).subscribe(data => {
            this.pending = false;

            this._router.navigate(['..'], { relativeTo: this._route });
        }, error => {
            this.pending = false;
        });
    }

    cancel() {
        // this._router.navigate(['..'], { relativeTo: this._route });
        this._location.back();
    }
}
