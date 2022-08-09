import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { IRecognitionObject } from '../models/recognition-object';

@Injectable({
    providedIn: 'root'
})
export class RecognitionObjectsService {
    private url = environment.apiUrl + 'api/RecognitionObjects';

    constructor(private _http: HttpClient) {
    }

    getRecognitionObjects(activeOnly: boolean = false) {
        let url = this.url;

        if (activeOnly) {
            url += '?activeOnly=true';
        }

        return this._http.get<Array<IRecognitionObject>>(url);
    }

    getRecognitionObject(id: number) {
        return this._http.get<IRecognitionObject>(`${this.url}/${id}`);
    }

    createRecognitionObject(model: IRecognitionObject) {
        return this._http.post<any>(this.url, model);
    }

    updateRecognitionObject(model: IRecognitionObject) {
        return this._http.put<any>(this.url, model);
    }
}
