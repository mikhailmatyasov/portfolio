import { Component, OnInit } from '@angular/core';
import { RecognitionObjectsService } from '../../services/recognition-objects.service';
import { IRecognitionObject, RecognitionObject } from '../../models/recognition-object';
import { MatDialog } from '@angular/material/dialog';
import { RecognitionObjectFormComponent } from './modals/recognition-object-form/recognition-object-form.component';

@Component({
    selector: 'app-recognition-objects',
    templateUrl: './recognition-objects.component.html',
    styleUrls: ['./recognition-objects.component.scss']
})
export class RecognitionObjectsComponent implements OnInit {
    pending: boolean = false;
    recognitionObjects: Array<IRecognitionObject> = [];

    constructor(private _service: RecognitionObjectsService,
                private dialog: MatDialog) {
    }

    ngOnInit() {
        this.loadRecognitionObjects();
    }

    add() {
        const ro = new RecognitionObject();

        ro.isActive = true;
        this.openForm(ro, true);
    }

    edit(id: number) {
        this._service.getRecognitionObject(id).subscribe(data => {
            if (data) this.openForm(data, false);
        });
    }

    private openForm(model: IRecognitionObject, create: boolean) {
        const dialogRef = this.dialog.open(RecognitionObjectFormComponent, {
            panelClass: 'dialog',
            data: {
                recognitionObject: model,
                create: create
            }
        });

        dialogRef.afterClosed().subscribe(result => {
            if (result) {
                model.name = result.name;
                model.description = result.description;
                model.isActive = result.isActive;

                let observable = create ? this._service.createRecognitionObject(model) : this._service.updateRecognitionObject(model);

                observable.subscribe(data => {
                    this.loadRecognitionObjects();
                });
            }
        });
    }

    private loadRecognitionObjects() {
        this.pending = true;

        this._service.getRecognitionObjects().subscribe(data => {
            this.recognitionObjects = data;
            this.pending = false;
        }, error => {
            this.pending = false;
        });
    }
}
