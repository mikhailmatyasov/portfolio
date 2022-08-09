import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from "../../services/auth.guard";
import { IpsListComponent } from "../../components/ips-list/ips-list.component";
import { DevicesLogsListComponent } from "../../components/devices-logs-list/devices-logs-list.component";
import { EmailsListComponent } from "../../components/emails-list/emails-list.component";
import { UnhandledExceptionsListComponent } from
    "../../components/unhandled-exceptions-list/unhandled-exceptions-list.component";
import { AdminRootComponent } from "./components/admin-root/admin-root.component";
import { Roles } from "../../models/auth";
import { CameraRoiComponent } from "../../components/camera-roi/camera-roi.component";
import { RecognitionObjectsComponent } from '../../components/recognition-objects/recognition-objects.component';


const routes: Routes = [
    {
        path: '', component: AdminRootComponent, data: { anonymous: false }, canActivate: [AuthGuard], children:
            [
                { path: 'ips', component: IpsListComponent, data: { anonymous: false }, canActivate: [AuthGuard] },
                { path: 'deviceslogslist', component: DevicesLogsListComponent, data: { anonymous: false }, canActivate: [AuthGuard] },
                { path: 'emails', component: EmailsListComponent, data: { anonymous: false }, canActivate: [AuthGuard] },
                { path: 'unhandledExceptions', component: UnhandledExceptionsListComponent, data: { anonymous: false }, canActivate: [AuthGuard] },
                {
                    path: 'users',
                    loadChildren: () => import('../../modules/users').then(module => module.UsersModule),
                    data: { role: Roles.Administrators },
                    canActivate: [AuthGuard],
                    canLoad: [AuthGuard]
                },
                {
                    path: 'devices',
                    loadChildren: () => import('../../modules/devices').then(module => module.DevicesModule),
                    data: { role: Roles.Administrators },
                    canActivate: [AuthGuard],
                    canLoad: [AuthGuard]
                },
                {
                    path: 'clients',
                    loadChildren: () => import('../../modules/clients').then(module => module.ClientsModule),
                    data: { role: Roles.Administrators },
                    canActivate: [AuthGuard],
                    canLoad: [AuthGuard]
                },
                {
                    path: 'demo',
                    loadChildren: () => import('../../modules/demo').then(module => module.DemoModule),
                    data: { anonymous: false, demo: true },
                    canActivate: [AuthGuard],
                    canLoad: [AuthGuard]
                },
                {
                    path: 'device/:deviceId/cameras/:cameraId/roi',
                    component: CameraRoiComponent,
                    data: { anonymous: false },
                    canActivate: [AuthGuard]
                },
                {
                    path: 'recognitionobjects',
                    component: RecognitionObjectsComponent,
                    data: { role: Roles.Administrators },
                    canActivate: [AuthGuard]
                }
            ]
    },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdminRoutingModule { }
