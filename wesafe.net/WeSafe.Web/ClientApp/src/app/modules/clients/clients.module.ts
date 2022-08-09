import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ClientsRoutingModule, routedComponents } from './clients-routing.module';
import { SharedModule } from '../shared';
import { ClientsService } from './services/clients.service';
import { MatPaginatorModule } from '@angular/material/paginator';

@NgModule({
    declarations: [
        ...routedComponents
    ],
    imports: [
        CommonModule,
        FormsModule,
        ClientsRoutingModule,
        SharedModule,
        MatPaginatorModule
    ],
    providers: [
        ClientsService
    ]
})
export class ClientsModule {
}
