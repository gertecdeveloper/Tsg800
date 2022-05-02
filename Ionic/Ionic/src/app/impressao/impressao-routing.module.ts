import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { ImpressaoPage } from './impressao.page';

const routes: Routes = [
  {
    path: '',
    component: ImpressaoPage
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ImpressaoPageRoutingModule {}
