import { Component, OnInit, Input } from '@angular/core';
import { IDetectedCamera, ConnectingDetectedCamera } from '../../../models/detected-camera';
import { PrivateClientService } from '../../../services/private-client.service';
import { ConfirmationModalComponent } from '../../confirmation-modal/confirmation-modal.component';
import { ConfirmationModel } from '../../confirmation-modal/models/confirmation';
import { MatDialog, PageEvent } from "@angular/material";
import { ConnectCameraModalComponent } from '../connect-camera-modal/connect-camera-modal.component';
import { Page } from '../../../models/page';

@Component({
  selector: 'app-detected-cameras',
  templateUrl: './detected-cameras.component.html',
  styleUrls: ['./detected-cameras.component.scss']
})
export class DetectedCamerasComponent implements OnInit {
    @Input()
    deviceId;

    detectedCameras: IDetectedCamera[] = [];

    page = new Page(1, 5);
    pageSizeOptions: number[] = [5, 10, 25, 50];

    constructor(private _clientService: PrivateClientService,
        private dialog: MatDialog) {
    }

    ngOnInit() {
        this.loadDetectedCameras();
    }

    loadPage(page: PageEvent) {
        this.page.pageNumber = page.pageIndex + 1;
        this.page.size = page.pageSize;
        this.loadDetectedCameras();
    }

  loadDetectedCameras() {
      this._clientService.getDeviceDetectedCameras(this.deviceId, this.page).subscribe(data => {
          this.detectedCameras = data.items;
          this.page.total = data.total;
      }, error => {
      });
  }

  deleteDetectedCamera(id: number) {
      const dialogRef = this.dialog.open(ConfirmationModalComponent, {
          panelClass: 'dialog',
          data: new ConfirmationModel("Delete this Camera?", "yes, delete", "no, cancel")
      });

      dialogRef.afterClosed().subscribe(result => {
          if (!result)
              return;

          this._clientService.removeDeviceDetectedCamera(this.deviceId, id).subscribe(data => {
                  this.loadDetectedCameras();
              },
              error => {
              });
      });
  }

  connect(camera: IDetectedCamera) {
      const dialogRef = this.dialog.open(ConnectCameraModalComponent, {
          panelClass: 'dialog',
          data: {
              login: camera.login || '',
              password: camera.password || ''
          }
      });

      dialogRef.afterClosed().subscribe(result => {
          if (!result)
              return;

          const connect = new ConnectingDetectedCamera();

          connect.id = camera.id;
          connect.login = result.login;
          connect.password = result.password;

          this._clientService.connectingDeviceDetectedCamera(this.deviceId, camera.id, connect).subscribe(data => {
                  this.loadDetectedCameras();
              },
              error => {
              });
      });
  }

}
