import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DemoRoutingModule, routedComponents } from './demo-routing.module';
import { SharedModule } from '../shared';
import { DemoService } from './services/demo.service';
import { AllCamerasListComponent } from './components/all-cameras-list/all-cameras-list.component';
import { EventsComponent } from './components/events/events.component';
import { OwlDateTimeModule } from 'ng-pick-datetime';

@NgModule({
    declarations: [
        ...routedComponents,
        AllCamerasListComponent,
        EventsComponent,
    ],
    imports: [
        CommonModule,
        DemoRoutingModule,
        SharedModule,
        OwlDateTimeModule
    ],
    providers: [
        DemoService
    ]
})
export class DemoModule {
}
