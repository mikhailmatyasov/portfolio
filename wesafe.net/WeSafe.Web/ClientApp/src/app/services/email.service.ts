import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { IEmail } from '../models/email';


@Injectable()
export class EmailService {

    protected emailApi = "api/email";
    protected emailUrl = environment.apiUrl + this.emailApi;

    constructor(private _http: HttpClient) {
    }

    getEmails() {
        return this._http.get<Array<IEmail>>(this.emailUrl);
    }

    createEmail(email: IEmail) {
        return this._http.post<any>(this.emailUrl, email);
    }

    deleteEmail(id: number) {
        return this._http.delete<any>(`${this.emailUrl}/${id}`);
    }

    changeNotifyServerException(id: number) {
        return this._http.post<any>(`${this.emailUrl}/${id}`, {});
    }
}
