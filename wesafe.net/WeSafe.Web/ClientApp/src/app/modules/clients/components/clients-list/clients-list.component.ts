import { Component, OnInit } from '@angular/core';
import { Page } from '../../../../models/page';
import { ClientsService } from '../../services/clients.service';
import { IClient } from '../../models/client';
import { PageEvent } from '@angular/material/paginator';

@Component({
    selector: 'app-clients-list',
    templateUrl: './clients-list.component.html',
    styleUrls: ['./clients-list.component.scss']
})
export class ClientsListComponent implements OnInit {
    page = new Page(1, 100);
    pending: boolean;
    rows: Array<IClient> = [];
    pageSizeOptions: number[] = [10, 25, 50, 100];

    constructor(private _clientsService: ClientsService) {
    }

    ngOnInit() {
        this.loadClients();
    }

    loadPage(page: PageEvent) {
        this.page.pageNumber = page.pageIndex + 1;
        this.page.size = page.pageSize;
        this.loadClients();
    }

    loadClients() {
        this.pending = true;

        this._clientsService.getClients(this.page).subscribe(data => {
            this.pending = false;
            this.rows = data.items;
            this.page.total = data.total;
        }, error => {
            this.pending = false;
        });
    }
}
