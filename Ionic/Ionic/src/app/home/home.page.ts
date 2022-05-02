import { Component } from '@angular/core';
import { Camera, CameraOptions } from '@ionic-native/camera/ngx';
import { Router } from '@angular/router';
declare var gpos8centos;

@Component({
  selector: 'app-home',
  templateUrl: 'home.page.html',
  styleUrls: ['home.page.scss'],
})

export class HomePage {
  temperatura: any;
  // gpos800: any;

  constructor(
    private camera: Camera,
    private router: Router
  ) 
  { }
  
  codigobarras1() {
    this.router.navigate(['codigobarras1']);
  }

  codbarrasv2() {
    gpos8centos.leitorCodigoV2((res) => 
      console.log(res), 
      (err) => console.log(err)
    );
  }

  impressao() {
    this.router.navigate(['imprimir']);
  }

  nfcLerGravar() {
    this.router.navigate(['nfc-ler-gravar']);
  }
}
