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

    private setDefaultStartDate() {
        var date = new Date();
        date.setHours(date.getHours() - 1);
        this.selectedStartDate = date;
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
}
