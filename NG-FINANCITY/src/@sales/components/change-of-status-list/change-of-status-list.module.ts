import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChangeOfStatusListComponent } from './change-of-status-list.component';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { PGridViewModule } from 'src/@common/components/p-grid-view/p-grid-view.module';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [ChangeOfStatusListComponent],
  imports: [
    CommonModule,
    PToolbarModule,
    PWindowModule,
    PGridViewModule,
    RouterModule.forChild([{ path: '', component: ChangeOfStatusListComponent }])
  ]
})
export class ChangeOfStatusListModule { }
