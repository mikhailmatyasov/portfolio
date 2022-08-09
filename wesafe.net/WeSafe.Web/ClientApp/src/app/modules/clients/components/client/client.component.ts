import { Component, OnInit } from '@angular/core';
import { IClient } from '../../models/client';
import { ClientsService } from '../../services/clients.service';
import { ActivatedRoute } from '@angular/router';
import { IDevice } from '../../../devices/models/device';
import { forkJoin } from 'rxjs';
import { DevicesService } from '../../../devices/services/devices.service';
import { ICamera } from '../../../../models/camera';
import { Location } from '@angular/common';

@Component({
    selector: 'app-client',
    templateUrl: './client.component.html',
    styleUrls: ['./client.component.scss']
})
export class ClientComponent implements OnInit {
    client: IClient;
    devices: Array<IDevice> = [];
    pending: boolean;
    clientId: number;

    constructor(private _clientsService: ClientsService,
                private _deviceService: DevicesService,
                private _location: Location,
                private _route: ActivatedRoute) {
    }

    ngOnInit() {
        this.clientId = this._route.snapshot.params.clientId;

        if (this.clientId) this.loadClient();
    }

    setActive(value) {
        const old = this.client.isActive;

        this.client.isActive = value;
        this.pending = true;

        this._clientsService.updateClient(this.client).subscribe(data => {
            if (!data.isSuccess) this.client.isActive = old;
            this.pending = false;
        }, error => {
            this.client.isActive = old;
            this.pending = false;
        });
    }

    back() {
        this._location.back();
    }

    private loadClient() {
        this.pending = true;

        forkJoin([
            this._clientsService.getClientById(this.clientId),
            this._clientsService.getClientDevices(this.clientId)
        ]).subscribe(data => {
            this.client = data[0];

            if (data[1].length > 0) {
                this.devices = data[1];
                this.devices.forEach(device => {
                    this.loadCameras(device)
                });
            }

            this.pending = false;
        }, error => {
            this.pending = false;
        });
    }

    private loadCameras(device: IDevice) {
        this._deviceService.getDeviceCameras(device.id).subscribe(data => {
            device.cameras = data;
        });
    }
}
