import { Component, OnInit, Input, SimpleChanges } from '@angular/core';
import { ICamera } from '../../../models/camera';
import { MatDialog, PageEvent } from "@angular/material";
import { PrivateClientService } from '../../../services/private-client.service';
import { Router } from '@angular/router';
import { ConfirmationModalComponent } from '../../confirmation-modal/confirmation-modal.component';
import { ConfirmationModel } from '../../confirmation-modal/models/confirmation';
import { Page } from '../../../models/page';

@Component({
  selector: 'app-active-cameras',
  templateUrl: './active-cameras.component.html',
  styleUrls: ['./active-cameras.component.scss']
})
export class ActiveCamerasComponent implements OnInit {

    @Input()
    deviceId;

    cameras: Array<ICamera> = [];

    page = new Page(1, 5);
    pageSizeOptions: number[] = [5, 10, 25, 50];

    constructor(private _clientService: PrivateClientService,
        private _router: Router, private dialog: MatDialog) {
    }

    ngOnInit() {
        this.loadCameras();
  }
  ngOnChanges(changes: SimpleChanges): void {
      if (changes && changes.deviceId) {
          this.loadCameras();
      }
  }

    loadPage(page: PageEvent) {
        this.page.pageNumber = page.pageIndex + 1;
        this.page.size = page.pageSize;
        this.loadCameras();
  }

  loadCameras() {
      this._clientService.getDeviceCameras(this.deviceId, this.page).subscribe(data => {
          this.cameras = data.items;
          this.page.total = data.total;
      }, error => {
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
                  this.loadCameras();
              },
              error => {
              });
      });
  }
}
