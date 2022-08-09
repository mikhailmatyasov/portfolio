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
import { PermitedAdminIpService } from './services/permitted-admin-ip.service';
import { CameraEditComponent } from './components/camera-edit/camera-edit.component';
import { CameraManufactorService } from "./modules/camera-manufactors/services/camera-manufactor.service";
import { AlertErrorPopupComponent } from './components/alert-error-popup/alert-error-popup.component';
import { DevicesLogsService } from './services/devices-logs.service';
import { ClientsService } from './modules/clients/services/clients.service';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { AlertService } from './services/alert-error-popup.service';
import { UnhandledExceptionService } from './services/unhandled-exception.service';
import { EmailService } from './services/email.service';
import { OwlDateTimeModule, OwlNativeDateTimeModule, OWL_DATE_TIME_LOCALE  } from 'ng-pick-datetime';
import { UsersService } from './modules/users/services/users.service';
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
import { AdminModule } from './modules/admin';
import { AreaFormComponent } from './components/camera-roi/editors/roi-editor-ver2/area-form/area-form.component';
import { EditDeviceNameComponent } from "./components/device-info/modals/edit-device-name/edit-device-name.component";
import { TrafficService } from "./services/traffic.service";
import { AlprService } from './services/alpr.service';
import { LayoutComponent } from './components/layout/layout.component';
import { LicensePlateRestrictionService } from './services/license-plate-restriction.service';
import { DeviceIndicatorsService } from './services/device-indicators.service';
import { RecognitionObjectsService } from './services/recognition-objects.service';
import { ConnectCameraModalComponent } from './components/cameras-list/connect-camera-modal/connect-camera-modal.component';
import { MatAutocompleteModule } from '@angular/material/autocomplete';

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
        ConnectCameraModalComponent,
    ],
    imports: [
        FormsModule,
        MatIconModule,
        BrowserModule,
        AppRoutingModule,
        BrowserAnimationsModule,
        HttpClientModule,
        SharedModule,
        AdminModule,
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
        MatAutocompleteModule
    ],
    providers: [
        httpInterceptorProviders,
        AuthGuard,
        LocalStorage,
        AuthService,
        PrivateClientService,
        ClientsService,
        CameraManufactorService,
        PermitedAdminIpService,
        AlertService,
        DevicesLogsService,
        UnhandledExceptionService,
        EmailService,
        UsersService,
        RegistrationCodeService,
        TrafficService,
        AlprService,
        LicensePlateRestrictionService,
        DeviceIndicatorsService,
        RecognitionObjectsService,

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
        ConnectCameraModalComponent
    ],
    bootstrap: [AppComponent]
})
export class AppModule {
}
