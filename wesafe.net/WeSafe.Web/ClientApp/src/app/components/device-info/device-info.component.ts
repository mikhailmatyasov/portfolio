import { Component, OnInit, Input } from '@angular/core';
import { IDevice } from "../../modules/devices/models/device";
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
    timeZones: string[] = [];

    constructor(private _clientService: PrivateClientService,
        private dialog: MatDialog) {
    }

    ngOnInit() {
        this.loadDevice();
        this._clientService.getTimeZones().subscribe(data => this.timeZones = data);
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
                data: {
                    deviceName: this.device.name,
                    deviceType: this.device.deviceType,
                    timeZone: this.device.timeZone,
                    timeZones: this.timeZones
                }
            });

        dialogRef.afterClosed().subscribe(result => {
            if (result && result.deviceName !== this.device.name) {
                this.updateDeviceName(result.deviceName);
            }

            if (result && result.deviceTypes !== this.device.deviceType) {
                this.updateDeviceType(result.deviceTypes);
            }

            if (result && result.timeZone !== this.device.timeZone) {
                this.updateDeviceTimeZone(result.timeZone);
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

    private updateDeviceType(newDeviceType: number) {
        this._clientService.changeDeviceType(this.deviceId, newDeviceType).subscribe(data => {
            this.device.deviceType = newDeviceType;
            },
            error => {
            });
    }

    private updateDeviceTimeZone(timeZone: string) {
        this._clientService.changeDeviceTimeZone(this.deviceId, timeZone).subscribe(data => {
                this.device.timeZone = timeZone;
            },
            error => {
            });
    }

    getDeviceState(state: boolean) {
        return state ? "Armed" : "Disarmed";
    }
}
