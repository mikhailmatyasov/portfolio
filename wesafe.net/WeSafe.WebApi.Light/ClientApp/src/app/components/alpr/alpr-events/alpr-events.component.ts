import { Component, OnInit, Input } from '@angular/core';
import { AlprService } from '../../../services/alpr.service';
import { NgbInputDatepicker } from '@ng-bootstrap/ng-bootstrap';

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

    constructor(private _alprService: AlprService) { }

    ngOnInit() {
        this.setDefaultStartDate();
        this.loadEvents();
    }

    loadPage() {
    }

    loadEvents() {
        this._alprService.getEvents(this.deviceId, this.selectedStartDate, this.selectedEndDate, this.searchPlateNumber).subscribe(
            response => {
                this.rows = response;
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
        var date = new Date();
        date.setHours(date.getHours() - 1);
        this.selectedStartDate = date;
    }

}
