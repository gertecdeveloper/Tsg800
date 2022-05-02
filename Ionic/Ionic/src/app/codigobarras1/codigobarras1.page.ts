import { Component, OnInit } from '@angular/core';
declare var gpos8centos;

@Component({
  selector: 'app-codigobarras1',
  templateUrl: './codigobarras1.page.html',
  styleUrls: ['./codigobarras1.page.scss'],
})
export class Codigobarras1Page implements OnInit {
  gpos700: any;
  resultadosBarCod: any;
  items = [];

  constructor() { }

  ngOnInit() {
  }

  EAN_8() {
    // this.teste = this.gpos700.leitorCodigo1("EAN_8");
    // this.gpos700.leitorCodigo1("EAN_8");
    gpos8centos.leitorCodigo1({barCode: "EAN_8"}, (res) => {
      this.items.push(res);
    }, (er) => 
    {
      this.items.push(er);
    });
  }

  EAN_13() {
    gpos8centos.leitorCodigo1({barCode: "EAN_13"}, (res) => {
      this.items.push(res);
    }, (er) => 
    {
      this.items.push(er);
    });
  }

  EAN_14(){
    gpos8centos.leitorCodigo1({barCode: "EAN_14"}, (res) => {
      this.items.push(res);
    }, (er) => 
    {
      this.items.push(er);
    });
  }

  QR_CODE(){
    gpos8centos.leitorCodigo1({barCode: "QR_CODE"}, (res) => {
      this.items.push(res);
    }, (er) => 
    {
      this.items.push(er);
    });
  }
}
