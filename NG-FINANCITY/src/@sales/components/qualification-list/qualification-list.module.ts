import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { QualificationListRoutingModule } from './qualification-list-routing.module';
import { QualificationListComponent } from './qualification-list.component';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { PGridViewModule } from 'src/@common/components/p-grid-view/p-grid-view.module';
import { QualificationDetailModule } from '../qualification-detail/qualification-detail.module';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [QualificationListComponent],
  imports: [
    CommonModule,
    PToolbarModule,
    PWindowModule,
    PGridViewModule,
    QualificationListRoutingModule,
    QualificationDetailModule,
    RouterModule.forChild([{ path: '', component: QualificationListComponent }])
  ]
})
export class QualificationListModule { }
