import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { PGridViewModule } from 'src/@common/components/p-grid-view/p-grid-view.module';
import { RecordIdIndexerListComponent } from './record-id-indexer-list.component';
import { RecordIdIndexerDetailModule } from '../record-id-indexer-detail/record-id-indexer-detail.module';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { GridModule } from '@progress/kendo-angular-grid';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [RecordIdIndexerListComponent],
  imports: [
    CommonModule,
    PToolbarModule,
    PWindowModule,
    PGridViewModule,
    RecordIdIndexerDetailModule,
    GridModule,
    RouterModule.forChild([{ path: '', component: RecordIdIndexerListComponent }])
  ]
})
export class RecordIdIndexerListModule { }
