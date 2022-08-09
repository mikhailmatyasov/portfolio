import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { IPermittedAdminIp } from '../models/permitted-admin-ip';

@Injectable()
export class PermitedAdminIpService {
    constructor(private _http: HttpClient) {
    }

    getIps() {
        return this._http.get<Array<IPermittedAdminIp>>(environment.apiUrl + 'api/permittedAdminIp');
    }

    createIp(ip: IPermittedAdminIp) {
        return this._http.post<any>(environment.apiUrl + `api/permittedAdminIp`, ip);
    }

    deleteIp(id: number) {
        return this._http.delete<any>(environment.apiUrl + `api/permittedAdminIp/${id}`);
    }
}
