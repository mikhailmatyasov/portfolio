import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { IClientSubscriber } from '../../models/client-subscriber';
import { PrivateClientService } from '../../services/private-client.service';
import { Assignment, IAssignment } from '../../models/assignment';
import { IDevice } from '../../models/device';
import { ICamera } from '../../models/camera';
import { MatDialogRef } from '@angular/material';

@Component({
    selector: 'app-assignment-modal',
    templateUrl: './assignment-modal.component.html',
    styleUrls: ['./assignment-modal.component.scss']
})
export class AssignmentModalComponent implements OnInit {
    @Input()
    user: IClientSubscriber;

    pending: boolean;
    adding: boolean;
    assignments: Array<IAssignment> = [];
    addingItem: IAssignment;
    errorMessage: string;
    devices: Array<IDevice> = [];
    cameras: Array<ICamera> = [];
    availableDevices: Array<IDevice> = [];
    availableCameras: Array<ICamera> = [];
    showAllCameras: boolean;

    constructor(private dialogRef: MatDialogRef<AssignmentModalComponent>, private _clientService: PrivateClientService) {
    }

    ngOnInit() {
        this.loadAssignments();
        this.loadDevices();
    }

    add() {
        this.adding = true;
        this.showAllCameras = false;
        this.addingItem = new Assignment();
        this.availableCameras = [];
    }

    delete(assignment: IAssignment) {
        const i = this.assignments.indexOf(assignment);

        this.assignments.splice(i, 1);
        this.prepareAvailableDevices();
    }

    commit() {
        const exist = this.assignments.find(item => item.deviceId === this.addingItem.deviceId &&
            item.cameraId === this.addingItem.cameraId);

        if (exist) {
            this.errorMessage = 'This assignment already exists.';
            return;
        }

        const device = this.devices.find(item => item.id === this.addingItem.deviceId);

        this.addingItem.deviceName = device.name;

        if (this.addingItem.cameraId) {
            const camera = this.cameras.find(item => item.id === this.addingItem.cameraId);

            this.addingItem.cameraName = camera.cameraName;
        }

        this.assignments.push(this.addingItem);
        this.prepareAvailableDevices();
        this.adding = false;
    }

    cancel() {
        this.errorMessage = null;
        this.adding = false;
    }

    save() {
        this.pending = true;

        this._clientService.saveSubscriberAssignments(this.user.id, this.assignments).subscribe(data => {
            this.pending = false;
            this.dialogRef.close();
        }, error => {
            this.errorMessage = 'Error';
            this.pending = false;
        });
    }

    deviceChanged() {
        this.loadCameras(this.addingItem.deviceId);
        this.addingItem.cameraId = null;
    }

    private loadAssignments() {
        this.pending = true;

        this._clientService.getSubscriberAssignments(this.user.id).subscribe(data => {
            this.assignments = data;
            this.pending = false;
        }, error => {
            this.pending = false;
        });
    }

    private loadDevices() {
        this._clientService.getDevices().subscribe(data => {
            this.devices = data;
            this.prepareAvailableDevices();
        });
    }

    private loadCameras(deviceId: number) {
        this._clientService.getDeviceCameras(deviceId).subscribe(data => {
            this.cameras = data;
            this.prepareAvailableCameras();
        });
    }

    private prepareAvailableDevices() {
        this.availableDevices = this.devices.filter(item => {
            return !this.assignments.find(x => x.deviceId === item.id && x.cameraId === null);
        });
    }

    private prepareAvailableCameras() {
        this.showAllCameras = !this.assignments.find(x => x.deviceId === this.addingItem.deviceId && x.cameraId !== null);

        this.availableCameras = this.cameras.filter(item => {
            return !this.assignments.find(x => x.deviceId === this.addingItem.deviceId && x.cameraId === item.id);
        });
    }
}
