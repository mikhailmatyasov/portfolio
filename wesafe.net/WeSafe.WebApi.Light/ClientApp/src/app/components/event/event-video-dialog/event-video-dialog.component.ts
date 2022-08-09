import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material";
import { FormGroup, Validators, FormControl } from '@angular/forms';

@Component({
  selector: 'app-event-video-dialog',
  templateUrl: './event-video-dialog.component.html',
  styleUrls: ['./event-video-dialog.component.scss']
})
export class EventVideoDialogComponent implements OnInit {

    formGroup: FormGroup;

    constructor(
        public dialogRef: MatDialogRef<EventVideoDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data) { }

    ngOnInit() {
        this.formGroup = new FormGroup({
            videoUrl: new FormControl(this.data.videoUrl,
                Validators.compose([Validators.required])),
        });
    }

    onNoClick(): void {
        this.dialogRef.close();
    }

}
