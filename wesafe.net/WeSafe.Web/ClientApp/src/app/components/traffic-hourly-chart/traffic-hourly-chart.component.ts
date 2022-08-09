import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { TrafficService } from '../../services/traffic.service';
import { PrivateClientService } from '../../services/private-client.service';
import { ICamera } from '../../models/camera';
import { TrafficChart } from '../../models/traffic-chart';
import { ChartType } from 'angular-google-charts';

@Component({
    selector: 'app-traffic-hourly-chart',
    templateUrl: './traffic-hourly-chart.component.html',
    styleUrls: ['./traffic-hourly-chart.component.scss']
})
export class TrafficHourlyChartComponent implements OnInit, OnChanges {
    @Input()
    deviceId;

    cameras: Array<ICamera> = [];
    selectedCameraId: number = 0;
    selectedDate: Date;
    loading = false;
    chartItems: TrafficChart[] = [];
    chartType = ChartType.Line;
    chartColumns = ['Hour', 'Entered', 'Exited'];
    values = [];

    options = {
        vAxis: {
            title: 'Count'
        }
    };

    constructor(private _trafficService: TrafficService,
                private _clientService: PrivateClientService) {
    }

    ngOnInit() {
        this.selectedDate = new Date();
        this.loadCameras();
    }

    ngOnChanges(changes: SimpleChanges): void {
    }

    dateChanged(event) {
        this.loadChart();
    }

    cameraChanged(event) {
        this.loadChart();
    }

    private loadCameras() {
        this._clientService.getDeviceCameras(this.deviceId, null).subscribe(data => {
            this.cameras = data.items;

            if (this.cameras && this.cameras.length > 0) {
                this.selectedCameraId = this.cameras[0].id;
                this.loadChart();
            }
        }, error => {
        });
    }

    private loadChart() {
        this.loading = true;
        this._trafficService.getHourlyChart(this.deviceId, this.selectedCameraId, this.selectedDate).subscribe(data => {
            this.chartItems = data;
            this.values = [
                {
                    "name": "Entered",
                    "series": this.chartItems.map(item => {
                        return { name: item.mark, value: item.entered };
                    })
                },
                {
                    "name": "Exited",
                    "series": this.chartItems.map(item => {
                        return { name: item.mark, value: item.exited };
                    })
                }
            ];
            this.loading = false;
        }, err => this.loading = false);
    }
}
