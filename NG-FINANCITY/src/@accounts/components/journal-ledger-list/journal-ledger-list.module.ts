import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { JournalLedgerListComponent } from './journal-ledger-list.component';
import { PGridViewModule } from 'src/@common/components/p-grid-view/p-grid-view.module';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { JournalLedgerGridComponent } from './journal-ledger-list-grid.component';
import { GridModule } from '@progress/kendo-angular-grid';
import { JournalLedgerGridDetailComponent } from './journal-ledger-list-grid.-detail.component';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [JournalLedgerListComponent,
    JournalLedgerGridComponent,
    JournalLedgerGridDetailComponent],
  imports: [
    CommonModule,
    PToolbarModule,
    PGridViewModule,
    LayoutModule,
    PGridViewModule,
    GridModule,
    RouterModule.forChild([{ path: '', component: JournalLedgerListComponent }])
  ],
  exports: [JournalLedgerGridComponent, JournalLedgerGridDetailComponent]
})
export class JournalLedgerListModule { }
