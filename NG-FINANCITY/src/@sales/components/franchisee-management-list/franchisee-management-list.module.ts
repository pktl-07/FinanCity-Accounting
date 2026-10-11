import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { PGridViewModule } from 'src/@common/components/p-grid-view/p-grid-view.module';
import { FranchiseeManagementListComponent } from './franchisee-management-list.component';
import { FranchiseeManagementDetailModule } from '../franchisee-management-detail/franchisee-management-detailmodule';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [FranchiseeManagementListComponent],
  imports: [
    CommonModule,
    PToolbarModule,
    PWindowModule,
    PGridViewModule,
    FranchiseeManagementDetailModule,
    RouterModule.forChild([{ path: '', component: FranchiseeManagementListComponent }])
  ]
})
export class FranchiseeManagementListModule { }
