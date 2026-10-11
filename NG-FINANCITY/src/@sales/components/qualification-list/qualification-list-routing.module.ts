import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { QualificationListComponent } from './qualification-list.component';


@NgModule({
  imports: [RouterModule.forChild([{ path: '', component: QualificationListComponent }])],
  exports: [RouterModule]
})
export class QualificationListRoutingModule { }
