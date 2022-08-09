import { Component, OnInit } from '@angular/core';
import { DemoService } from '../../services/demo.service';
import { Page } from '../../../../models/page';
import { NgbInputDatepicker } from '@ng-bootstrap/ng-bootstrap';
import { EventFilter, IEvent } from '../../../../models/event';

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

    constructor(private _demoService: DemoService) {
    }

    ngOnInit() {
        this.loadEvents();
    }

    loadPage(page: number) {
        this.page.pageNumber = page;
        this.loadEvents();
    }

    loadEvents() {
        const filter = this.getEventFilterModel();

        this._demoService.getEvents(this.page, filter).subscribe(data => {
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
}
