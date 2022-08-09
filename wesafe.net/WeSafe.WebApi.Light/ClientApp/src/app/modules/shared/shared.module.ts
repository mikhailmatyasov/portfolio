import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { NavHeaderComponent } from './nav-header';
import { UserInfoComponent } from './user-info';
import { NgbPaginationModule, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { DeviceDetailsComponent } from './device-details/device-details.component';
import { IconPencilComponent } from './icon-pencil/icon-pencil.component';
import { IconTrashComponent } from './icon-trash/icon-trash.component';
import { NgxMaskModule } from 'ngx-mask';
import { Ng5SliderModule } from 'ng5-slider';
import { IconBellComponent } from './icon-bell/icon-bell.component';
import { UserDevicesListComponent } from '../../components/user-devices-list/user-devices-list.component';
import { MatTableModule, MatButtonModule, MatFormFieldModule, MatInputModule, MatDialogModule, MatIconModule,
    MatSnackBarModule,
    MatSelectModule,
    MatSliderModule,
    MatCheckboxModule,
    MatTabsModule
} from '@angular/material';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { OwlDateTimeModule, OwlNativeDateTimeModule } from 'ng-pick-datetime';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { DashboardComponent } from '../../components/dashboard/dashboard.component';
import { CameraEditComponent } from '../../components/camera-edit/camera-edit.component';
import { CameraRoiComponent } from '../../components/camera-roi/camera-roi.component';
import { CamerasListComponent } from '../../components/cameras-list/cameras-list.component';
import { AlertUsersListComponent } from '../../components/alert-users-list/alert-users-list.component';
import { RoiEditorVer1Component } from '../../components/camera-roi/editors/roi-editor-ver1/roi-editor-ver1.component';
import { RoiEditorVer2Component } from '../../components/camera-roi/editors/roi-editor-ver2/roi-editor-ver2.component';
import { BindDeviceDialog } from '../../components/user-devices-list/bind-device-dialog/bind-device-dialog.component';
import { EventsComponent } from '../../components/events/events.component';
import { MatPaginatorModule } from '@angular/material/paginator';
import { BackToComponent } from '../../components/back-to/back-to.component';
import { DeviceInfoComponent } from '../../components/device-info/device-info.component';
import { TrafficComponent } from '../../components/traffic/traffic.component';
import { AlprComponent } from '../../components/alpr/alpr.component';
import { LicensePlateRestrictionsComponent } from '../../components/alpr/license-plate-restrictions/license-plate-restrictions.component';
import { AlprEventsComponent } from '../../components/alpr/alpr-events/alpr-events.component';
import { IndicatorGaugeComponent } from '../../components/indicator-gauge/indicator-gauge.component';
import { GoogleChartsModule } from 'angular-google-charts';
import { IconDashboardComponent } from './icon-dashboard/icon-dashboard.component';
import { IconEventsComponent } from './icon-events/icon-events.component';
import { IconSettingsComponent } from './icon-settings/icon-settings.component';
import { EventComponent } from '../../components/event/event.component';
import { EventVideosComponent } from '../../components/event-videos/event-videos.component';

@NgModule({
    declarations: [
        NavHeaderComponent,
        UserInfoComponent,
        DeviceDetailsComponent,
        IconPencilComponent,
        IconTrashComponent,
        IconBellComponent,
        UserDevicesListComponent,
        DashboardComponent,
        EventsComponent,
        CameraEditComponent,
        CameraRoiComponent,
        CamerasListComponent,
        AlertUsersListComponent,
        RoiEditorVer1Component,
        RoiEditorVer2Component,
        BindDeviceDialog,
        BackToComponent,
        DeviceInfoComponent,
        TrafficComponent,
        AlprComponent,
        AlprEventsComponent,
        LicensePlateRestrictionsComponent,
        IndicatorGaugeComponent,
        IconDashboardComponent,
        IconEventsComponent,
        IconSettingsComponent,
        EventComponent,
        EventVideosComponent
    ],
    exports: [
        BindDeviceDialog,
        RoiEditorVer1Component,
        RoiEditorVer2Component,
        AlertUsersListComponent,
        CamerasListComponent,
        CameraRoiComponent,
        CameraEditComponent,
        DashboardComponent,
        UserDevicesListComponent,
        NavHeaderComponent,
        UserInfoComponent,
        DeviceDetailsComponent,
        NgbPaginationModule,
        IconPencilComponent,
        IconTrashComponent,
        IconBellComponent,
        NgxMaskModule,
        Ng5SliderModule,
        IconDashboardComponent,
        IconEventsComponent,
        IconSettingsComponent,
        EventComponent,
        EventVideosComponent,
        EventsComponent,
    ],
    imports: [
        FormsModule,
        MatTableModule,
        CommonModule,
        RouterModule,
        NgbPaginationModule,
        NgxMaskModule.forRoot(),
        Ng5SliderModule,
        NgbModule,
        NgMultiSelectDropDownModule,
        OwlDateTimeModule,
        OwlNativeDateTimeModule,
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
        MatPaginatorModule,
        MatTabsModule,
        GoogleChartsModule,
    ],
    entryComponents: [BindDeviceDialog]
})
export class SharedModule {
}
