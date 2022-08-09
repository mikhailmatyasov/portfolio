import { environment } from "../../environments/environment";
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Page } from '../models/page';

@Injectable()
export class AlprService {

    protected alprApi = "api/plateEvent";
    protected alprUrl = environment.apiUrl + this.alprApi;

    constructor(private _http: HttpClient) {
    }

    getEvents(page: Page, deviceId: number, startDate: Date, endDate: Date, plateNumber: string) {
        let url = `${this.alprUrl}/${deviceId}?skip=${(page.pageNumber - 1) * page.size}&take=${page.size}`;

        if (startDate) url += `&startDateTime=${startDate.toISOString()}`;
        if (endDate) url += `&endDateTime=${endDate.toISOString()}`;

        if (plateNumber) {
            url += "&plateNumber=" + plateNumber.toLocaleLowerCase();
        }

        return this._http.get<any>(url);
    }
}
