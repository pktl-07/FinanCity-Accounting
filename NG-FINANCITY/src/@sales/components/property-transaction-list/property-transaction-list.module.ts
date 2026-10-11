import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { PGridViewModule } from 'src/@common/components/p-grid-view/p-grid-view.module';
import { PropertyTransactionListComponent } from './property-transaction-list.component'
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [PropertyTransactionListComponent],
  imports: [
    CommonModule,
    PToolbarModule,
    PWindowModule,
    PGridViewModule,
    RouterModule.forChild([{ path: '', component: PropertyTransactionListComponent }])
  ]
})
export class PropertyTransactionListModule { }
