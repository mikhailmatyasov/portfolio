import { Component, OnInit } from '@angular/core';
import { Page } from '../../models/page';
import { IDeviceLog, IDeviceLogFilter, DeviceLogFilter, ILogLevel } from '../../models/device-log';
import { DevicesLogsService } from '../../services/devices-logs.service';
import { ClientsService } from '../../modules/clients/services/clients.service';
import { PrivateClientService } from '../../services/private-client.service';
import { ICamera } from '../../models/camera';
import { IDevice } from '../../modules/devices/models/device';
import { IClient } from '../../modules/clients/models/client';
import { NgbInputDatepicker } from '@ng-bootstrap/ng-bootstrap';
import { IDropdownSettings } from 'ng-multiselect-dropdown';
import { PageEvent } from '@angular/material/paginator';

@Component({
  selector: 'app-devices-logs-list',
  templateUrl: './devices-logs-list.component.html',
    styleUrls: ['./devices-logs-list.component.scss']
})

export class DevicesLogsListComponent implements OnInit {
    page = new Page(1, 100);
    searchText: string;
    rows: Array<IDeviceLog> = [];
    logLevels: Array<ILogLevel> = [];
    clients: Array<IClient> = [];
    devices: Array<IDevice> = [];
    cameras: Array<ICamera> = [];
    selectedClientId: number = 0;
    selectedDeviceId: number = 0;
    selectedCameraId: number = 0;
    selectedLogLevels: Array<ILogLevel>;
    selectedStartDate: Date;
    selectedEndDate: Date;
    deviceLogFilter: IDeviceLogFilter = new DeviceLogFilter();
    dropdownSettings: IDropdownSettings = {
        singleSelection: false,
        idField: 'id',
        textField: 'value',
    };
    pageSizeOptions: number[] = [10, 25, 50, 100];

    constructor(private _deviceLogService: DevicesLogsService, private _clientService: ClientsService, private _privateClientService: PrivateClientService) {
    }

    ngOnInit() {
        this.loadDevicesLogs();
        this.getLogLevels();
        this.getClientNames();
    }

    onSelectAndDeSelectAll(items: any) {
        this.selectedLogLevels = items;
    }

    loadPage(page: PageEvent) {
        this.page.pageNumber = page.pageIndex + 1;
        this.page.size = page.pageSize;
        this.loadDevicesLogs();
    }

    loadDevicesLogs() {
        let filter = this.getDeviceLogFilterModel();
        this._deviceLogService.getDevicesLogs(this.page, filter).subscribe(data => {
            this.rows = data.items;
            this.page.total = data.total;
        }, error => {
        });
    }

    insertLastLogs() {
        this._deviceLogService.insertDevicesLogs().subscribe(data => {
            this.loadDevicesLogs();
        }, error => {
        });
    }

    getLogLevels() {
        this._deviceLogService.getDevicesLogLevels().subscribe(data => {
                this.logLevels = Object.keys(data).map(k => {
                    return { 'id': parseInt(k), 'value': data[k] };
                });
            },
            error => {
            });
    }

    getClientNames() {
        this._clientService.getClients(new Page(1, null)).subscribe(data => {
            this.clients = data.items;
        }, error => {
        });
    }

    getClientsDevices(clientId: number) {
        if (this.selectedClientId && this.selectedClientId != 0) {
            this._clientService.getClientDevices(clientId).subscribe(data => {
                this.devices = data;
            }, error => {
            });
        }
    }

    getDeviceCameras(deviceId: number) {
        if (this.selectedDeviceId && this.selectedDeviceId != 0) {
            this._privateClientService.getDeviceCameras(deviceId, new Page(1, 100)).subscribe(data => {
                this.cameras = data.items;
            }, error => {
            });
        }
    }

    getDeviceLogFilterModel(): DeviceLogFilter{
        let filter = new DeviceLogFilter();

        if (this.selectedLogLevels) {
            filter.logLevels = [];
            this.selectedLogLevels.forEach((item) => {
                filter.logLevels.push(item.value);
            });
        }

        if (this.selectedClientId) {
            filter.clientId = this.selectedClientId;

            if (this.selectedDeviceId) {
                filter.deviceId = this.selectedDeviceId;

                if (this.selectedCameraId) {
                    filter.cameraId = this.selectedCameraId;
                }
            }
        }

        if (this.selectedStartDate)
            filter.fromDate =
                this.getDateFormat(this.selectedStartDate);

        if (this.selectedEndDate)
            filter.toDate =
                this.getDateFormat(this.selectedEndDate);

        if (this.searchText)
            filter.searchText = this.searchText.toLocaleLowerCase();

        return filter;
    }

    getDownloadLogsUrl() {
        let filter = this.getDeviceLogFilterModel();

        return this._deviceLogService.getDownloadLogsUrl(filter);
    }

    private getDateFormat(datePicker: Date): string {
        return datePicker.toISOString();
    }

    clearDatePicker(datePicker: NgbInputDatepicker) {
        datePicker.close();
        datePicker.manualDateChange("", true);
        datePicker.writeValue("");
    }

    startDateTimeChanged(event) {
        this.selectedStartDate = event.value;
    }

    endDateTimeChanged(event) {
        this.selectedEndDate = event.value;
    }
}
