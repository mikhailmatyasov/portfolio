import { Component, OnInit } from '@angular/core';
import { IUpsertUser, UpsertUser } from '../../models/user';
import { UsersService } from '../../services/users.service';
import { Router } from '@angular/router';
import { Location } from '@angular/common';
import { Roles } from '../../../../models/auth';

@Component({
    selector: 'app-user-create',
    templateUrl: './user-create.component.html',
    styleUrls: ['./user-create.component.scss']
})
export class UserCreateComponent implements OnInit {
    user = new UpsertUser();
    pending: boolean;
    error: string;

    constructor(private _userService: UsersService,
                private _router: Router,
                private _location: Location) {
    }

    ngOnInit() {
        this.user.roleName = Roles.Users;
        this.user.isActive = true;
    }

    save(model: IUpsertUser) {
        this.pending = true;
        this.error = null;

        this._userService.createUser(model).subscribe(data => {
            this.pending = false;

            if (data.isSuccess) this._location.back();
            else {
                this.error = data.errors[0];
            }
        }, error => {
            this.pending = false;
        });
    }

    cancel() {
        this._location.back();
    }
}
