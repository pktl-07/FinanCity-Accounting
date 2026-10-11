import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserManagementListComponent } from './user-management-list.component';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { PGridViewModule } from 'src/@common/components/p-grid-view/p-grid-view.module';
import { UserManagementDetailModule } from '../user-management-detail/user-management-detail.module';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [UserManagementListComponent],
  imports: [
    CommonModule,
    PToolbarModule,
    PWindowModule,
    PGridViewModule,
    UserManagementDetailModule,
    RouterModule.forChild([{ path: '', component: UserManagementListComponent }])
  ]
})
export class UserListManagementModule { }
