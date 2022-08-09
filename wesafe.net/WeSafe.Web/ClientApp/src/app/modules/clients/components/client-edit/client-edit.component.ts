import { Component, OnInit } from '@angular/core';
import { ClientsService } from '../../services/clients.service';
import { IClient } from '../../models/client';
import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';

@Component({
    selector: 'app-client-edit',
    templateUrl: './client-edit.component.html',
    styleUrls: ['./client-edit.component.scss']
})
export class ClientEditComponent implements OnInit {
    client: IClient;
    pending: boolean;
    clientId: number;

    constructor(private _clientsService: ClientsService,
                private _location: Location,
                private _route: ActivatedRoute) {
    }

    ngOnInit() {
        this.clientId = this._route.snapshot.params.clientId;

        if (this.clientId) this.loadClient();
    }

    submit() {
        this.pending = true;

        this._clientsService.updateClient(this.client).subscribe(data => {
            if (data.isSuccess) this._location.back();

            this.pending = false;
        }, error => {
            this.pending = false;
        });
    }

    cancel() {
        this._location.back();
    }

    private loadClient() {
        this.pending = true;

        this._clientsService.getClientById(this.clientId).subscribe(data => {
            this.client = data;
            this.pending = false;
        }, error => {
            this.pending = false;
        });
    }
}
