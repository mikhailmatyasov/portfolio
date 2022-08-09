import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { deviceTokenPattern } from "../../../../patterns/regex-patterns";
import { FormGroup, FormControl, Validators } from '@angular/forms';

@Component({
    selector: 'bind-device-dialog',
    templateUrl: './bind-device-dialog.component.html',
    styleUrls: ['./bind-device-dialog.component.scss']
})
export class BindDeviceDialog implements OnInit {
    formGroup: FormGroup;
    constructor(
        public dialogRef: MatDialogRef<BindDeviceDialog>) { }

    ngOnInit(): void {
        this.formGroup = new FormGroup({
            deviceToken: new FormControl('',
                Validators.compose([Validators.required, Validators.pattern(deviceTokenPattern)])),
        });
    }
}
