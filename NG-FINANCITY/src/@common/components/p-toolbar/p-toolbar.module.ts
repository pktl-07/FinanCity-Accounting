import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PToolbarComponent } from './p-toolbar.component';
import { PConfirmDialogModule } from '../p-confirm-dialog/p-confirm-dialog.module';
import { PToolbarTemplateDirective } from './p-toolbar-template.directive';

@NgModule({

  declarations: [PToolbarComponent,
    PToolbarTemplateDirective],
  imports: [
    CommonModule,
    PConfirmDialogModule,
  ],
  exports: [PToolbarComponent,
    PToolbarTemplateDirective],
})
export class PToolbarModule { }
