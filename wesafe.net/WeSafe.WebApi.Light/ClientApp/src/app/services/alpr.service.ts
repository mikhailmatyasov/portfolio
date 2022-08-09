import { environment } from "../../environments/environment";
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class AlprService {

    protected alprApi = "api/plateEvent";
    protected alprUrl = environment.apiUrl + this.alprApi;

    constructor(private _http: HttpClient) {
    }

    getEvents(deviceId: number, startDate: Date, endDate: Date, plateNumber: string) {
        let url = this.alprUrl + "/" + deviceId + "?StartDateTime=" + startDate.toISOString() + "&EndDateTime=" + endDate.toISOString();

        if (plateNumber) {
            url = url + "&plateNumber=" + plateNumber.toLocaleLowerCase();
        }

        return this._http.get<any>(url);
    }
}
