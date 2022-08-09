import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DemoRootComponent } from './components/demo-root/demo-root.component';
import { DemoComponent } from './components/demo/demo.component';
import { EventsComponent } from './components/events/events.component';

const routes: Routes = [
    {
        path: '',
        component: DemoRootComponent,
        children: [
            { path: '', component: DemoComponent },
            { path: 'events', component: EventsComponent }
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
export class DemoRoutingModule {
}

export const routedComponents = [
    DemoRootComponent,
    DemoComponent
];
