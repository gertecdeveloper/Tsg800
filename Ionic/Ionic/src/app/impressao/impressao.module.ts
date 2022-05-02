import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { IonicModule } from '@ionic/angular';

import { ImpressaoPageRoutingModule } from './impressao-routing.module';

import { ImpressaoPage } from './impressao.page';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    ImpressaoPageRoutingModule
  ],
  declarations: [ImpressaoPage]
})
export class ImpressaoPageModule {}
