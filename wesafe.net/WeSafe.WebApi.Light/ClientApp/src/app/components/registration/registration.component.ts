import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { SignUpModel } from '../../models/auth';
import { Router } from '@angular/router';
import { RegistrationCodeService } from '../../services/registration-code.service';
import { deviceTokenPattern, phoneNumberPattern } from '../../../patterns/regex-patterns';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { loginAsyncValidator } from '../../validators/login-validator';
import { mustMatch } from '../../validators/must-match-validator';
import { DeviceIndicatorsService } from '../../services/device-indicators.service';
import { IDeviceType } from '../../models/device';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
    styleUrls: ['../login-home/login-home.component.scss', './registration.component.scss']
})
export class RegistrationComponent implements OnInit {
    deviceTypes: Array<IDeviceType> = [];
    registrationCode: string;
    confirmPassword: string;
    pending: boolean;
    private model: SignUpModel;
    errorMessage: string;
    validLogin: boolean;
    formGroup: FormGroup;
    selectedDeviceType: number;

    constructor(private _authService: AuthService, private _devicesService: DeviceIndicatorsService,
                private _router: Router, private _registrationCode: RegistrationCodeService) {
        if (!this._registrationCode.registrationCode)
            this._router.navigateByUrl('/');
    }

    ngOnInit() {
        this.loadDeviceTypes();
        this.formGroup = new FormGroup({
            deviceToken: new FormControl(this._registrationCode.registrationCode,
                Validators.compose([Validators.required, Validators.pattern(deviceTokenPattern)])),
            deviceTypes: new FormControl(this.deviceTypes),
            userName: new FormControl('',
                Validators.compose([Validators.required, Validators.maxLength(20)]), [loginAsyncValidator(this._authService)]),
            password: new FormControl('',
                Validators.compose([Validators.required, Validators.minLength(5), Validators.maxLength(20)])),
            name: new FormControl('',
                Validators.compose([Validators.required])),
            phone: new FormControl('',
                Validators.compose([Validators.required, Validators.pattern(phoneNumberPattern)])),
            confirmPassword: new FormControl('',
                Validators.compose([Validators.required]))
        },
            {
                validators: [mustMatch('password', 'confirmPassword')]
            });
    }

    completeSignUp() {
        this.pending = true;
        this.errorMessage = null;
        this.model = this.formGroup.value;
        this.model.deviceType = this.selectedDeviceType;

        this._authService.signup(this.model).subscribe(response => {
            if (response.isSuccess === false) {
                const err = response.errors[0];

                if (err === 'LOGINEXIST') this.errorMessage = 'This login already exists. Please, change it.';
                else if (err === 'EXIST') this.errorMessage = 'Client with this phone number already exists.';
                else if (err === 'DEVICENOTFOUND' || err === 'DEVICEACTIVATED')
                    this.errorMessage = 'This registration code wrong or already activated. Please, contact us or try again.';
                else this.errorMessage = err;
            }
            else {
                this._router.navigateByUrl('/devices');
            }

            this.pending = false;
        }, error => {
            this.pending = false;
            this.errorMessage = 'Invalid request.';
        });
    }

    loadDeviceTypes() {
        this._devicesService.getDeviceTypes().subscribe(data => {
                this.deviceTypes = Object.keys(data).map(k => {
                    return { 'id': parseInt(k), 'value': data[k] };
                });
            },
            error => {
            });
    }
}
