import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { IUnhandledException, UnhandledExceptionFilter } from '../models/unhandled-exception';
import { Observable } from 'rxjs';
import { Page } from '../models/page';
import { PageResponse } from '../models/page-response';


@Injectable()
export class UnhandledExceptionService {

    protected unhandledExceptionApi = "api/unhandledException";
    protected unhandledExceptionUrl = environment.apiUrl + this.unhandledExceptionApi;

    constructor(private _http: HttpClient) {
    }

    getUnhandledExceptions(page: Page, filter: UnhandledExceptionFilter): Observable<PageResponse<IUnhandledException>> {
        let url = `${this.unhandledExceptionUrl}?skip=${(page.pageNumber - 1) * page.size}&take=${page.size}`;
        url += this.getFilterQueryUrl(filter);

        return this._http.get<PageResponse<IUnhandledException>>(url);
    }

    private getFilterQueryUrl(filter: UnhandledExceptionFilter) {
        let url = "";

        if (filter.userName) url += `&userName=${filter.userName}`;
        if (filter.fromDate) url += `&fromDate=${filter.fromDate}`;
        if (filter.toDate) url += `&toDate=${filter.toDate}`;
        if (filter.searchText) url += `&searchText=${filter.searchText}`;

        return url;
    }
}
