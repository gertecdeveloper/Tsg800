import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { NfcLerGravarPage } from './nfc-ler-gravar.page';

const routes: Routes = [
  {
    path: '',
    component: NfcLerGravarPage
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class NfcLerGravarPageRoutingModule {}
