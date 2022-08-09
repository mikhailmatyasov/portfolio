import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { IDevice } from '../models/device';
import { ICamera, ICameraStat } from '../models/camera';
import { IClientSubscriber } from '../models/client-subscriber';
import { IAssignment } from '../models/assignment';
import { IPageResponse } from '../models/page-response';
import { IEvent, IEventFilter } from '../models/event';
import { Page } from '../models/page';

@Injectable()
export class PrivateClientService {
    constructor(private _http: HttpClient) {
    }

    getDevices() {
        return this._http.get<Array<IDevice>>(environment.apiUrl + 'api/client/devices');
    }

    getDeviceById(deviceId: number) {
        return this._http.get<IDevice>(environment.apiUrl + `api/client/devices/${deviceId}`);
    }

    bindtDeviceToClient(token: string) {
        return this._http.post<any>(environment.apiUrl + `api/client/devices/${token}`, {});
    }

    updateDeviceName(deviceId: number, newDeviceName: string) {
        return this._http.post<any>(environment.apiUrl + `api/client/devices/${deviceId}/editname`, { newDeviceName: newDeviceName });
    }

    getDeviceCameras(deviceId: number) {
        return this._http.get<Array<ICamera>>(environment.apiUrl + `api/client/devices/${deviceId}/cameras`);
    }

    getDeviceCameraStat(deviceId: number) {
        return this._http.get<ICameraStat>(environment.apiUrl + `api/client/devices/${deviceId}/cameras-stat`);
    }

    getDeviceCamera(deviceId: number, id: number) {
        return this._http.get<ICamera>(environment.apiUrl + `api/client/devices/${deviceId}/cameras/${id}`);
    }

    createDeviceCamera(deviceId: number, camera: ICamera) {
        return this._http.post<any>(environment.apiUrl + `api/client/devices/${deviceId}/cameras`, camera);
    }

    updateDeviceCamera(deviceId: number, camera: ICamera) {
        return this._http.put<any>(environment.apiUrl + `api/client/devices/${deviceId}/cameras`, camera);
    }

    removeDeviceCamera(deviceId: number, id: number) {
        return this._http.delete<any>(environment.apiUrl + `api/client/devices/${deviceId}/cameras/${id}`);
    }

    getSubscribers() {
        return this._http.get<Array<IClientSubscriber>>(environment.apiUrl + 'api/client/subscribers');
    }

    createSubscriber(subscriber: IClientSubscriber) {
        return this._http.post<any>(environment.apiUrl + `api/client/subscribers`, subscriber);
    }

    deleteSubscriber(subscriberId: number) {
        return this._http.delete<any>(environment.apiUrl + `api/client/subscribers/${subscriberId}`);
    }

    getSubscriberAssignments(subscriberId: number) {
        return this._http.get<Array<IAssignment>>(environment.apiUrl + `api/client/subscribers/${subscriberId}/assignments`);
    }

    saveSubscriberAssignments(subscriberId: number, assignments: Array<IAssignment>) {
        return this._http.post<any>(environment.apiUrl + `api/client/subscribers/${subscriberId}/assignments`, assignments);
    }

    getEvents(page: Page, filter: IEventFilter) {
        let url = `${environment.apiUrl}api/client/events?skip=${(page.pageNumber - 1) * page.size}&take=${page.size}`;

        if (filter.deviceId) url += `&deviceId=${filter.deviceId}`;
        if (filter.cameraId) url += `&cameraId=${filter.cameraId}`;
        if (filter.fromDate) url += `&fromDate=${filter.fromDate}`;
        if (filter.toDate) url += `&toDate=${filter.toDate}`;

        return this._http.get<IPageResponse<IEvent>>(url);
    }
}
