import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { PGridViewModule } from 'src/@common/components/p-grid-view/p-grid-view.module';
import { DepartmentListComponent } from './department-list.component';
import { DepartmentDetailModule } from '../department-detail/department-detail.module';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [DepartmentListComponent],
  imports: [
    CommonModule,
    PToolbarModule,
    PWindowModule,
    PGridViewModule,
    DepartmentDetailModule,
    RouterModule.forRoot([{ path: '', component: DepartmentListComponent }])
  ]
})
export class DepartmentListModule { }
