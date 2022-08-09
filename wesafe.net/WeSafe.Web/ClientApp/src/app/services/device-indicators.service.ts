import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { IDeviceIndicators } from '../models/device-indicators';

@Injectable({
    providedIn: 'root'
})
export class DeviceIndicatorsService {
    constructor(private _http: HttpClient) {
    }

    getIndicators(deviceId: number, startDate: Date, endDate: Date) {
        let url = environment.apiUrl + `api/client/devices/${deviceId}/indicators`;

        if (startDate || endDate) url += '?';
        if (startDate) url += 'from=' + startDate.toISOString();
        if (endDate) url += (startDate ? '&' : '') + 'to=' + endDate.toISOString();

        return this._http.get<Array<IDeviceIndicators>>(url);
    }
}
