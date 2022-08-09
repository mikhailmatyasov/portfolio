import { Component, Input, OnChanges, OnInit, SimpleChanges, ViewEncapsulation } from '@angular/core';
import { ICamera } from '../../models/camera';
import { PrivateClientService } from '../../services/private-client.service';
import { Router } from '@angular/router';
import { ConfirmationModalComponent } from "../confirmation-modal/confirmation-modal.component";
import { ConfirmationModel } from "../confirmation-modal/models/confirmation";
import { MatDialog } from "@angular/material";

@Component({
    selector: 'app-cameras-list',
    templateUrl: './cameras-list.component.html',
    styleUrls: ['./cameras-list.component.scss'],
    encapsulation: ViewEncapsulation.None
})
export class CamerasListComponent implements OnInit, OnChanges {
    @Input()
    deviceId: number;

    cameras: Array<ICamera> = [];
    pending: boolean;

    constructor(private _clientService: PrivateClientService,
        private _router: Router, private dialog: MatDialog) {
    }

    ngOnInit() {
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (changes && changes.deviceId) this.loadCameras();
    }

    loadCameras() {
        this.pending = true;

        this._clientService.getDeviceCameras(this.deviceId).subscribe(data => {
            this.cameras = data;
            this.pending = false;
        }, error => {
            this.pending = false;
        });
    }

    addCamera() {
        this._router.navigate(['devices', this.deviceId, 'cameras', 'create']);
    }

    deleteCamera(id: number) {
        const dialogRef = this.dialog.open(ConfirmationModalComponent, {
            panelClass: 'dialog',
            data: new ConfirmationModel("Delete this Camera?", "yes, delete", "no, cancel")
        });

        dialogRef.afterClosed().subscribe(result => {
            if (!result)
                return;

            this._clientService.removeDeviceCamera(this.deviceId, id).subscribe(data => {
                    this.pending = false;

                    this.loadCameras();
                },
                error => {
                    this.pending = false;
                });

        });

        //if (confirm('Do you want to delete camera?')) {
        //    this._clientService.removeDeviceCamera(this.deviceId, id).subscribe(data => {
        //        this.pending = false;

        //        this.loadCameras();
        //    }, error => {
        //        this.pending = false;
        //    });
        //}
    }
}
