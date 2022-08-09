import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from './services/auth.guard';
import { NotFoundComponent } from './components/not-found';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { LoginComponent } from './components/login/login.component';
import { HomeComponent } from './components/home/home.component';
import { UserDevicesListComponent } from './components/user-devices-list/user-devices-list.component';
import { CameraEditComponent } from './components/camera-edit/camera-edit.component';
import { CameraRoiComponent } from './components/camera-roi/camera-roi.component';
import { ProfileComponent } from './components/profile/profile.component';
import { RegistrationComponent } from './components/registration/registration.component';
import { Roles } from "./models/auth";
import { LayoutComponent } from './components/layout/layout.component';

const routes: Routes = [
    { path: '', component: HomeComponent, data: { anonymous: true } },
    {
        path: 'devices',
        component: LayoutComponent,
        data: { anonymous: false },
        canActivate: [AuthGuard],
        children: [
            {
                path: '',
                component: UserDevicesListComponent,
                data: { anonymous: false },
                canActivate: [AuthGuard]
            },
            {
                path: ':deviceId',
                component: DashboardComponent,
                data: { anonymous: false, create: true },
                canActivate: [AuthGuard]
            },
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
            },
            {
                path: ':deviceId/cameras/:cameraId/roi',
                component: CameraRoiComponent,
                data: { anonymous: false },
                canActivate: [AuthGuard]
            }
        ]
    },
    {
        path: 'admin',
        loadChildren: () => import('./modules/admin').then(module => module.AdminModule),
        data: { anonymous: false, role:Roles.Administrators },
        canActivate: [AuthGuard],
        canLoad: [AuthGuard]
    },
    { path: 'login', component: LoginComponent, data: { anonymous: true } },
    { path: 'registration', component: RegistrationComponent, data: { anonymous: true } },
    { path: 'profile', component: ProfileComponent, data: { anonymous: false }, canActivate: [AuthGuard] },
    { path: '**', component: NotFoundComponent, data: { anonymous: true } }
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule {
}

export const routedComponents = [
    LoginComponent,
    NotFoundComponent,
    HomeComponent,
    ProfileComponent,
    RegistrationComponent
];
