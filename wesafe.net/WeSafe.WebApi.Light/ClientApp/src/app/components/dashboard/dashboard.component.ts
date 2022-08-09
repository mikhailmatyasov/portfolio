import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DeviceIndicatorsService } from '../../services/device-indicators.service';
import { IDeviceIndicators } from '../../models/device-indicators';
import { PrivateClientService } from '../../services/private-client.service';
import { Page } from '../../models/page';
import { EventFilter, IEvent, IEventEntry } from '../../models/event';

@Component({
    selector: 'app-dashboard',
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
    deviceId: number;
    indicators: Array<IDeviceIndicators> = [];
    lastEvent: IEvent = null;

    constructor(
        private _route: ActivatedRoute,
        private _indicatorsService: DeviceIndicatorsService,
        private _clientService: PrivateClientService) {
    }

    ngOnInit() {
        this.deviceId = this._route.snapshot.params.deviceId;
        this.loadLastEvent();
        // this.mockEvent();
    }

    private loadLastEvent() {
        this._clientService.getEvents(new Page(1, 1), new EventFilter()).subscribe(data => {
            if (data.items.length > 0) this.lastEvent = data.items[0];
        }, error => {
        });
    }

    private loadIndicators(deviceId: number) {
        const now = new Date();

        this._indicatorsService.getIndicators(deviceId, new Date(now.getTime() - 60000), null).subscribe(data => {
            this.indicators = data;
        });
    }

    private mockEvent() {
        this.lastEvent = {
            id: 1,
            deviceId: 1,
            deviceName: 'dfdf',
            cameraId: 1,
            cameraName: 'string',
            alert: false,
            parameters: null,
            message: 'Alert from camera 7! Object detected!',
            time: new Date().toISOString(),
            entries: [
                {
                    id: 1,
                    cameraLogId: 1,
                    imageUrl: 'https://storage.googleapis.com/wesafe-945da.appspot.com/637359708566795152_7712735.jpg?GoogleAccessId=firebase-adminsdk-lsfbl@wesafe-945da.iam.gserviceaccount.com&Expires=1631910056&Signature=JvXNuyDHedIPyTUg8FW9UjPEnorijX%2FMXCbWCYVaeb82qwniOfvFpa9lbC2GGU5LPDCgCy7HJwK75MVi1oBTtN0VP9Ann1lU2qr0OMbBb9pogM6nAX%2BDZPKS40A6dCXqiKG8VjF5aEdEgSIZiT%2BUz96f3X1HCj9DcmSyJhTzt%2BvDypwkc7CjHNfVkXVtWeZlbnFirWXZtRwFGjhf%2F088lUp2rLvN%2BmSQ4DpjfLsQfzjqdpSM4vp8f6I1i1Ym4SuNXnheb0okjOG23IAxHE9bhNUW7MBGYMCTaGIJDXoPJMxQl%2BAE9NoRsFgCJ2omkG%2BdXUH7ZoHdC1xQaR2VOT8kxA%3D%3D',
                    typeKey: null,
                    urlExpiration: null
                }
            ]
        };
    }
}
