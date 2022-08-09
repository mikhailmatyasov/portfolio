import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { IUpsertUser } from '../../models/user';
import { Roles } from '../../../../models/auth';

@Component({
    selector: 'app-user-form',
    templateUrl: './user-form.component.html',
    styleUrls: ['./user-form.component.scss']
})
export class UserFormComponent implements OnInit {
    @Input()
    user: IUpsertUser;

    @Input()
    create: boolean;

    @Input()
    error: string;

    @Input()
    pending: boolean;

    @Output()
    saved = new EventEmitter<IUpsertUser>();

    @Output()
    cancelled = new EventEmitter();

    roles: Array<string> = [Roles.Users, Roles.Administrators];
    showPassword: boolean;
    userRole = Roles.Users;
    confirmPassword: string;

    constructor() {
    }

    ngOnInit() {
    }

    submit() {
        this.user.phone = "+" + this.user.phone;
        this.saved.emit(this.user);
    }

    cancel() {
        this.cancelled.emit();
    }
}
