import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { UsersComponent } from './components/users/users.component';
import { UsersListComponent } from './components/users-list/users-list.component';
import { UserCreateComponent } from './components/user-create/user-create.component';
import { UserEditComponent } from './components/user-edit/user-edit.component';

const routes: Routes = [
    {
        path: '',
        component: UsersComponent,
        children: [
            { path: '', component: UsersListComponent },
            { path: 'create', component: UserCreateComponent },
            { path: ':userId', component: UserEditComponent }
        ]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
    declarations: [],
})
export class UsersRoutingModule {
}

export const routedComponents = [
    UsersComponent,
    UsersListComponent,
    UserCreateComponent,
    UserEditComponent
];
