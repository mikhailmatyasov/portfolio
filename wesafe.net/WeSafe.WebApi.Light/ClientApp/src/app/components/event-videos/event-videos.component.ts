import { Component, OnInit, Input } from '@angular/core';
import { PrivateClientService } from '../../services/private-client.service';
import { ICamera } from '../../models/camera';
import { VideoService } from '../../services/video.service';
import { NgbInputDatepicker } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-event-videos',
  templateUrl: './event-videos.component.html',
  styleUrls: ['./event-videos.component.scss']
})
export class EventVideosComponent implements OnInit {
    cameras: Array<ICamera> = [];
    selectedCameraId: number;
    selectedStartDate: Date;
    selectedEndDate: Date;

    videos: Array<string> = [];

    constructor(private _clientService: PrivateClientService, private _videoService: VideoService) { }

    ngOnInit() {
        this.loadCameras();
    }

    loadCameras() {
        this._clientService.getDeviceCameras(1).subscribe(data => {
            this.cameras = data;
        }, error => {
        });
    }

    loadVideos() {
        this._videoService.getVideos(this.selectedCameraId, this.selectedStartDate, this.selectedEndDate).subscribe(data => {
            this.videos = data;
        }, error => {
        });
    }

    clearDatePicker(datePicker: NgbInputDatepicker) {
        datePicker.close();
        datePicker.manualDateChange('', true);
        datePicker.writeValue('');
    }

    startDateTimeChanged(event) {
        this.selectedStartDate = event.value;
    }

    endDateTimeChanged(event) {
        this.selectedEndDate = event.value;
    }
}
