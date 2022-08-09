import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../../services/auth.service';
import { Router } from '@angular/router';

@Component({
    selector: 'user-info',
    templateUrl: 'user-info.component.html',
    styleUrls: ['./user-info.component.scss']
})

export class UserInfoComponent implements OnInit {
    constructor(public authService: AuthService,
                private _router: Router) {
    }

    ngOnInit() {
    }

    logout() {
        this.authService.logout();
        this._router.navigate(['login']);
    }
}
