import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { IonicModule } from '@ionic/angular';

import { NfcLerGravarPageRoutingModule } from './nfc-ler-gravar-routing.module';

import { NfcLerGravarPage } from './nfc-ler-gravar.page';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    NfcLerGravarPageRoutingModule
  ],
  declarations: [NfcLerGravarPage]
})
export class NfcLerGravarPageModule {}
