import {
    AbstractControl, AsyncValidator,
    NG_ASYNC_VALIDATORS,
    ValidationErrors, Validator, FormControl, AsyncValidatorFn
} from '@angular/forms';
import { Directive } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthService } from "../services/auth.service";
import { map } from 'rxjs/operators';

const getLoginStatus = (authService: AuthService, value: string) :Observable<any> => {
    return authService.getLoginStatus(value).subscribe(data => {
            if (data.isSuccess === true) {
                return null;
            } else {
                return { loginValidator: 'This login already exists. Please, change it.' }
            }
        },
        error => {
            return { loginValidator: 'Invalid request' }
        }) as any;
};

@Directive({
    selector: '[loginValidator]',
    providers: [
        { provide: NG_ASYNC_VALIDATORS, useExisting: LoginValidator, multi: true }
    ]
})
export class LoginValidator implements AsyncValidator {

    constructor(private _authService: AuthService) { }

    validate(control: AbstractControl): Observable<ValidationErrors | null> {
        return getLoginStatus(this._authService, control.value) as any;
    }
}

export function loginAsyncValidator(authService: AuthService): AsyncValidatorFn {
    return (control: AbstractControl): Promise<ValidationErrors | null> | Observable<ValidationErrors | null> => {
        return authService.getLoginStatus(control.value).pipe(map(data => {
            if (data.isSuccess === true) {
                return null;
            } else {
                return { loginValidator: 'This login already exists. Please, change it.' }
            }
        }));
    };
}
