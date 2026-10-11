import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IonicModule } from '@ionic/angular';
import { FCFileOpenerPage } from './fc-file-opener.page';

@NgModule({
  declarations: [FCFileOpenerPage],
  imports: [CommonModule, IonicModule],
  exports: [FCFileOpenerPage],
})
export class FCFileOpenerModule { }
