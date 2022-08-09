import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { ICamera } from '../../../../models/camera';
import { DemoService } from '../../services/demo.service';

@Component({
    selector: 'app-all-cameras-list',
    templateUrl: './all-cameras-list.component.html',
    styleUrls: ['./all-cameras-list.component.scss'],
    encapsulation: ViewEncapsulation.None
})
export class AllCamerasListComponent implements OnInit {
    cameras: Array<ICamera> = [];
    pending: boolean;

    constructor(private _demoService: DemoService) {
    }

    ngOnInit() {
        this.loadCameras();
    }

    loadCameras() {
        this.pending = true;

        this._demoService.getCameras().subscribe(data => {
            this.cameras = data;
            this.pending = false;
        }, error => {
            this.pending = false;
        });
    }
}
