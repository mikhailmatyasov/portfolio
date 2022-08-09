import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { Camera, ICamera } from '../../models/camera';
import { PrivateClientService } from '../../services/private-client.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
    selector: 'app-camera-roi',
    templateUrl: './camera-roi.component.html',
    styleUrls: ['./camera-roi.component.scss']
})
export class CameraRoiComponent implements OnInit {
    title: string;
    camera: ICamera;
    pending: boolean;
    roi: any = {};

    public deviceId: number;
    private cameraId: number;

    constructor(private _clientService: PrivateClientService,
                private _router: Router,
                private _route: ActivatedRoute,
                private _location: Location) {
    }

    ngOnInit() {
        this.deviceId = this._route.snapshot.params.deviceId;
        this.cameraId = this._route.snapshot.params.cameraId;
        this.pending = true;

        this._clientService.getDeviceCamera(this.deviceId, this.cameraId).subscribe(data => {
            this.camera = new Camera(data);
            this.title = data.cameraName + ' ROI';
            this.pending = false;

            if (this.camera.lastImagePath) this.init();
        }, error => {
            this.pending = false;
        });
    }

    init() {
        this.roi = this.camera.roi ? JSON.parse(this.camera.roi) : {
            version: '2.0',
            areas: []
        };

        if (this.roi.version === '2.0') {
        }
        else {
        }
    }

    save(roi) {
        this.pending = true;
        this.camera.roi = JSON.stringify(roi);

        this._clientService.updateDeviceCamera(this.deviceId, this.camera).subscribe(data => {
            this._location.back();
            this.pending = false;
        }, error => {
            this.pending = false;
        });
    }

    cancel() {
        this._location.back();
    }
}
