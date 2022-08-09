import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { routedComponents, UsersRoutingModule } from './users-routing.module';
import { UsersService } from './services/users.service';
import { SharedModule } from '../shared';
import { UserFormComponent } from './components/user-form/user-form.component';
import { FormsModule } from '@angular/forms';
import { MatPaginatorModule } from '@angular/material/paginator';

@NgModule({
    declarations: [
        ...routedComponents,
        UserFormComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        SharedModule,
        UsersRoutingModule,
        MatPaginatorModule
    ],
    providers: [
        UsersService
    ]
})
export class UsersModule {
}
