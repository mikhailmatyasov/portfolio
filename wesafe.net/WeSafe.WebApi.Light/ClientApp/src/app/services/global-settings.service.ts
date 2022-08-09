import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { IGlobalSettings } from '../models/global-settings';
import { environment } from '../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class GlobalSettingsService {
    constructor(private _http: HttpClient) {
    }

    public getSettings(): Observable<IGlobalSettings> {
        return this._http.get<IGlobalSettings>(`${environment.apiUrl}api/GlobalSettings`);
    }

    public updateSettings(model: IGlobalSettings): Observable<any> {
        return this._http.post<any>(`${environment.apiUrl}api/GlobalSettings`, model);
    }
}
