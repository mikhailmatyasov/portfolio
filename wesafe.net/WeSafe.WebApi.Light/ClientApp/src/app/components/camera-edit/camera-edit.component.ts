import { Component, OnInit } from '@angular/core';
import { PrivateClientService } from '../../services/private-client.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Camera, ICamera, RecognitionSettings } from '../../models/camera';
import { CameraTypeService } from '../../services/camera-type.service';
import { ICameraType, ICameraVendor } from '../../models/camera-type';
import { tap } from 'rxjs/operators';
import { IScheduler, ISchedulerRule, Scheduler, WeekDaysHoursSchedulerRule, WeekDaysHoursSchedulerRuleType } from '../../models/scheduler';
import { Options } from 'ng5-slider';
import { CameraManufactorService } from '../../services/camera-manufactor.service';
import { Location } from '@angular/common';

@Component({
    selector: 'app-camera-edit',
    templateUrl: './camera-edit.component.html',
    styleUrls: ['./camera-edit.component.scss']
})
export class CameraEditComponent implements OnInit {
    camera: ICamera;
    pending: boolean;
    cameraVendors: Array<ICameraVendor> = [];
    cameraTypes: Array<ICameraType> = [];
    cameraManufactors: Array<CameraManufactor> = [];
    selectedVendorId: number;
    selectedCameraManufactorId: number;
    selectedCameraMarkId: number;
    selecredCameraRtspPathId: number;
    rtspTemplate: string;
    activeCameraCount: number;
    maxActiveCameras: number;
    create: boolean;
    showReached: boolean;
    matrix: any = {};
    days = [
        { day: 6, name: 'SUN' },
        { day: 0, name: 'MON' },
        { day: 1, name: 'TUE' },
        { day: 2, name: 'WED' },
        { day: 3, name: 'THU' },
        { day: 4, name: 'FRI' },
        { day: 5, name: 'SAT' },
    ];
    hours: Array<number> = [];
    settings: RecognitionSettings;
    confidenceOptions: Options = {
        floor: 70,
        ceil: 100
    };
    sensitivityOptions: Options = {
        floor: 0,
        ceil: 10
    };
    alertFreqOptions: Options = {
        floor: 1,
        ceil: 60
    };

    public deviceId: number;
    private cameraId: number;

    constructor(private _clientService: PrivateClientService,
                private _cameraTypeService: CameraTypeService,
                private _router: Router,
                private _route: ActivatedRoute,
                private _cameraManufactorService: CameraManufactorService,
                private _location: Location) {
        for (let i = 0; i < 24; i++) {
            this.hours.push(i);
        }
    }

    ngOnInit() {
        this.deviceId = this._route.snapshot.params.deviceId;
        this.selectedVendorId = null;
        this.create = this._route.snapshot.data.create;

        if (this.create) {
            this.camera = new Camera();
            this.camera.isActive = true;
            this.settings = new RecognitionSettings();

            this.loadCameraStat().subscribe(data => {
                if (this.activeCameraReached()) {
                    this.camera.isActive = false;
                }
            });
        } else {
            this.cameraId = this._route.snapshot.params.cameraId;
            this.pending = true;

            this._clientService.getDeviceCamera(this.deviceId, this.cameraId).subscribe(data => {
                this.camera = new Camera(data);
                this.settings = RecognitionSettings.parse(this.camera.recognitionSettings);

                if (this.camera.schedule) this.buildMatrix(JSON.parse(this.camera.schedule));

                if (this.camera.cameraTypeId) {
                    this._cameraTypeService.getCameraType(this.camera.cameraTypeId).subscribe(res => {
                        if (res) {
                            this.selectedVendorId = res.cameraVendorId;
                            this.rtspTemplate = res.rtspTemplate;
                            this.loadCameraTypes(this.selectedVendorId);
                        }
                    });
                }

                this.loadCameraStat().subscribe(stat => {
                    if (this.activeCameraReached() && !this.camera.isActive) {
                        this.showReached = true;
                    }
                });

                this.pending = false;
            }, error => {
                this.pending = false;
            });
        }

        this.loadVendors();
        this.loadCameraManufactors();
    }

    loadCameraManufactors() {
        this._cameraManufactorService.getManufactors().subscribe(data => {
            this.cameraManufactors = data;
        });
    }

    getSelectedManufactor() {
        return this.cameraManufactors.filter(c => c.id === this.selectedCameraManufactorId)[0];
    }

    getSelectedMark() {
        return this.getSelectedManufactor().cameraMarks.filter(x => x.id === this.selectedCameraMarkId)[0];
    }

    cameraManufactorChanged() {
        const manufactor = this.getSelectedManufactor();

        this.selectedCameraMarkId = manufactor.cameraMarks[0].id;
        this.selecredCameraRtspPathId = manufactor.cameraMarks[0].rtspPaths[0].id;

        this.updateRtspTemplate(manufactor.cameraMarks[0].rtspPaths[0].path);
    }

    cameraMarkChanged() {
        this.selecredCameraRtspPathId = this.getSelectedMark().rtspPaths[0].id;
        const path = this.getSelectedMark().rtspPaths.filter(x => x.id === this.selecredCameraRtspPathId)[0].path;

        this.updateRtspTemplate(path);
    }
    rtspPathChanged() {
        const path = this.getSelectedMark().rtspPaths.filter(x => x.id === this.selecredCameraRtspPathId)[0].path;
        this.updateRtspTemplate(path);
    }

    cameraVendorChanged() {
        this.rtspTemplate = null;
        this.camera.cameraTypeId = null;

        if (this.selectedVendorId) {
            this.loadCameraTypes(this.selectedVendorId, true);
        }
    }

    updateRtspTemplate(template: string) {
        this.rtspTemplate = 'rtsp://{login}:{pwd}@{ip}:{port}' + template;
        this.cameraChanged();
    }

    cameraTypeChanged(id) {
        let type: ICameraType;

        for (let i = 0; i < this.cameraTypes.length; i++) {
            if (this.cameraTypes[i].id === id) {
                type = this.cameraTypes[i];
            }
        }

        if (type) {
            this.rtspTemplate = type.rtspTemplate;
        }
    }

    submit() {
        const rules: Array<ISchedulerRule> = [];

        for (const weekdayProp in this.matrix) {
            const rule = new WeekDaysHoursSchedulerRule();

            // tslint:disable-next-line:no-bitwise
            rule.Days = 1 << (+weekdayProp);

            for (const hourProp in this.matrix[weekdayProp]) {
                if (this.matrix[weekdayProp][hourProp].checked) {
                    rule.Hours.push(+hourProp);
                }
            }

            if (rule.Hours.length > 0) rules.push(rule);
        }

        if (rules.length > 0) this.camera.schedule = JSON.stringify(new Scheduler(rules));
        else this.camera.schedule = null;

        if (this.camera.recognitionSettings) {
            let obj = JSON.parse(this.camera.recognitionSettings);

            if (!obj) obj = {};

            obj.confidence = this.settings.confidence;
            obj.sensitivity = this.settings.sensitivity;
            obj.alertFrequency = this.settings.alertFrequency;

            this.camera.recognitionSettings = JSON.stringify(obj);
        }
        else this.camera.recognitionSettings = JSON.stringify(this.settings);

        let observable;

        this.pending = true;

        if (this._route.snapshot.data.create) {
            observable = this._clientService.createDeviceCamera(this.deviceId, this.camera);
        } else {
            observable = this._clientService.updateDeviceCamera(this.deviceId, this.camera);
        }

        observable.subscribe(data => {
            // this._router.navigateByUrl('/devices/' + this.deviceId);
            this._location.back();
            this.pending = false;
        }, error => {
            this.pending = false;
        });
    }

    cancel() {
        // this._router.navigateByUrl('/devices/' + this.deviceId);
        this._location.back();
    }

    cameraChanged() {
        if (this.rtspTemplate) {
            this.camera.specificRtcpConnectionString = this.rtspTemplate
                .replace('{ip}', this.camera.ip || '')
                .replace('{port}', this.camera.port || '')
                .replace('{login}', this.camera.login || '')
                .replace('{pwd}', escape(this.camera.password) || '');
            this.camera.specificRtcpConnectionString = unescape(this.camera.specificRtcpConnectionString);
        }
    }

    activeCameraReached(): boolean {
        return this.maxActiveCameras !== null && this.activeCameraCount >= this.maxActiveCameras;
    }

    findMatrixItem(weekday: number, hour: number) {
        for (const prop in this.matrix) {
            if (prop === weekday.toString()) {
                for (const hourProp in this.matrix[prop]) {
                    if (hourProp === hour.toString() && this.matrix[prop][hourProp]) return this.matrix[prop][hourProp];
                }
            }
        }

        return null;
    }

    isMatrixItemChecked(weekday: number, hour: number) {
        const item = this.findMatrixItem(weekday, hour);

        return item == null ? false : item.checked;
    }

    matrixItemClick(weekday: number, hour: number) {
        const item = this.getOrCreateItem(weekday, hour);

        item.checked = !item.checked;
    }

    hourClick(hour: number) {
        let checked = true;

        for (let i = 0; i < this.days.length; i++) {
            const item = this.getOrCreateItem(this.days[i].day, hour);

            checked = checked && item.checked;
        }

        for (let i = 0; i < this.days.length; i++) {
            const item = this.getOrCreateItem(this.days[i].day, hour);

            item.checked = !checked;
        }
    }

    weekdayClick(weekday: number) {
        let checked = true;

        for (let i = 0; i < this.hours.length; i++) {
            const item = this.getOrCreateItem(weekday, this.hours[i]);

            checked = checked && item.checked;
        }

        for (let i = 0; i < this.hours.length; i++) {
            const item = this.getOrCreateItem(weekday, this.hours[i]);

            item.checked = !checked;
        }
    }

    private getOrCreateItem(weekday: number, hour: number) {
        let item = this.findMatrixItem(weekday, hour);

        if (item === null) {
            const matrixItem = this.matrix[weekday] || (this.matrix[weekday] = {});

            item = matrixItem[hour] || (matrixItem[hour] = { checked: false });
        }

        return item;
    }

    private buildMatrix(scheduler: IScheduler) {
        if (!scheduler || !scheduler.Rules) return;

        for (let i = 0; i < scheduler.Rules.length; i++) {
            const rule = scheduler.Rules[i];

            if (rule.TYPE !== WeekDaysHoursSchedulerRuleType) continue;

            const weekDayRule = new WeekDaysHoursSchedulerRule(rule);

            for (let j = 0; j < 7; j++) {
                // tslint:disable-next-line:no-bitwise
                if ((weekDayRule.Days & (1 << j)) === (1 << j)) {
                    const matrixItem = this.matrix[j] || (this.matrix[j] = {});

                    for (let k = 0; k < weekDayRule.Hours.length; k++) {
                        const hour = weekDayRule.Hours[k];
                        const hourItem = matrixItem[hour] || (matrixItem[hour] = { checked: true });

                        hourItem.checked = true;
                    }
                }
            }
        }
    }

    private loadVendors() {
        this.pending = true;

        this._cameraTypeService.getVendors(true).subscribe(data => {
            this.cameraVendors = data;
            this.pending = false;
        }, error => {
            this.pending = false;
        });
    }

    private loadCameraTypes(cameraVendorId: number, update: boolean = false) {
        // this.pending = true;

        this._cameraTypeService.getCameraTypes(cameraVendorId, true).subscribe(data => {
            this.cameraTypes = data;
            // this.pending = false;

            if (update && data.length > 0) {
                this.camera.cameraTypeId = data[0].id;
                this.cameraTypeChanged(this.camera.cameraTypeId);
                this.cameraChanged();
            }
        }, error => {
            // this.pending = false;
        });
    }

    private loadCameraStat() {
        return this._clientService.getDeviceCameraStat(this.deviceId)
                   .pipe(tap(data => {
                       this.activeCameraCount = data.activeCount;
                       this.maxActiveCameras = data.maxActiveCameras;
                   }));
    }
}
