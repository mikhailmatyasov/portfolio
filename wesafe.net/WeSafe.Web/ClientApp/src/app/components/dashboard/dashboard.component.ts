import { Component, OnInit } from '@angular/core';
import { ActivatedRoute} from '@angular/router';
import { DeviceIndicatorsService } from '../../services/device-indicators.service';
import { IDeviceIndicators } from '../../models/device-indicators';
import { PrivateClientService } from '../../services/private-client.service';

@Component({
    selector: 'app-dashboard',
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
    deviceId: number;
    deviceType: number;
    indicators: Array<IDeviceIndicators> = [];

    constructor(
        private _route: ActivatedRoute,
        private _indicatorsService: DeviceIndicatorsService,
        private _clientService: PrivateClientService) {
    }

    ngOnInit() {
        this.deviceId = this._route.snapshot.params.deviceId;
        this._clientService.getDeviceById(this.deviceId).subscribe(data => {
                this.deviceType = data.deviceType;
            },
            error => {
            });
        this.loadIndicators(this.deviceId);
    }

    private loadIndicators(deviceId: number) {
        const now = new Date();

        this._indicatorsService.getIndicators(deviceId, new Date(now.getTime() - 60000), null).subscribe(data => {
            this.indicators = data;
        });
    }
}
