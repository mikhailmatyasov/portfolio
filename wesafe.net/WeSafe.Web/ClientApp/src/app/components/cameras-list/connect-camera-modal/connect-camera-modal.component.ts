import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
    selector: 'app-connect-camera-modal',
    templateUrl: './connect-camera-modal.component.html',
    styleUrls: ['./connect-camera-modal.component.scss']
})
export class ConnectCameraModalComponent implements OnInit {
    formGroup: FormGroup;

    constructor(public dialogRef: MatDialogRef<ConnectCameraModalComponent>,
                @Inject(MAT_DIALOG_DATA) public data) {
    }

    ngOnInit() {
        this.formGroup = new FormGroup({
            login: new FormControl(this.data.login,
                Validators.compose([Validators.required, Validators.maxLength(50)])),
            password: new FormControl(this.data.password,
                Validators.compose([Validators.required, Validators.maxLength(50)]))
        });
    }

    onNoClick(): void {
        this.dialogRef.close();
    }
}
