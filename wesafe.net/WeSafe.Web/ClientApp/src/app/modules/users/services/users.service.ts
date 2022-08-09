import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Page } from '../../../models/page';
import { Observable } from 'rxjs';
import { PageResponse } from '../../../models/page-response';
import { IUpsertUser, IUser } from '../models/user';
import { environment } from '../../../../environments/environment';

@Injectable()
export class UsersService {

    constructor(private _http: HttpClient) {
    }

    public getUsers(page: Page, sort: string = null): Observable<PageResponse<IUser>> {
        let url = `${environment.apiUrl}api/users?skip=${(page.pageNumber - 1) * page.size}`;

        if (page.size) url += `&take=${page.size}`;
        if (sort) url += `&sort=${sort}`;

        return this._http.get<PageResponse<IUser>>(url);
    }

    public getUserById(userId: string): Observable<IUser> {
        const url = `${environment.apiUrl}api/users/${userId}`;

        return this._http.get<IUser>(url);
    }

    public createUser(user: IUpsertUser) {
        return this._http.post<any>(`${environment.apiUrl}api/users`, user);
    }

    public updateUser(user: IUpsertUser) {
        return this._http.put<any>(`${environment.apiUrl}api/users`, user);
    }

    public removeUser(userId: string) {
        const url = `${environment.apiUrl}api/users/${userId}`;

        return this._http.delete<any>(url);
    }

    public unlockUser(userId: string) {
        return this._http.post<any>(`${environment.apiUrl}api/users/${userId}/unlock`, {});
    }
}
