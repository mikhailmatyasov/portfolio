import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AdminRoutingModule } from './admin-routing.module';
import { AdminRootComponent } from './components/admin-root/admin-root.component';
import { SharedModule } from "../shared";
import { IpsListComponent } from "../../components/ips-list/ips-list.component";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import {
    MatButtonModule, MatFormFieldModule, MatInputModule, MatDialogModule, MatIconModule, MatSnackBarModule,
    MatSelectModule,
    MatSliderModule,
    MatCheckboxModule
} from "@angular/material";
import { DevicesLogsListComponent } from "../../components/devices-logs-list/devices-logs-list.component";
import { NgMultiSelectDropDownModule } from "ng-multiselect-dropdown";
import { OwlDateTimeModule, OwlNativeDateTimeModule } from "ng-pick-datetime";
import { EmailsListComponent } from "../../components/emails-list/emails-list.component";
import { UnhandledExceptionsListComponent } from
        "../../components/unhandled-exceptions-list/unhandled-exceptions-list.component";
import { MatPaginatorModule } from '@angular/material/paginator';
import { RecognitionObjectsComponent } from '../../components/recognition-objects/recognition-objects.component';
import { RecognitionObjectFormComponent } from '../../components/recognition-objects/modals/recognition-object-form/recognition-object-form.component';

@NgModule({
    declarations: [
        AdminRootComponent,
        IpsListComponent,
        DevicesLogsListComponent,
        EmailsListComponent,
        UnhandledExceptionsListComponent,
        RecognitionObjectsComponent,
        RecognitionObjectFormComponent
    ],
    imports: [
        MatButtonModule,
        MatFormFieldModule,
        MatInputModule,
        MatDialogModule,
        MatIconModule,
        MatSnackBarModule,
        MatSelectModule,
        MatSliderModule,
        MatCheckboxModule,
        ReactiveFormsModule,
        FormsModule,
        SharedModule,
        CommonModule,
        AdminRoutingModule,
        NgMultiSelectDropDownModule,
        OwlDateTimeModule,
        OwlNativeDateTimeModule,
        MatPaginatorModule,
    ],
    entryComponents: [
        RecognitionObjectFormComponent
    ]
})
export class AdminModule {
}
