import { environment } from "../../environments/environment";
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class VideoService {

    protected videoApi = "api/videoRecords";
    protected videoUrl = environment.apiUrl + this.videoApi;

    constructor(private _http: HttpClient) {
    }

    getVideos(cameraId: number, startDate: Date, endDate: Date) {

        let url = this.videoUrl + "?cameraId=" + cameraId;

        if (startDate) {
            url = url + "&StartDateTime=" + startDate.toISOString();
        }

        if (startDate) {
            url = url + "&EndDateTime=" + endDate.toISOString();
        }

        return this._http.get<any>(url);
    }

    getEventVideo(cameraId: number, eventDate: string) {

        let url = this.videoUrl + "/eventVideo" + "?cameraId=" + cameraId + "&eventDate=" + eventDate;

        return this._http.get<any>(url);
    }
}
