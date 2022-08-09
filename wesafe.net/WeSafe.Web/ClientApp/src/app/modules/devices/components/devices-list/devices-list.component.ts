import { Component, OnInit, ViewChild } from '@angular/core';
import { Page } from '../../../../models/page';
import { DevicesService } from '../../services/devices.service';
import { IDevice, DeviceFilter, Device } from '../../models/device';
import { PageEvent, MatPaginator } from '@angular/material/paginator';

@Component({
    selector: 'app-devices-list',
    templateUrl: './devices-list.component.html',
    styleUrls: ['./devices-list.component.scss']
})

export class DevicesListComponent implements OnInit {
    page = new Page(1, 100);
    pending: boolean;
    rows: Array<IDevice> = [];
    pageSizeOptions: number[] = [10, 25, 50, 100];
    searchText: string;
    filterBy: number = null;
    @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;

    constructor(private _deviceService: DevicesService) {
    }

    ngOnInit() {
        this.loadDevices();
    }

    loadPage(page: PageEvent) {
        this.page.pageNumber = page.pageIndex + 1;
        this.page.size = page.pageSize;
        this.loadDevices();
    }

    getDeviceFilterModel(): DeviceFilter {
        let filter = new DeviceFilter();
        filter.sort = null;
        filter.clientId = null;
        filter.search = this.searchText;
        filter.filterBy = this.filterBy;
        return filter;
    }

    getDevices() {
        let filter = this.getDeviceFilterModel();
        this._deviceService.getDevices(this.page, filter).subscribe(data => {
            this.pending = false;
            this.rows = data.items;
            this.page.total = data.total;
        }, error => {
            this.pending = false;
        });
    }

    loadDevices() {
        this.pending = true;
        this.getDevices();
    }

    filterDevices() {
        this.page.pageNumber = 1;
        this.loadDevices();
        this.paginator.firstPage();
    }
}
