import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ICameraType, ICameraVendor } from '../models/camera-type';
import { environment } from '../../environments/environment';

@Injectable()
export class CameraTypeService {
    constructor(private _http: HttpClient) {
    }

    public getVendors(showInactive: boolean = false): Observable<Array<ICameraVendor>> {
        return this._http.get<Array<ICameraVendor>>(`${environment.apiUrl}api/camerasvendors?showInactive=${showInactive}`);
    }

    public createVendor(vendor: ICameraVendor): Observable<any> {
        return this._http.post<any>(`${environment.apiUrl}api/camerasvendors`, vendor);
    }

    public updateVendor(vendor: ICameraVendor): Observable<any> {
        return this._http.put<any>(`${environment.apiUrl}api/camerasvendors`, vendor);
    }

    public removeVendor(vendorId: number): Observable<any> {
        const url = `${environment.apiUrl}api/camerasvendors/${vendorId}`;

        return this._http.delete<any>(url);
    }

    public getCameraTypes(vendorId: number, showInactive: boolean = false): Observable<Array<ICameraType>> {
        return this._http.get<Array<ICameraType>>(`${environment.apiUrl}api/camerasvendors/${vendorId}/types?showInactive=${showInactive}`);
    }

    public getCameraType(id: number): Observable<ICameraType> {
        return this._http.get<ICameraType>(`${environment.apiUrl}api/camerasvendors/types/${id}`);
    }

    public createCameraType(vendorId: number, model: ICameraType): Observable<any> {
        return this._http.post<any>(`${environment.apiUrl}api/camerasvendors/${vendorId}/types`, model);
    }

    public updateCameraType(vendorId: number, model: ICameraType): Observable<any> {
        return this._http.put<any>(`${environment.apiUrl}api/camerasvendors/${vendorId}/types`, model);
    }

    public removeCameraType(vendorId: number, id: number): Observable<any> {
        return this._http.delete<any>(`${environment.apiUrl}api/camerasvendors/${vendorId}/types/${id}`);
    }
}
