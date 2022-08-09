import { environment } from "../../environments/environment";
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TrafficChart } from '../models/traffic-chart';

@Injectable()
export class TrafficService {

    protected trafficApi = "api/traffic";
    protected trafficUrl = environment.apiUrl + this.trafficApi;

    constructor(private _http: HttpClient) {
    }

    getTraffic(deviceId: number, startDate: Date, endDate: Date) {
        let url = `${this.trafficUrl}/${deviceId}?StartDateTime=${startDate.toISOString()}`;

        if (endDate) url += "&EndDateTime=" + endDate.toISOString();

        return this._http.get<any>(url);
    }

    getHourlyChart(deviceId: number, cameraId: number, date: Date) {
        const monthString = date.getMonth() + 1 < 10 ? `0${date.getMonth() + 1}` : `${date.getMonth() + 1}`;
        const dateString = date.getDate() < 10 ? `0${date.getDate()}` : `${date.getDate()}`;

        const url = `${this.trafficUrl}/${deviceId}/hourly-chart?cameraId=${cameraId}&date=${date.getFullYear()}-${monthString}-${dateString}`;

        return this._http.get<TrafficChart[]>(url);
    }
}
