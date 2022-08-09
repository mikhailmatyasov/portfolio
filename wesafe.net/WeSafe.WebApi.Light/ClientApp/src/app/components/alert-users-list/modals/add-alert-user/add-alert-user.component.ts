import { Component, OnInit } from '@angular/core';
import { MatDialogRef } from "@angular/material";
import {  phoneNumberPattern } from '../../../../../patterns/regex-patterns';
import { FormGroup, FormControl, Validators } from "@angular/forms";

@Component({
    selector: 'app-add-alert-user',
    templateUrl: './add-alert-user.component.html',
    styleUrls: ['./add-alert-user.component.scss']
})
export class AddAlertUserComponent implements OnInit {
    formGroup: FormGroup;

    constructor(private dialogRef: MatDialogRef<AddAlertUserComponent>) {}

    ngOnInit() {
        this.formGroup = new FormGroup({
            name: new FormControl('',
                Validators.compose([Validators.required])),
            phone: new FormControl('',
                Validators.compose([Validators.required, Validators.pattern(phoneNumberPattern)]))
        });
    }

    onNoClick() {
        this.dialogRef.close();
    }
}
