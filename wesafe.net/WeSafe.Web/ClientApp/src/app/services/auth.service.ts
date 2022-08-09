import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ITokenResponse, LoginModel, ProfileModel, Roles, SignUpModel } from '../models/auth';
import { environment } from '../../environments/environment';
import { tap } from 'rxjs/operators';
import { LocalStorage } from './local-storage.service';
import { Observable } from 'rxjs';

@Injectable()
export class AuthService {
    private _authentication = {
        authenticated: false,
        userName: null,
        displayName: null,
        role: null,
        expiresAt: null,
        demo: false
    };

    constructor(private _http: HttpClient,
                private _localStorage: LocalStorage) {
        this.fillAuthentication();
    }

    public isAuthenticated() {
        return this._authentication.authenticated;
    }

    public isExpired() {
        return !this.isAuthenticated() || (new Date()) > (this._authentication.expiresAt);
    }

    public isAdministrator(): boolean {
        return this._authentication.role === Roles.Administrators;
    }

    public isOperator(): boolean {
        return this._authentication.role === Roles.Operators;
    }

    public isUser(): boolean {
        return this._authentication.role === Roles.Users;
    }

    public isDemo(): boolean {
        return this._authentication.demo;
    }

    public isInRole(role: string): boolean {
        return this._authentication.role === role;
    }

    public getAccessToken() {
        const authData = this._localStorage.getItem('authentication');

        return authData ? authData.accessToken : null;
    }

    public getUserName(): string {
        return this._authentication.userName;
    }

    public getDisplayName(): string {
        return this._authentication.displayName || this._authentication.userName;
    }

    public login(model: LoginModel) {
        return this._http
            .post<ITokenResponse>(environment.apiUrl + 'api/account/token', model)
                   .pipe(
                       tap(response => {
                           this._localStorage.setItem('authentication', response);

                           this._authentication.authenticated = true;
                           this._authentication.userName = response.userName;
                           this._authentication.displayName = response.displayName;
                           this._authentication.role = response.role;
                           this._authentication.expiresAt = new Date(response.expiresAt);
                           this._authentication.demo = response.demo;
                       })
                   );
    }

    public logout() {
        this._localStorage.remove('authentication');

        this._authentication.authenticated = false;
        this._authentication.userName = null;
        this._authentication.displayName = null;
        this._authentication.role = null;
        this._authentication.expiresAt = null;
        this._authentication.demo = false;
    }

    public getDeviceTokenStatus(token: string) {
        return this._http.post<any>(environment.apiUrl + 'api/account/token-status', { deviceToken: token });
    }

    public getLoginStatus(userName: string):Observable<any> {
        return this._http.post<any>(environment.apiUrl + 'api/account/login-status', { userName: userName });
    }

    public signup(model: SignUpModel) {
        return this._http
            .post<any>(environment.apiUrl + 'api/account/signup', model)
                   .pipe(
                       tap(response => {
                           if (response.isSuccess === false) return;

                           this._localStorage.setItem('authentication', response);

                           this._authentication.authenticated = true;
                           this._authentication.userName = response.userName;
                           this._authentication.displayName = response.displayName;
                           this._authentication.role = response.role;
                           this._authentication.expiresAt = new Date(response.expiresAt);
                           this._authentication.demo = response.demo;
                       })
                   );
    }

    public getProfile(): Observable<ProfileModel> {
        return this._http.get<ProfileModel>(environment.apiUrl + 'api/account/profile');
    }

    public updateProfile(model: ProfileModel) {
        return this._http
            .post<any>(environment.apiUrl + 'api/account/profile', model)
            .pipe(
                tap(response => {
                    if (response.isSuccess === false) return;

                    this._authentication.displayName = model.displayName;

                    const authData = this._localStorage.getItem('authentication');

                    authData.displayName = model.displayName;

                    this._localStorage.setItem('authentication', authData);
                })
            );
    }

    private fillAuthentication() {
        const authData = this._localStorage.getItem('authentication');

        if (authData && (new Date()) < new Date(authData.expiresAt)) {
            this._authentication.authenticated = true;
            this._authentication.userName = authData.userName;
            this._authentication.displayName = authData.displayName;
            this._authentication.role = authData.role;
            this._authentication.expiresAt = new Date(authData.expiresAt);
            this._authentication.demo = authData.demo;
        }
    }
}
