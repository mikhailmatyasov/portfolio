import { environment } from "../../environments/environment";
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class TrafficService {

    protected trafficApi = "api/traffic";
    protected trafficUrl = environment.apiUrl + this.trafficApi;

    constructor(private _http: HttpClient) {
    }

    getTraffic(deviceId: number, startDate: Date, endDate: Date) {

        const url = this.trafficUrl +
            "/" +
            deviceId +
            "?StartDateTime=" +
            startDate.toISOString() +
            "&EndDateTime=" +
            endDate.toISOString();

        return this._http.get<any>(url);
    }
}
