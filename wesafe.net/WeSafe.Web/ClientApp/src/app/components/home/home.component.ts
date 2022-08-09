import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { RegistrationCodeService } from '../../services/registration-code.service';
import { deviceTokenPattern } from '../../../patterns/regex-patterns';
import { FormControl, Validators, FormGroup } from '@angular/forms';

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    styleUrls: ['../login-home/login-home.component.scss', './home.component.scss']
})
export class HomeComponent implements OnInit {
    pending: boolean;
    errorMessage: string;
    formGroup: FormGroup;

    constructor(private _authService: AuthService,
        private _router: Router,
        private _registrationCode: RegistrationCodeService) {
    }

    ngOnInit() {
        if (this._authService.isAuthenticated() && !this._authService.isExpired()) {
            if (this._authService.isAdministrator() || this._authService.isOperator()) this._router.navigateByUrl('/admin');
            if (this._authService.isUser() && !this._authService.isDemo()) this._router.navigateByUrl('/devices');
            if (this._authService.isUser() && this._authService.isDemo()) this._router.navigateByUrl('/demo');
        }

        this.formGroup = new FormGroup({
            deviceToken: new FormControl('',
                Validators.compose([Validators.required, Validators.pattern(deviceTokenPattern)]))
        });
    }

    submitSignUp() {
        this.pending = true;
        this.errorMessage = null;

        this._authService.getDeviceTokenStatus(this.formGroup.value.deviceToken).subscribe(data => {
            if (data.isSuccess === true) {
                this._registrationCode.registrationCode = this.formGroup.value.deviceToken;
                this._router.navigateByUrl('/registration');
            } else {
                this.errorMessage = 'This registration code wrong or already activated. Please, contact us or try again.';
            }

            this.pending = false;
        }, error => {
            this.pending = false;
                this.errorMessage = 'This registration code wrong or already activated. Please, contact us or try again.';
        });
    }
}
