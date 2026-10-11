import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { PGridViewModule } from 'src/@common/components/p-grid-view/p-grid-view.module';
import { SalesInvoiceListComponent } from './sales-invoice-list.component'
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [SalesInvoiceListComponent],
  imports: [
    CommonModule,
    PToolbarModule,
    PWindowModule,
    PGridViewModule,
    RouterModule.forChild([{ path: '', component: SalesInvoiceListComponent }])
  ]
})
export class SalesInvoiceListModule { }
