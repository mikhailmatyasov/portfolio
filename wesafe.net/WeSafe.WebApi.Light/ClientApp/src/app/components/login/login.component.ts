import { Component, OnInit } from '@angular/core';
import { LoginModel, Roles } from '../../models/auth';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { FormGroup, FormControl, Validators } from '@angular/forms';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['../login-home/login-home.component.scss', './login.component.scss']
})
export class LoginComponent implements OnInit {
    pending: boolean;
    error: boolean;
    errorMessage: string;
    private model = new LoginModel();
    formGroup: FormGroup;

    constructor(private _authService: AuthService,
                private _router: Router) {
    }

    ngOnInit() {
        this.formGroup = new FormGroup({
            userName: new FormControl('',
                Validators.compose([Validators.required])),
            password: new FormControl('',
                Validators.compose([Validators.required]))
        });
    }

    submit() {
        this.pending = true;
        this.model = this.formGroup.value;

        this._authService.login(this.model).subscribe(res => {
            this.pending = false;
            this.error = false;

            if (res.role === Roles.Administrators || res.role === Roles.Operators) this._router.navigateByUrl('/admin');
            else this._router.navigateByUrl('/devices');
        }, error => {
                this.error = true;
                this.errorMessage = error.error.messageError;
                this.pending = false;
        });
    }
}
