import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PageResponse } from '../../../models/page-response';
import { IDevice, IDeviceFilter } from '../models/device';
import { environment } from '../../../../environments/environment';
import { Page } from '../../../models/page';
import { ICamera } from '../../../models/camera';
import { ITokenResponse } from '../../../models/auth';

@Injectable({
    providedIn: 'root'
})
export class DevicesService {
    constructor(private _http: HttpClient) {
    }

    protected devicesApi = "api/devices";
    protected devicesUrl = environment.apiUrl + this.devicesApi;

    private getFilterQueryUrl(filter: IDeviceFilter): string {

        let url = "";
        if (filter.clientId) url += `&clientId=${filter.clientId}`;
        if (filter.filterBy != null) url += `&filterBy=${filter.filterBy}`;
        if (filter.search != null) url += `&search=${filter.search}`;
        if (filter.sort) url += `&sort=${filter.sort}`;

        return url;
    }

    public getDevices(page: Page, filter: IDeviceFilter): Observable<PageResponse<IDevice>> {
        let url = `${this.devicesUrl}?skip=${(page.pageNumber - 1) * page.size}&take=${page.size}`;

        url += this.getFilterQueryUrl(filter);

        return this._http.get<PageResponse<IDevice>>(url);
    }

    public getDeviceById(deviceId: number): Observable<IDevice> {
        const url = `${this.devicesUrl}/${deviceId}`;

        return this._http.get<IDevice>(url);
    }

    public getDeviceCameras(deviceId: number): Observable<Array<ICamera>> {
        const url = `${this.devicesUrl}/${deviceId}/cameras`;

        return this._http.get<Array<ICamera>>(url);
    }

    public createDevice(device: IDevice) {
        return this._http.post<any>(`${this.devicesUrl}`, device);
    }

    public updateDevice(device: IDevice) {
        return this._http.put<any>(`${this.devicesUrl}`, device);
    }

    public removeDevice(deviceId: number) {
        const url = `${this.devicesUrl}/${deviceId}`;

        return this._http.delete<IDevice>(url);
    }

    public deactivateDevice(deviceId: number) {
        const url = `${this.devicesUrl}/${deviceId}/deactivate`;

        return this._http.post<any>(url, {});
    }

    public resetAuthDevice(deviceId: number) {
        const url = `${this.devicesUrl}/${deviceId}/resetauth`;

        return this._http.post<any>(url, {});
    }

    public getApiToken(deviceId: number): Observable<ITokenResponse> {
        const url = `${this.devicesUrl}/${deviceId}/create-api-token`;

        return this._http.get<ITokenResponse>(url);
    }

    getDeviceTypes() {
        return this._http.get<any>(this.devicesUrl + `/deviceTypes`);
    }
}
