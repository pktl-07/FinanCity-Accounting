import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PGridViewModule } from 'src/@common/components/p-grid-view/p-grid-view.module';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { TextBoxModule } from '@progress/kendo-angular-inputs';
import { JobListComponent } from './job-list.component';
import { JobDetailModule } from '../job-detail/job-detail.module';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [JobListComponent],
  imports: [
    CommonModule,
    PToolbarModule,
    PWindowModule,
    PGridViewModule,
    TextBoxModule,
    JobDetailModule,
    RouterModule.forChild([{ path: '', component: JobListComponent }])
  ]
})
export class JobListModule { }
