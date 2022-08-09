import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpEvent, HttpHandler, HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { AuthService } from './auth.service';
import { Router } from '@angular/router';
import { tap } from 'rxjs/operators';
import { AlertService } from './alert-error-popup.service';

@Injectable({ providedIn: 'root' })
export class ApiHttpInterceptor implements HttpInterceptor {
    constructor(private _authService: AuthService,
        private _router: Router,
        private _alertErrorService : AlertService) {
    }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        if (req.url.startsWith(environment.apiUrl + 'api/')) {
            if (this._authService.isAuthenticated()) req = this.handleRequest(req);

            return next.handle(req).pipe(tap(event => {}, error => this.handleErrorResponse(error)));
        }

        return next.handle(req);
    }

    private handleRequest(req: HttpRequest<any>): HttpRequest<any> {
        return req.clone({ setHeaders: { Authorization: 'Bearer ' + this._authService.getAccessToken() } });
    }

    private handleErrorResponse(error) {
        let errorText = this.getErrorText(error);
        this._alertErrorService.error(errorText);
                   
        if (error.status === 401) {
            this._authService.logout();
            this._router.navigateByUrl('/login?returnUrl=' + this._router.url);
        }
    }

    private getErrorText(error) {
        let errorText = error.statusText;
        if (error.error.ErrorMessage)
            errorText = errorText + ": " + error.error.ErrorMessage;
        if (error.error) {
            if (typeof (error) != "object")
                errorText = errorText + ": " + error.error;
        }
        return errorText;
    }
}
