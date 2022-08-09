import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { IDeviceLog, DeviceLogFilter } from '../models/device-log';
import { PageResponse } from '../models/page-response';
import { Observable } from 'rxjs';
import { Page } from '../models/page';
import { AuthService } from './auth.service';

@Injectable()
export class DevicesLogsService {
    constructor(private _http: HttpClient, private _authService: AuthService) {
    }

    getDevicesLogs(page: Page, filter: DeviceLogFilter): Observable<PageResponse<IDeviceLog>> {
        let url = `${environment.apiUrl}api/deviceLog?skip=${(page.pageNumber - 1) * page.size}&take=${page.size}`;
        url += this.getFilterQueryUrl(filter);

        return this._http.get<PageResponse<IDeviceLog>>(url);
    }

    insertDevicesLogs() {
        return this._http.post<any>(environment.apiUrl + `api/deviceLog/insertLogs`, {});
    }

    getDevicesLogLevels() {
        return this._http.get<any>(environment.apiUrl + `api/deviceLog/logLevels`);
    }

    getDownloadLogsUrl(filter: DeviceLogFilter) {
        let url = `${environment.apiUrl}api/deviceLog/downloadLogs?access_token=${this._authService.getAccessToken()}`;
        url += this.getFilterQueryUrl(filter);

        return url;
    }

    private getFilterQueryUrl(filter: DeviceLogFilter) {
        let url = "";

        if (filter.logLevels) {
            filter.logLevels.forEach((item) => {
                url += `&logLevels=${item}`;
            });
        }
        if (filter.clientId) url += `&clientId=${filter.clientId}`;
        if (filter.deviceId) url += `&deviceId=${filter.deviceId}`;
        if (filter.cameraId) url += `&cameraId=${filter.cameraId}`;
        if (filter.fromDate) url += `&fromDate=${filter.fromDate}`;
        if (filter.toDate) url += `&toDate=${filter.toDate}`;
        if (filter.searchText) url += `&searchText=${filter.searchText}`;

        return url;
    }
}
