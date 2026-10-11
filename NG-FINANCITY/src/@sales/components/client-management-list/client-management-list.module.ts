import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ClientManagementListComponent } from './client-management-list.component';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { ClientManagementDetailModule } from '../client-management-detail/client-management-detail.module';
import { PGridViewModule } from 'src/@common/components/p-grid-view/p-grid-view.module';
import { TextBoxModule } from '@progress/kendo-angular-inputs';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [ClientManagementListComponent],
  imports: [
    CommonModule,
    PToolbarModule,
    PWindowModule,
    ClientManagementDetailModule,
    PGridViewModule,
    TextBoxModule,
    RouterModule.forChild([{ path: '', component: ClientManagementListComponent }])
  ]
})
export class ClientManagementListModule { }
