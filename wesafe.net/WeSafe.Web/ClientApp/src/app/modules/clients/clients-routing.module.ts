import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ClientsComponent } from './components/clients/clients.component';
import { ClientsListComponent } from './components/clients-list/clients-list.component';
import { ClientComponent } from './components/client/client.component';
import { ClientEditComponent } from './components/client-edit/client-edit.component';

const routes: Routes = [
    {
        path: '',
        component: ClientsComponent,
        children: [
            { path: '', component: ClientsListComponent },
            { path: ':clientId', component: ClientComponent },
            { path: ':clientId/edit', component: ClientEditComponent }
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
export class ClientsRoutingModule {
}

export const routedComponents = [
    ClientsComponent,
    ClientsListComponent,
    ClientComponent,
    ClientEditComponent
];
