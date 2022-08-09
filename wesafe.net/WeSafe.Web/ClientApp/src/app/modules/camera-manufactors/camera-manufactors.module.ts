import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CameraManufactorsRoutingModule } from './camera-manufactors-routing.module';
import { CameraManufactorService } from "./services/camera-manufactor.service";


@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    CameraManufactorsRoutingModule
    ],
  providers: [
      CameraManufactorService
  ]
})
export class CameraManufactorsModule { }
