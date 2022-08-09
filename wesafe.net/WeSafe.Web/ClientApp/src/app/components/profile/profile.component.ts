import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { ProfileModel } from '../../models/auth';
import { Location } from '@angular/common';

@Component({
    selector: 'app-profile',
    templateUrl: './profile.component.html',
    styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
    pending: boolean;
    model = new ProfileModel();
    changePwd: boolean;
    error: string;

    constructor(private _service: AuthService,
                private _location: Location) {
    }

    ngOnInit() {
        this.loadProfile();
    }

    submit() {
        this.pending = true;
        this.error = null;

        this._service.updateProfile(this.model).subscribe(data => {
            if (data.isSuccess) this._location.back();
            else this.error = 'Error by updating the profile.';

            this.pending = false;
        }, error => {
            this.pending = false;
        });
    }

    cancel() {
        this._location.back();
    }

    private loadProfile() {
        this.pending = true;

        this._service.getProfile().subscribe(data => {
            this.model = data;
            this.pending = false;
        }, error => {
            this.pending = false;
        });
    }
}
