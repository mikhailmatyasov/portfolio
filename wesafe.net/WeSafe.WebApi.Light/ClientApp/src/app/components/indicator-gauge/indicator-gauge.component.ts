import { Component, Input, OnChanges, OnInit, SimpleChanges, ViewEncapsulation } from '@angular/core';
import { IDeviceIndicators } from '../../models/device-indicators';
import { ChartType } from 'angular-google-charts';

@Component({
    selector: 'app-indicator-gauge',
    templateUrl: './indicator-gauge.component.html',
    styleUrls: ['./indicator-gauge.component.scss'],
    encapsulation: ViewEncapsulation.None
})
export class IndicatorGaugeComponent implements OnInit, OnChanges {
    @Input()
    data: Array<IDeviceIndicators> = [];

    @Input()
    field: string;

    title: string = '';
    chartType = ChartType.Gauge;
    values = [];

    options = {
        redFrom: 90, redTo: 100,
        yellowFrom:75, yellowTo: 90,
        minorTicks: 5
    };

    private titles = {
        'cpuUtilization': 'CPU utilization',
        'gpuUtilization': 'GPU utilization',
        'memoryUtilization': 'Memory utilization',
        'temperature': 'GPU temperature'
    };

    constructor() {
    }

    ngOnInit() {
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (changes.field && changes.field.currentValue) {
            this.title = this.titles[this.field];
        }
        if (changes.data && changes.data.currentValue && this.data.length > 0) {
            this.values = [
                [this.field === 'temperature' ? 'C' : '%', this.data[this.data.length - 1][this.field]]
            ];
        }
    }
}
