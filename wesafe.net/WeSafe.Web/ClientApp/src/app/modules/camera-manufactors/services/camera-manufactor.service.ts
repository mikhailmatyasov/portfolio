import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from "../../../../environments/environment";
import { Observable } from 'rxjs';

@Injectable()
export class CameraManufactorService {
    constructor(private _http: HttpClient) {
    }

    public getManufactors(): Observable<Array<CameraManufactor>> {
        return this._http.get<Array<CameraManufactor>>(`${environment.apiUrl}api/cameramanufactor`);
    }
}
