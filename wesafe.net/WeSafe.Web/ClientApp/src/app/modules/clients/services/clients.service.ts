import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Page } from '../../../models/page';
import { Observable } from 'rxjs';
import { PageResponse } from '../../../models/page-response';
import { IDevice } from '../../devices/models/device';
import { environment } from '../../../../environments/environment';
import { IClient } from '../models/client';

@Injectable()
export class ClientsService {
    constructor(private _http: HttpClient) {
    }

    public getClients(page: Page, sort: string = null, search: string = null): Observable<PageResponse<IClient>> {
        let url = `${environment.apiUrl}api/clients?skip=${(page.pageNumber - 1) * page.size}`;

        if (page.size) url += `&take=${page.size}`;
        if (sort) url += `&sort=${sort}`;
        if (search) url += `&search=${search}`;

        return this._http.get<PageResponse<IClient>>(url);
    }

    public getClientById(clientId: number): Observable<IClient> {
        const url = `${environment.apiUrl}api/clients/${clientId}`;

        return this._http.get<IClient>(url);
    }

    public getClientDevices(clientId: number): Observable<Array<IDevice>> {
        const url = `${environment.apiUrl}api/clients/${clientId}/devices`;

        return this._http.get<Array<IDevice>>(url);
    }

    public updateClient(client: IClient) {
        return this._http.put<any>(`${environment.apiUrl}api/clients`, client);
    }
}
