import { Component, Input, OnChanges, OnInit, SimpleChanges, ViewEncapsulation } from '@angular/core';
import { PrivateClientService } from '../../services/private-client.service';
import { AddAlertUserComponent } from "./modals/add-alert-user/add-alert-user.component";
import { MatDialog } from "@angular/material";
import { ConfirmationModalComponent } from "../confirmation-modal/confirmation-modal.component";
import { ConfirmationModel } from '../confirmation-modal/models/confirmation';
import { AssignmentModalComponent } from "../assignment-modal/assignment-modal.component";
import { IClientSubscriber } from "../../models/client-subscriber";

@Component({
    selector: 'app-alert-users-list',
    templateUrl: './alert-users-list.component.html',
    styleUrls: ['./alert-users-list.component.scss'],
    encapsulation: ViewEncapsulation.None
})
export class AlertUsersListComponent implements OnInit, OnChanges {
    @Input()
    deviceId: number;

    users: Array<IClientSubscriber> = [];
    pending: boolean;
    adding: boolean;

    errorMessage: string;

    constructor(private _clientService: PrivateClientService, private dialog: MatDialog) {
    }

    ngOnInit() {
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (changes && changes.deviceId) this.loadUsers();
    }

    loadUsers() {
        this.pending = true;

        this._clientService.getSubscribers().subscribe(data => {
            this.users = data;
            this.pending = false;
        }, error => {
            this.pending = false;
        });
    }

    addUser() {
        const dialogRef = this.dialog.open(AddAlertUserComponent, {
            panelClass: 'dialog',
        });

        dialogRef.afterClosed().subscribe(result => {
            if (!result)
                return;
            this.save(result);

        });
    }

    save(newUser) {
        this.pending = true;

        this._clientService.createSubscriber(newUser).subscribe(data => {
            if (data.isSuccess) {
                this.errorMessage = null;
                this.adding = false;
                this.loadUsers();
            }
            else {
                this.pending = false;
                this.errorMessage = 'This phone already exists.';
            }
        }, error => {
            this.pending = false;
            this.errorMessage = 'Error.';
        });
    }

    cancel() {
        this.errorMessage = null;
        this.adding = false;
    }

    delete(id: number) {
        const dialogRef = this.dialog.open(ConfirmationModalComponent, {
            panelClass: 'dialog',
            data: new ConfirmationModel("Delete this telegram user?", "yes, delete", "no, cancel")
        });

        dialogRef.afterClosed().subscribe(result => {
            if (!result)
                return;

            this._clientService.deleteSubscriber(id).subscribe(data => {
                this.loadUsers();
            });

        });
    }


    open(user: IClientSubscriber) {
        const dialogRef = this.dialog.open(AssignmentModalComponent, {
            panelClass: 'dialog',
        });

        dialogRef.afterClosed().subscribe(result => {
            if (!result)
                return;
        });
        //const modalRef = this._modalService.open(AssignmentModalComponent);
        dialogRef.componentInstance.user = user;
    }
}
