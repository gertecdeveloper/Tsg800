import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { IonicModule } from '@ionic/angular';

import { ImpressaoPage } from './impressao.page';

describe('ImpressaoPage', () => {
  let component: ImpressaoPage;
  let fixture: ComponentFixture<ImpressaoPage>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ImpressaoPage ],
      imports: [IonicModule.forRoot()]
    }).compileComponents();

    fixture = TestBed.createComponent(ImpressaoPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }));

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
