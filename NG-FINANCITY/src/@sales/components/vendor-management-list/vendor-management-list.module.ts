import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { PGridViewModule } from 'src/@common/components/p-grid-view/p-grid-view.module';
import { VendorManagementListComponent } from './vendor-management-list.component';
import { VendorManagementDetailModule } from '../vendor-management-detail/vendor-management-detail.module';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [VendorManagementListComponent],
  imports: [
    CommonModule,
    PToolbarModule,
    PWindowModule,
    PGridViewModule,
    VendorManagementDetailModule,
    RouterModule.forChild([{ path: '', component: VendorManagementListComponent }])
  ]
})
export class VendorManagementListModule { }
