import { Component, OnInit } from '@angular/core';
import { Page } from '../../models/page';
import { UnhandledExceptionService } from '../../services/unhandled-exception.service';
import { IUnhandledException, IUnhandledExceptionFilter, UnhandledExceptionFilter } from '../../models/unhandled-exception';
import { NgbInputDatepicker } from '@ng-bootstrap/ng-bootstrap';
import { UsersService } from '../../modules/users/services/users.service';
import { IUser } from '../../modules/users/models/user';
import { PageEvent } from '@angular/material/paginator';

@Component({
  selector: 'app-unhandled-exceptions-list',
  templateUrl: './unhandled-exceptions-list.component.html',
    styleUrls: ['./unhandled-exceptions-list.component.scss']
})
export class UnhandledExceptionsListComponent implements OnInit {
    rows: Array<IUnhandledException> = [];
    page = new Page(1, 100);
    users: Array<IUser> = [];
    searchErrorText: string;
    selectedUserName: string = "Any";
    selectedStartDate: Date;
    selectedEndDate: Date;
    pageSizeOptions: number[] = [10, 25, 50, 100];

    constructor(private _unhandledExceptionService: UnhandledExceptionService, private _userService: UsersService) {
    }

    ngOnInit() {
        this.getUsers();
        this.loadUnhandledExceptions();
    }

    loadPage(page: PageEvent) {
        this.page.pageNumber = page.pageIndex + 1;
        this.page.size = page.pageSize;
        this.loadUnhandledExceptions();
    }

    loadUnhandledExceptions() {
        let filter = this.getUnhandledExceptionFilterModel();
        this._unhandledExceptionService.getUnhandledExceptions(this.page, filter).subscribe(data => {
            this.rows = data.items;
            this.page.total = data.total;
        }, error => {
        });
    }

    getUnhandledExceptionFilterModel(): UnhandledExceptionFilter {
        let filter = new UnhandledExceptionFilter();

        if (this.selectedStartDate)
            filter.fromDate =
                this.getDateFormat(this.selectedStartDate);

        if (this.selectedEndDate)
            filter.toDate =
                this.getDateFormat(this.selectedEndDate);

        if (this.selectedUserName.toLocaleLowerCase() != "any")
            filter.userName = this.selectedUserName;

        if (this.searchErrorText)
            filter.searchText = this.searchErrorText.toLocaleLowerCase();

        return filter;
    }

    getUsers() {
        this._userService.getUsers(new Page(1, null)).subscribe(data => {
            this.users = data.items;
        }, error => {
        });
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
