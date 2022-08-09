import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, CanLoad, Route, Router, RouterStateSnapshot } from '@angular/router';
import { AuthService } from './auth.service';
import { ALL } from 'tslint/lib/rules/completedDocsRule';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate, CanLoad {
    constructor(private _authService: AuthService,
                private _router: Router) {
    }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        const anonymous = route.data.anonymous as boolean;
        const demo = route.data.demo as boolean;

        if (anonymous) return true;

        if (!this._authService.isAuthenticated() || this._authService.isExpired()) {
            this._router.navigate(['login']);

            return false;
        }

        if (!this.checkRoles(route.data.role)) {
            this._router.navigate(['login']);

            return false;
        }

        if (demo && !this._authService.isDemo()) {
            return false;
        }

        return true;
    }

    canLoad(route: Route) {
        const allowed = this.checkRoles(route.data.role);

        if (!allowed)this._router.navigate(['login']);

        return allowed;
    }

    private checkRoles(role) {
        if (!!role) {
            if (role instanceof Array) {
                for (const item of role) {
                    if (this._authService.isInRole(item)) return true;
                }

                return false;
            }

            return this._authService.isInRole(role as string);
        }

        return true;
    }
}
