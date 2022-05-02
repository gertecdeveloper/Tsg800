import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { IonicModule } from '@ionic/angular';

import { NfcLerGravarPage } from './nfc-ler-gravar.page';

describe('NfcLerGravarPage', () => {
  let component: NfcLerGravarPage;
  let fixture: ComponentFixture<NfcLerGravarPage>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ NfcLerGravarPage ],
      imports: [IonicModule.forRoot()]
    }).compileComponents();

    fixture = TestBed.createComponent(NfcLerGravarPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }));

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
