import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DevicesRoutingModule, routedComponents } from './devices-routing.module';
import { SharedModule } from '../shared';
import { FormsModule } from '@angular/forms';
import { DevicesService } from './services/devices.service';
import { DeviceFormComponent } from './components/device-form/device-form.component';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSelectModule } from '@angular/material';

@NgModule({
    declarations: [
        ...routedComponents,
        DeviceFormComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        DevicesRoutingModule,
        SharedModule,
        MatPaginatorModule,
        MatSelectModule
    ],
    providers: [
        DevicesService
    ]
})
export class DevicesModule {
}
