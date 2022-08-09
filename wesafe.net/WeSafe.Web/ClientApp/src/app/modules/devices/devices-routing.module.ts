import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DevicesComponent } from './components/devices/devices.component';
import { DevicesListComponent } from './components/devices-list/devices-list.component';
import { DeviceCreateComponent } from './components/device-create/device-create.component';
import { DeviceEditComponent } from './components/device-edit/device-edit.component';
import { CameraEditComponent } from "../../components/camera-edit/camera-edit.component";
import { AuthGuard } from "../../services/auth.guard";

const routes: Routes = [
    {
        path: '',
        component: DevicesComponent,
        children: [
            { path: '', component: DevicesListComponent },
            { path: 'create', component: DeviceCreateComponent },
            { path: ':deviceId', component: DeviceEditComponent },
            {
                path: ':deviceId/cameras/create',
                component: CameraEditComponent,
                data: { anonymous: false, create: true },
                canActivate: [AuthGuard]
            },
            {
                path: ':deviceId/cameras/:cameraId',
                component: CameraEditComponent,
                data: { anonymous: false, create: false },
                canActivate: [AuthGuard]
            }
        ]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [
        RouterModule
    ],
    declarations: [],
})
export class DevicesRoutingModule {
}

export const routedComponents = [
    DevicesComponent,
    DevicesListComponent,
    DeviceCreateComponent,
    DeviceEditComponent
];
