import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuditTrailListComponent } from './audit-trail-list.component';
import { PGridViewModule } from 'src/@common/components/p-grid-view/p-grid-view.module';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { GridModule } from '@progress/kendo-angular-grid';
import { AuditTrailGridDetailComponent } from './audit-trail-list-grid-detail.component';
import { AuditTrailListGridComponent } from './audit-trail-list-grid.component';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [AuditTrailListComponent,
    AuditTrailListGridComponent,
    AuditTrailGridDetailComponent],
  imports: [
    CommonModule,
    PToolbarModule,
    PGridViewModule,
    LayoutModule,
    PGridViewModule,
    GridModule,
    RouterModule.forChild([{ path: '', component: AuditTrailListComponent }])
  ]
})
export class AuditTrailListModule { }
