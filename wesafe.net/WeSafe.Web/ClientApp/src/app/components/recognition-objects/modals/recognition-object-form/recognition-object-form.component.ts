import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material";
import { FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
    selector: 'app-recognition-object-form',
    templateUrl: './recognition-object-form.component.html',
    styleUrls: ['./recognition-object-form.component.scss']
})
export class RecognitionObjectFormComponent implements OnInit {
    formGroup: FormGroup;

    constructor(public dialogRef: MatDialogRef<RecognitionObjectFormComponent>,
                @Inject(MAT_DIALOG_DATA) public data) {
    }

    ngOnInit() {
        this.formGroup = new FormGroup({
            name: new FormControl({value: this.data.recognitionObject.name, disabled: !this.data.create},
                Validators.compose([Validators.required, Validators.maxLength(50)])),
            description: new FormControl(this.data.recognitionObject.description),
            isActive: new FormControl(this.data.recognitionObject.isActive)
        });
    }

    onNoClick(): void {
        this.dialogRef.close();
    }
}
