import { Component, OnInit } from '@angular/core';
import { Page } from '../../../../models/page';
import { IUser } from '../../models/user';
import { UsersService } from '../../services/users.service';
import { PageEvent } from '@angular/material/paginator';

@Component({
    selector: 'app-users-list',
    templateUrl: './users-list.component.html',
    styleUrls: ['./users-list.component.scss']
})
export class UsersListComponent implements OnInit {
    page = new Page(1, 100);
    pending: boolean;
    rows: Array<IUser> = [];
    pageSizeOptions: number[] = [10, 25, 50, 100];

    constructor(private _userService: UsersService) {
    }

    ngOnInit() {
        this.loadUsers();
    }

    loadPage(page: PageEvent) {
        this.page.pageNumber = page.pageIndex + 1;
        this.page.size = page.pageSize;
        this.loadUsers();
    }

    loadUsers() {
        this.pending = true;

        this._userService.getUsers(this.page).subscribe(data => {
            this.pending = false;
            this.rows = data.items;
            this.page.total = data.total;
        }, error => {
            this.pending = false;
        });
    }

    unlockUser(userId) {
        this._userService.unlockUser(userId).subscribe(data => {
            this.loadUsers();
        }, error => {
        });
    }
}
