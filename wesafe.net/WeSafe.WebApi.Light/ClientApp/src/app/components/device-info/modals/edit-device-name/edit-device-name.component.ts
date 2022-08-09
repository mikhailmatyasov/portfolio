import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material";
import { FormGroup, FormControl, Validators } from '@angular/forms';

@Component({
    selector: 'app-edit-device-name',
    templateUrl: './edit-device-name.component.html',
    styleUrls: ['./edit-device-name.component.scss']
})
export class EditDeviceNameComponent implements OnInit {

    formGroup: FormGroup;

    constructor(
        public dialogRef: MatDialogRef<EditDeviceNameComponent>,
        @Inject(MAT_DIALOG_DATA) public data) { }

    ngOnInit() {
        this.formGroup = new FormGroup({
            deviceName: new FormControl(this.data.deviceName,
                Validators.compose([Validators.required, Validators.maxLength(15)])),
        });
    }

    onNoClick(): void {
        this.dialogRef.close();
    }
}
