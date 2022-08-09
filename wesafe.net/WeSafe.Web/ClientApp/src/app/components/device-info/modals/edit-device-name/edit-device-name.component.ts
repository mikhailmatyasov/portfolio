import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material";
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { DevicesService } from '../../../../modules/devices/services/devices.service';
import { IDeviceType } from '../../../../modules/devices/models/device';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { TimeZone } from '../../../../models/time-zone';

@Component({
    selector: 'app-edit-device-name',
    templateUrl: './edit-device-name.component.html',
    styleUrls: ['./edit-device-name.component.scss']
})
export class EditDeviceNameComponent implements OnInit {

    formGroup: FormGroup;
    deviceTypes: Array<IDeviceType> = [];
    selectedDeviceType: number;
    timeZones: Array<TimeZone> = [];
    filteredTimeZones: Observable<Array<TimeZone>>;

    constructor(
        private _devicesService: DevicesService,
        public dialogRef: MatDialogRef<EditDeviceNameComponent>,
        @Inject(MAT_DIALOG_DATA) public data) { }

    ngOnInit() {
        const timeZoneCtrl = new FormControl(this.data.timeZone, Validators.required);

        this.loadDeviceTypes();
        this.formGroup = new FormGroup({
            deviceName: new FormControl(this.data.deviceName,
                Validators.compose([Validators.required, Validators.maxLength(15)])),
            deviceTypes: new FormControl(this.deviceTypes),
            timeZone: timeZoneCtrl
        });
        this.selectedDeviceType = this.data.deviceType;
        this.timeZones = this.data.timeZones;

        this.filteredTimeZones = timeZoneCtrl.valueChanges
                                   .pipe(
                                       startWith(''),
                                       map(value => this._filter(value))
                                   );
    }

    onNoClick(): void {
        this.dialogRef.close();
    }

    loadDeviceTypes() {
        this._devicesService.getDeviceTypes().subscribe(data => {
                this.deviceTypes = Object.keys(data).map(k => {
                    return { 'id': parseInt(k), 'value': data[k] };
                });
            },
            error => {
            });
    }

    private _filter(value: string): Array<TimeZone> {
        const filterValue = value.toLowerCase();

        return this.timeZones.filter(option => option.name.toLowerCase().includes(filterValue));
    }
}
