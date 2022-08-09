import { Component, OnInit, Inject } from '@angular/core';
import { PrivateClientService } from '../../services/private-client.service';
import { IDevice } from '../../modules/devices/models/device';
import { MatDialog } from '@angular/material/dialog';
import { AuthService } from '../../services/auth.service';
import { BindDeviceDialog } from "./bind-device-dialog/bind-device-dialog.component";

const moment = require('moment-timezone');

@Component({
  selector: 'app-user-devices-list',
  templateUrl: './user-devices-list.component.html',
  styleUrls: ['./user-devices-list.component.scss']
})
export class UserDevicesListComponent implements OnInit {
    displayedColumns: string[] = ['name', 'macAddress', 'hwVersion', 'swVersion', 'token', 'assemblingDate', 'activationDate', 'camerasNumber'];
    devices: Array<IDevice> = [];
    errorMessage: string;

    constructor(public authService: AuthService, private _clientService: PrivateClientService, public dialog: MatDialog) {
    }

    ngOnInit() {
        this.loadDevices();
    }

    openDialog(): void {
        const dialogRef = this.dialog.open(BindDeviceDialog, {
            panelClass: 'dialog'
        });

        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.bindDeviceToClient(result);
            }

        });
    }

    loadDevices() {
        this._clientService.getDevices().subscribe(data => {
            this.devices = data;
        }, error => {
        });
    }

    bindDeviceToClient(deviceToken:string) {
        this._clientService.bindtDeviceToClient(deviceToken, moment.tz.guess()).subscribe(data => {
            if (data.errors) this.errorMessage = data.errors[0];
            else this.loadDevices();

        }, error => {  });

    }

}
