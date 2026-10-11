import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SalesOrderTypeComponent } from './sales-order-type-list.component';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { PGridViewModule } from 'src/@common/components/p-grid-view/p-grid-view.module';
import { SalesOrderTypeDetailModule } from '../sales-order-type-detail/sales-order-type-detail.module';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [SalesOrderTypeComponent],
  imports: [
    CommonModule,
    PToolbarModule,
    PWindowModule,
    PGridViewModule,
    SalesOrderTypeDetailModule,
    RouterModule.forChild([{ path: '', component: SalesOrderTypeComponent }])
  ]
})
export class SalesOrderTypeListModule { }
