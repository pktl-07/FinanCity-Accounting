import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CommissoinSchemeListComponent } from './commissoin-scheme-list.component';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { PGridViewModule } from 'src/@common/components/p-grid-view/p-grid-view.module';
import { CommissionSchemeDetailModule } from '../commission-scheme-detail/commission-scheme-detail.module';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [CommissoinSchemeListComponent],
  imports: [
    CommonModule,
    PToolbarModule,
    PWindowModule,
    PGridViewModule,
    CommissionSchemeDetailModule,
    RouterModule.forChild([{ path: '', component: CommissoinSchemeListComponent }])
  ]
})
export class CommissoinSchemeListModule { }
