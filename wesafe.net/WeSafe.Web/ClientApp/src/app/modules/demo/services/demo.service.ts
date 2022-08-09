import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ICamera } from '../../../models/camera';
import { environment } from '../../../../environments/environment';
import { IPageResponse } from '../../../models/page-response';
import { IEvent, IEventFilter } from '../../../models/event';
import { Page } from '../../../models/page';

@Injectable({
    providedIn: 'root'
})
export class DemoService {
    constructor(private _http: HttpClient) {
    }

    getCameras() {
        return this._http.get<Array<ICamera>>(environment.apiUrl + `api/demo/cameras`);
    }

    getEvents(page: Page, filter: IEventFilter) {
        let url = `${environment.apiUrl}api/demo/events?skip=${(page.pageNumber - 1) * page.size}&take=${page.size}`;

        if (filter.deviceId) url += `&deviceId=${filter.deviceId}`;
        if (filter.cameraId) url += `&cameraId=${filter.cameraId}`;
        if (filter.fromDate) url += `&fromDate=${filter.fromDate}`;
        if (filter.toDate) url += `&toDate=${filter.toDate}`;

        return this._http.get<IPageResponse<IEvent>>(url);
    }
}
