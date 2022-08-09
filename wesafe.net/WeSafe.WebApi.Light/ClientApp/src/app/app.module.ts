import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule, routedComponents } from './app-routing.module';
import { AppComponent } from './app.component';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { ApiHttpInterceptor } from './services/http.interceptor';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AuthGuard } from './services/auth.guard';
import { AuthService } from './services/auth.service';
import { LocalStorage } from './services/local-storage.service';
import { SharedModule } from './modules/shared';
import { PrivateClientService } from './services/private-client.service';
import { CameraEditComponent } from './components/camera-edit/camera-edit.component';
import { CameraTypeService } from './services/camera-type.service';
import { CameraManufactorService } from './services/camera-manufactor.service';
import { AlertErrorPopupComponent } from './components/alert-error-popup/alert-error-popup.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { AlertService } from './services/alert-error-popup.service';
import { OwlDateTimeModule, OwlNativeDateTimeModule, OWL_DATE_TIME_LOCALE  } from 'ng-pick-datetime';
import { AssignmentModalComponent } from './components/assignment-modal/assignment-modal.component';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule, MatSnackBarModule, MatTabsModule } from '@angular/material';
import { RegistrationCodeService } from './services/registration-code.service';
import { MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatSliderModule } from '@angular/material/slider';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatPaginatorModule } from '@angular/material/paginator';
import { LoginHomeComponent } from './components/login-home/login-home.component';
import { AddAlertUserComponent } from './components/alert-users-list/modals/add-alert-user/add-alert-user.component';
import { ConfirmationModalComponent } from './components/confirmation-modal/confirmation-modal.component';
import { LoginValidator } from './validators/login-validator';
import { AreaFormComponent } from './components/camera-roi/editors/roi-editor-ver2/area-form/area-form.component';
import { EditDeviceNameComponent } from './components/device-info/modals/edit-device-name/edit-device-name.component';
import { TrafficService } from './services/traffic.service';
import { AlprService } from './services/alpr.service';
import { LayoutComponent } from './components/layout/layout.component';
import { LicensePlateRestrictionService } from './services/license-plate-restriction.service';
import { DeviceIndicatorsService } from './services/device-indicators.service';
import { MenuComponent } from './components/menu/menu.component';
import { SettingsComponent } from './components/settings/settings.component';
import { GlobalSettingsService } from './services/global-settings.service';
import { EventsDashboardComponent } from './components/events-dashboard/events-dashboard.component';
import { VideoService } from './services/video.service';
import { EventVideoDialogComponent } from './components/event/event-video-dialog/event-video-dialog.component';

const httpInterceptorProviders = [
    { provide: HTTP_INTERCEPTORS, useClass: ApiHttpInterceptor, multi: true }
];

@NgModule({
    declarations: [
        ...routedComponents,
        AppComponent,
        AlertErrorPopupComponent,
        AssignmentModalComponent,
        LoginHomeComponent,
        EditDeviceNameComponent,
        AddAlertUserComponent,
        ConfirmationModalComponent,
        LoginValidator,
        AreaFormComponent,
        LayoutComponent,
        MenuComponent,
        SettingsComponent,
        EventsDashboardComponent,
        EventVideoDialogComponent
    ],
    imports: [
        FormsModule,
        MatIconModule,
        BrowserModule,
        AppRoutingModule,
        BrowserAnimationsModule,
        HttpClientModule,
        SharedModule,
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
        MatPaginatorModule,
        ReactiveFormsModule,
        MatTabsModule
    ],
    providers: [
        httpInterceptorProviders,
        AuthGuard,
        LocalStorage,
        AuthService,
        PrivateClientService,
        CameraTypeService,
        CameraManufactorService,
        AlertService,
        RegistrationCodeService,
        TrafficService,
        AlprService,
        LicensePlateRestrictionService,
        DeviceIndicatorsService,
        GlobalSettingsService,
        VideoService,

        { provide: OWL_DATE_TIME_LOCALE, useValue: 'ru' }
    ],
    exports: [
        CameraEditComponent
    ],
    entryComponents: [
        AssignmentModalComponent,
        EditDeviceNameComponent,
        AddAlertUserComponent,
        ConfirmationModalComponent,
        AreaFormComponent,
        EventVideoDialogComponent
    ],
    bootstrap: [AppComponent]
})
export class AppModule {
}
