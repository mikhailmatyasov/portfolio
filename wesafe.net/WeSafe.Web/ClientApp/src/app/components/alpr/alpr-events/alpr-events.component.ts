import { Component, OnInit, Input } from '@angular/core';
import { AlprService } from '../../../services/alpr.service';
import { NgbInputDatepicker } from '@ng-bootstrap/ng-bootstrap';
import { Page } from '../../../models/page';
import { PageEvent } from '@angular/material/paginator';

@Component({
  selector: 'app-alpr-events',
  templateUrl: './alpr-events.component.html',
  styleUrls: ['./alpr-events.component.scss']
})
export class AlprEventsComponent implements OnInit {

    @Input()
    deviceId;

    rows: Array<any> = [];
    selectedStartDate: Date;
    selectedEndDate: Date;
    searchPlateNumber: string;
    page = new Page(1, 25);
    pageSizeOptions: number[] = [10, 25, 50, 100];

    constructor(private _alprService: AlprService) { }

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
        this._alprService.getEvents(this.page, this.deviceId, this.selectedStartDate, this.selectedEndDate, this.searchPlateNumber).subscribe(
            response => {
                this.rows = response.items;
                this.page.total = response.total;
            });
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
        const date = new Date();

        date.setHours(date.getHours() - 24);
        this.selectedStartDate = date;
    }

}
