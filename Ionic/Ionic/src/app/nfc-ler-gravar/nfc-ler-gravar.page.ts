import { Component, OnInit } from '@angular/core';
import { AlertController } from '@ionic/angular';
declare var gpos8centos;

@Component({
  selector: 'app-nfc-ler-gravar',
  templateUrl: './nfc-ler-gravar.page.html',
  styleUrls: ['./nfc-ler-gravar.page.scss'],
})
export class NfcLerGravarPage implements OnInit {
  mensagem: string;

  constructor(public alertController: AlertController) { }

  ngOnInit() {
  }

  async presentAlert() {
    const alert = await this.alertController.create({
      // header: 'Escreva uma mensagem!',
      message: 'Escreva uma mensagem'
    });
    await alert.present();
  }

  lerNfc() {
    gpos8centos.lerNfc(
      (su) => {console.log(su)},
      (er) => {console.log(er)});
  }

  gravarNfc() {
    if(this.mensagem == null || this.mensagem == "") {
      this.presentAlert();
      return;
    }
    
    gpos8centos.gravarNfc({mensagemGravar: this.mensagem},
      (su) => {console.log(su)},
      (er) => {console.log(er)});
  }
  
  formatarNfc() {
    gpos8centos.formatarNfc(
      (su) => {console.log(su)},
      (er) => {console.log(er)});
  }
  
  testeNfc() {
    gpos8centos.testeNfc(
      (su) => {console.log(su)},
      (er) => {console.log(er)});
  }
  
}
