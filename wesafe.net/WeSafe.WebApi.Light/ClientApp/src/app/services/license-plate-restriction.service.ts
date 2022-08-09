import { environment } from "../../environments/environment";
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ILicensePlateRestriction } from '../models/licensePlateRestriction';

@Injectable()
export class LicensePlateRestrictionService {

    protected licensePlateRestrictionApi = "api/licensePlateRestriction";
    protected licensePlateRestrictionUrl = environment.apiUrl + this.licensePlateRestrictionApi;

    constructor(private _http: HttpClient) {
    }

    get(deviceId: number) {
        return this._http.get<Array<ILicensePlateRestriction>>(this.licensePlateRestrictionUrl + "/" + deviceId);
    }

    delete(id: number) {
        return this._http.delete(this.licensePlateRestrictionUrl + "/" + id);
    }

    create(deviceId: number, licensePlateRestriction: ILicensePlateRestriction) {
        return this._http.post<any>(this.licensePlateRestrictionUrl + "/" + deviceId, licensePlateRestriction);
    }

    getLicensePlateTypes() {
        return this._http.get<any>(this.licensePlateRestrictionUrl + `/licensePlateTypes`);
    }
}
