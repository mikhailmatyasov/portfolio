import { Component, OnInit } from '@angular/core';
import { IUpsertUser, UpsertUser } from '../../models/user';
import { UsersService } from '../../services/users.service';
import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';

@Component({
    selector: 'app-user-edit',
    templateUrl: './user-edit.component.html',
    styleUrls: ['./user-edit.component.scss']
})
export class UserEditComponent implements OnInit {
    user = new UpsertUser();
    pending: boolean;
    error: string;

    constructor(private _userService: UsersService,
                private _location: Location,
                private _route: ActivatedRoute) {
    }

    ngOnInit() {
        this.pending = true;

        this._userService.getUserById(this._route.snapshot.params.userId).subscribe(data => {
            if (!data) {
                this.cancel();
                return;
            }

            this.user = new UpsertUser(data);
        });
    }

    save(model: IUpsertUser) {
        this.pending = true;
        this.error = null;

        this._userService.updateUser(model).subscribe(data => {
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
