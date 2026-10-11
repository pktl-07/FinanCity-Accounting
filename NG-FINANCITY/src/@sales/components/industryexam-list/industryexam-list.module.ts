import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IndustryExamListComponent } from './industryexam-list.component';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { PGridViewModule } from 'src/@common/components/p-grid-view/p-grid-view.module';
import { IndustryExamDetailModule } from '../industryexam-detail/industryexam-detail.module';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [IndustryExamListComponent],
  imports: [
    CommonModule,
    PToolbarModule,
    PWindowModule,
    PGridViewModule,
    IndustryExamDetailModule,
    RouterModule.forChild([{ path: '', component: IndustryExamListComponent }])
  ]
})
export class IndustryExamListModule { }
