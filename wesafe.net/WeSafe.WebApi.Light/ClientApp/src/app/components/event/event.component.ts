import { Component, Input, OnInit } from '@angular/core';
import { IEvent } from '../../models/event';
import { EventVideoDialogComponent } from './event-video-dialog/event-video-dialog.component';
import { MatDialog } from '@angular/material';
import { VideoService } from '../../services/video.service';

@Component({
    selector: 'app-event',
    templateUrl: './event.component.html',
    styleUrls: ['./event.component.scss']
})
export class EventComponent implements OnInit {
    @Input()
    event: IEvent;
    videoUrl: string;
    imageIndex = 0;

    constructor(private dialog: MatDialog, private _videoService: VideoService) {
    }

    ngOnInit() {
    }

    imageClick() {
        if (this.imageIndex === this.event.entries.length - 1) this.imageIndex = 0;
        else this.imageIndex++;
    }

    getEventVideo() {
        this._videoService.getEventVideo(this.event.cameraId, this.event.time).subscribe(data => {
            this.videoUrl = data.videoUrl;
            this.openEventVideo(this.videoUrl);
        },
            error => {
                console.log(error);
            });
    }

    openEventVideo(videoUrl: string) {
        const dialogRef = this.dialog.open(EventVideoDialogComponent,
            {
                panelClass: 'dialog',
                data: { videoUrl: videoUrl }
            });

        dialogRef.afterClosed().subscribe(result => {
        });
    }
}
