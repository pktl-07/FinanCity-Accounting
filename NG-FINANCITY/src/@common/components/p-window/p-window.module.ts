import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { WindowModule } from '@progress/kendo-angular-dialog';
import { PWindowComponent } from './p-window.component';

@NgModule({
  entryComponents: [PWindowComponent],
  declarations: [PWindowComponent],
  imports: [
    CommonModule,
    WindowModule
  ],
  exports: [PWindowComponent]
})
export class PWindowModule { }
