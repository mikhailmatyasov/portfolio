import { Component, OnInit } from '@angular/core';
import { Device, IDevice } from '../../models/device';
import { DevicesService } from '../../services/devices.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
    selector: 'app-device-create',
    templateUrl: './device-create.component.html',
    styleUrls: ['./device-create.component.scss']
})
export class DeviceCreateComponent implements OnInit {
    device = new Device();
    pending: boolean;
    error: string;

    constructor(private _deviceService: DevicesService,
                private _router: Router,
                private _route: ActivatedRoute) {
    }

    ngOnInit() {
        this.device.maxActiveCameras = 4;
    }

    save(model: IDevice) {
        this.pending = true;
        this.error = null;

        this._deviceService.createDevice(model).subscribe(data => {
            this.pending = false;
            this._router.navigate(['..', data.payload], { relativeTo: this._route });
        }, error => {
            this.error = error.error ? (error.error.ErrorMessage || 'Error') : 'Error';
            this.pending = false;
        });
    }

    cancel() {
        this._router.navigate(['..'], { relativeTo: this._route });
    }
}
