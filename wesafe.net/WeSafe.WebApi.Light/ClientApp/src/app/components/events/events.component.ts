import { Component, OnInit } from '@angular/core';
import { Page } from '../../models/page';
import { NgbInputDatepicker } from '@ng-bootstrap/ng-bootstrap';
import { EventFilter, IEvent } from '../../models/event';
import { PrivateClientService } from '../../services/private-client.service';
import { PageEvent } from '@angular/material/paginator';

@Component({
    selector: 'app-events',
    templateUrl: './events.component.html',
    styleUrls: ['./events.component.scss']
})
export class EventsComponent implements OnInit {
    page = new Page(1, 100);
    rows: Array<IEvent> = [];
    selectedStartDate: Date;
    selectedEndDate: Date;
    pageSizeOptions: number[] = [10, 25, 50, 100];

    constructor(private _clientService: PrivateClientService) { }

    ngOnInit() {
        this.setDefaultStartDate();
        this.loadEvents();
    }

    loadPage(page: PageEvent) {
        this.page.pageNumber = page.pageIndex + 1;
        this.page.size = page.pageSize;
        this.loadEvents();
        // this.mockEvents();
    }

    loadEvents() {
        const filter = this.getEventFilterModel();

        this._clientService.getEvents(this.page, filter).subscribe(data => {
            this.rows = data.items;
            this.page.total = data.total;
        }, error => {
        });
    }

    getEventFilterModel(): EventFilter {
        const filter = new EventFilter();

        if (this.selectedStartDate)
            filter.fromDate = this.getDateFormat(this.selectedStartDate);

        if (this.selectedEndDate)
            filter.toDate = this.getDateFormat(this.selectedEndDate);

        return filter;
    }

    private getDateFormat(datePicker: Date): string {
        return datePicker.toISOString();
    }

    clearDatePicker(datePicker: NgbInputDatepicker) {
        datePicker.close();
        datePicker.manualDateChange('', true);
        datePicker.writeValue('');
    }

    startDateTimeChanged(event) {
        this.selectedStartDate = event.value;
    }

    endDateTimeChanged(event) {
        this.selectedEndDate = event.value;
    }

    private setDefaultStartDate() {
        var date = new Date();
        date.setHours(date.getHours() - 1);
        this.selectedStartDate = date;
    }

    private mockEvents() {
        this.rows = [];
        this.page.total = 1100;

        for (let i = 0; i < 100; i++ ) {
            this.rows.push({
                id: i + 1,
                deviceId: 1,
                deviceName: 'dfdf',
                cameraId: 1,
                cameraName: 'string',
                alert: false,
                parameters: null,
                message: `Alert from camera ${i + 1}! Object detected!`,
                time: new Date().toISOString(),
                entries: [
                    {
                        id: 1,
                        cameraLogId: 1,
                        imageUrl: 'https://storage.googleapis.com/wesafe-945da.appspot.com/637359708566795152_7712735.jpg?GoogleAccessId=firebase-adminsdk-lsfbl@wesafe-945da.iam.gserviceaccount.com&Expires=1631910056&Signature=JvXNuyDHedIPyTUg8FW9UjPEnorijX%2FMXCbWCYVaeb82qwniOfvFpa9lbC2GGU5LPDCgCy7HJwK75MVi1oBTtN0VP9Ann1lU2qr0OMbBb9pogM6nAX%2BDZPKS40A6dCXqiKG8VjF5aEdEgSIZiT%2BUz96f3X1HCj9DcmSyJhTzt%2BvDypwkc7CjHNfVkXVtWeZlbnFirWXZtRwFGjhf%2F088lUp2rLvN%2BmSQ4DpjfLsQfzjqdpSM4vp8f6I1i1Ym4SuNXnheb0okjOG23IAxHE9bhNUW7MBGYMCTaGIJDXoPJMxQl%2BAE9NoRsFgCJ2omkG%2BdXUH7ZoHdC1xQaR2VOT8kxA%3D%3D',
                        typeKey: null,
                        urlExpiration: null
                    },
                    {
                        id: 2,
                        cameraLogId: 1,
                        imageUrl: 'https://storage.googleapis.com/wesafe-945da.appspot.com/637359821302691552_1839408.jpg?GoogleAccessId=firebase-adminsdk-lsfbl@wesafe-945da.iam.gserviceaccount.com&Expires=1631921330&Signature=kfZDa%2F3w%2B9Q70EDRZ6nXo91SaXfyBSoBvyL6One5l7LUysHH2k4jQlT3KcxWagTwMnsmTtciIC4RsqsC3vtv3uDeME98o%2FI%2FqStxjAGUcqAjnvDkKH%2FbFDEPRpiwZ%2B5k8flRib0Bh6dxu%2BprfRk30Yi%2FGUVO5%2BVpP48LnHocWEzXbGhMye%2BD%2FejvyKMK5EbPgxfL2FeoUXgNz508mE9FTldRLdexLb3Ejs8IQcgiOYaOdtZGWkuEVADUJeQmDDlJ4L0%2Bvpi%2B97KFrw5mEPJNP6lTXd7mPnFXIY3dEXqRHO49CywWyJyhdxNWy0I%2Ft2mAcMeLccy%2FF%2F6pSHGbuss1eg%3D%3D',
                        typeKey: null,
                        urlExpiration: null
                    }
                ]
            });
        }
    }
}
