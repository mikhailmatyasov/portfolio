import { Component, OnInit, Input } from '@angular/core';
import { IDevice } from "../../models/device";
import { PrivateClientService } from "../../services/private-client.service";
import { EditDeviceNameComponent } from "./modals/edit-device-name/edit-device-name.component";
import { MatDialog } from '@angular/material';

@Component({
    selector: 'app-device-info',
    templateUrl: './device-info.component.html',
    styleUrls: ['./device-info.component.scss']
})
export class DeviceInfoComponent implements OnInit {
    @Input()
    deviceId;

    device: IDevice;
    pending: boolean;
    changeDeviceName: boolean = false;

    constructor(private _clientService: PrivateClientService,
        private dialog: MatDialog) {
    }

    ngOnInit() {
        this.loadDevice();
    }

    loadDevice() {
        this._clientService.getDeviceById(this.deviceId).subscribe(data => {
                this.device = data;
            },
            error => {
            });
    }

    editDeviceName() {
        const dialogRef = this.dialog.open(EditDeviceNameComponent,
            {
                panelClass: 'dialog',
                data: { deviceName: this.device.name }
            });

        dialogRef.afterClosed().subscribe(result => {
            if (result && result.deviceName !== this.device.name) {
                this.updateDeviceName(result.deviceName);
            }

        });
    }


    private updateDeviceName(newDeviceName: string) {
        this.changeDeviceName = false;
        this._clientService.updateDeviceName(this.deviceId, newDeviceName).subscribe(data => {
                this.device.name = newDeviceName;
            },
            error => {
            });
    }

    getDeviceState(state: boolean) {
        return state ? "Armed" : "Disarmed";
    }
}
