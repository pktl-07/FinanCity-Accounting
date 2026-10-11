import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CashbookSetupListComponent } from './cashbook-setup-list.component';
import { PGridViewModule } from 'src/@common/components/p-grid-view/p-grid-view.module';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { TextBoxModule } from '@progress/kendo-angular-inputs';
import { CashbookSetupDetailModule } from '../cashbook-setup-detail/cashbook-setup-detail.module';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [CashbookSetupListComponent],
  imports: [
    CommonModule,
    PToolbarModule,
    PWindowModule,
    PGridViewModule,
    CashbookSetupDetailModule,
    TextBoxModule,
    RouterModule.forChild([{ path: '', component: CashbookSetupListComponent }])
  ]
})
export class CashoobkSetupListModule { }
