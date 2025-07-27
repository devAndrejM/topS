﻿import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IonicModule } from '@ionic/angular';
import { BottomNavComponent } from './components/bottom-nav/bottom-nav.component';

@NgModule({
  declarations: [
    BottomNavComponent
  ],
  imports: [
    CommonModule,
    IonicModule
  ],
  exports: [
    BottomNavComponent
  ]
})
export class SharedModule { }
