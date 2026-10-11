import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PSalesStatusComponent } from './p-sales-status.component';

@NgModule({

  declarations: [PSalesStatusComponent],
  imports: [
    CommonModule,
  ],
  exports: [PSalesStatusComponent],
})
export class PSalesStatusModule { }
