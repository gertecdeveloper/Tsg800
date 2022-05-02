import { NgModule } from '@angular/core';
import { PreloadAllModules, RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: 'home',
    loadChildren: () => import('./home/home.module').then( m => m.HomePageModule)
  },
  {
    path: '',
    redirectTo: 'home',
    pathMatch: 'full'
  },
  {
    path: 'codigobarras1',
    loadChildren: () => import('./codigobarras1/codigobarras1.module').then( m => m.Codigobarras1PageModule)
  },
  {
    path: 'imprimir',
    loadChildren: () => import('./impressao/impressao.module').then( m => m.ImpressaoPageModule)
  },
  {
    path: 'nfc-ler-gravar',
    loadChildren: () => import('./nfc-ler-gravar/nfc-ler-gravar.module').then( m => m.NfcLerGravarPageModule)
  },
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes, { preloadingStrategy: PreloadAllModules })
  ],
  exports: [RouterModule]
})
export class AppRoutingModule { }
