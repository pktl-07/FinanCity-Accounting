import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LeadManagementListComponent } from './lead-management-list.component';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { PGridViewModule } from 'src/@common/components/p-grid-view/p-grid-view.module';
import { TextBoxModule } from '@progress/kendo-angular-inputs';
import { LeadManagementDetailModule } from '../lead-management-detail/lead-management-detail.module';
import { LeadManagementSalespersonListComponent } from './lead-management-salesperson-list/lead-management-salesperson-list.component';
import { RouterModule } from '@angular/router';


const components = [LeadManagementListComponent,
  LeadManagementSalespersonListComponent]

@NgModule({
  entryComponents: [...components],
  declarations: [...components],
  imports: [
    CommonModule,
    PToolbarModule,
    PWindowModule,
    LeadManagementDetailModule,
    PGridViewModule,
    TextBoxModule,
    RouterModule.forChild([{ path: '', component: LeadManagementListComponent }])
  ]
})
export class LeadManagementListModule { }
