import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { ClientManagementDetailModule } from '../client-management-detail/client-management-detail.module';
import { PGridViewModule } from 'src/@common/components/p-grid-view/p-grid-view.module';
import { SalespersonManagementListComponent } from './salesperson-management-list.component';
import { SalespersonManagementDetailModule } from '../salesperson-management-detail/salesperson-management-detail.module';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [SalespersonManagementListComponent],
  imports: [
    CommonModule,
    PToolbarModule,
    PWindowModule,
    ClientManagementDetailModule,
    PGridViewModule,
    SalespersonManagementDetailModule,
    RouterModule.forChild([{ path: '', component: SalespersonManagementListComponent }])
  ]
})
export class SalespersonManagementListModule { }
