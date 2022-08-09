import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
    selector: 'app-area-form',
    templateUrl: './area-form.component.html',
    styleUrls: ['./area-form.component.scss']
})
export class AreaFormComponent implements OnInit {
    title: string;
    buttonText: string;
    create: boolean;
    formGroup: FormGroup;
    areas = [
        { type: 'ignore', name: 'Ignore area',},
        { type: 'perimeter', name: 'Perimeter area',},
        { type: 'pool', name: 'Pool area', },
        { type: 'entryline', name: 'Entry Line' },
        { type: 'exitline', name: 'Exit Line' },
    ];

    constructor(@Inject(MAT_DIALOG_DATA) public data: any,
                private dialogRef: MatDialogRef<AreaFormComponent>) {
    }

    ngOnInit() {
        this.create = this.data.create;

        if (this.data.create) {
            this.title = 'Add new ROI';
            this.buttonText = 'Draw';
        }
        else {
            this.title = 'Edit ROI';
            this.buttonText = 'Save';
        }

        this.formGroup = new FormGroup({
            name: new FormControl(this.data.area.name,
                Validators.compose([Validators.required])),
            type: new FormControl(this.data.area.type,
                Validators.compose([Validators.required]))
        });
    }

    onNoClick() {
        this.dialogRef.close();
    }
}
