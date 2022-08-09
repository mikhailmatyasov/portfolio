import { Component, OnInit, Input } from '@angular/core';
import { NgbInputDatepicker } from '@ng-bootstrap/ng-bootstrap';
import { TrafficService } from "../../services/traffic.service";

@Component({
  selector: 'app-traffic',
  templateUrl: './traffic.component.html',
  styleUrls: ['./traffic.component.scss']
})
export class TrafficComponent implements OnInit {
    @Input()
    deviceId;

    rows: Array<any> = [];
    selectedStartDate: Date;
    selectedEndDate: Date;

    constructor(private _trafficService: TrafficService) { }

    ngOnInit() {
        this.setDefaultStartDate();
        this.loadEvents();
    }

    loadPage() {
        this.loadEvents();
    }

    loadEvents() {
        this._trafficService.getTraffic(this.deviceId, this.selectedStartDate, this.selectedEndDate).subscribe(
            response => {
                this.rows = response.traffic;
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
