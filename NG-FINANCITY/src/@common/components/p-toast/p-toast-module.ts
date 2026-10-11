import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PToastChildComponent } from './p-toast-child.component';
import { NotificationModule } from '@progress/kendo-angular-notification';

@NgModule({
  entryComponents: [PToastChildComponent],
  declarations: [PToastChildComponent],
  imports: [
    CommonModule,
    NotificationModule
  ],
  providers: []
})
export class PToastModule {
}
