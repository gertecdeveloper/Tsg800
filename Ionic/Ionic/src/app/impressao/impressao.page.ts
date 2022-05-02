import { Component, OnInit } from '@angular/core';
import { AlertController } from '@ionic/angular';
declare var gpos8centos;

@Component({
  selector: 'app-impressao',
  templateUrl: './impressao.page.html',
  styleUrls: ['./impressao.page.scss'],
})
export class ImpressaoPage implements OnInit {
  gpos700: any;
  teste: any;
  mensagem: string;
  alinhamentoValue: string;
  listSelecionado = [false, false, false];
  alinhar:string = "";
  left:string = "LEFT";
  center:string = "CENTER";
  right:string = "RIGHT";
  font:string = "DEFAULT";
  size:string = "60";
  height:number = 280;
  width:number = 280;
  barCode:string = "QR_CODE";

  constructor(public alertController: AlertController) { }

  ngOnInit() {
  }

  async presentAlert(msg) {
    const alert = await this.alertController.create({
      // header: 'Escreva uma mensagem!',
      message: msg
    });
    await alert.present();
  }

  checarImpressora() {
    gpos8centos.checarImpressora((res) => {
    }, (err) => {
      console.log(err);
    });
  }
  
  radioGroupChange(event) {
    this.alinhamentoValue = event.detail['value'];
    switch (this.alinhamentoValue) {
      case 'LEFT':
        this.alinhar = this.alinhamentoValue;
        break;
      case 'CENTER':
        this.alinhar = this.alinhamentoValue;
        break;
      case 'RIGHT':
        this.alinhar = this.alinhamentoValue;
        break;
      default:
        this.alinhar = null;
    }
  }

  optionsFont(event) {
    this.font = event.detail['value'];
  }

  optionsSize(event) {
    this.size = event.detail['value'];
  }

  optionsBarCodeHeight(event) {
    this.height = parseInt(event.detail['value']);
  }

  optionsBarCodeWidth(event) {
    this.width = parseInt(event.detail['value']);
  }

  optionsBarCodeTipo(event) {
    this.barCode = event.detail['value'];
  }

  imprimirTexto() {
    // this.gpos700.imprimir();
    if(this.mensagem == null || this.mensagem == "") {
      this.presentAlert("Escreva uma mensagem");
      return;
    }
    if(this.alinhar == null || this.alinhar == "") {
      this.presentAlert("Selecione um alinhamento");
      return;
    }
    // gpos8centos.imprimir();
    gpos8centos.imprimir({
      tipoImpressao: "Texto", 
      mensagem: this.mensagem,
      alinhar: this.alinhar,
      opNegrito: this.listSelecionado[0],
      opItalico: this.listSelecionado[1],
      opSublinhado: this.listSelecionado[2],
      font: this.font,
      size: this.size
    }, (res) => console.log(res), 
       (err) => console.log(err)
    );

    gpos8centos.impressoraOutput(
      {},
      (res) => console.log(res),
      (err) => console.log(err)
    );
  }

  todasFunc() {
    gpos8centos.imprimir({
      tipoImpressao: "TodasFuncoes"
    }, (res) => console.log(res),
       (err) => console.log(err)
    );

    gpos8centos.impressoraOutput(
      {},
      (res) => console.log(res),
      (err) => console.log(err)
    );
  }

  imprimirImagem() {
    gpos8centos.imprimir({
      tipoImpressao: "Imagem"
    }, (res) => console.log(res),
       (err) => console.log(err)
    );

    gpos8centos.impressoraOutput(
      {},
      (res) => console.log(res),
      (err) => console.log(err)
    );
  }

  imprimirBarCode() {
    if(this.mensagem == null || this.mensagem == "") {
      this.presentAlert("Escreva uma mensagem");
      return;
    }

    gpos8centos.imprimir({
      tipoImpressao: "CodigoDeBarra",
      height: this.height,
      width: this.width,
      barCode: this.barCode,
      mensagem: this.mensagem
    }, (res) => console.log(res),
       (err) => console.log(err)
    );

    gpos8centos.impressoraOutput(
      {"avancaLinha": 40},
      (res) => console.log(res),
      (err) => console.log(err)
    );
  }
}
