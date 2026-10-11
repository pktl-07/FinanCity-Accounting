import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ExternalSalespersonListComponent } from './external-salesperson-list.component';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { PGridViewModule } from 'src/@common/components/p-grid-view/p-grid-view.module';
import { ExternalSalespersonDetailModule } from '../external-salesperson-detail/external-salesperson-detail.module';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [ExternalSalespersonListComponent],
  imports: [
    CommonModule,
    PToolbarModule,
    PWindowModule,
    ExternalSalespersonDetailModule,
    PGridViewModule,
    RouterModule.forChild([{ path: '', component: ExternalSalespersonListComponent }])
  ]
})
export class ExternalSalespersonListModule { }
