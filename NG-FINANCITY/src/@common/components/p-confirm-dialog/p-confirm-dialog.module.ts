import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DialogModule } from '@progress/kendo-angular-dialog';
import { PConfirmDialogComponent } from './p-confirm-dialog.component';
import { PErrorComponent, PWarningCreateComponent, PValidationFailComponent, PFileDeletWarnningComponent } from './p-error-component';

@NgModule({
  entryComponents: [
    PErrorComponent,
    PWarningCreateComponent,
    PValidationFailComponent,
    PFileDeletWarnningComponent
  ],
  declarations: [
    PConfirmDialogComponent,
    PErrorComponent,
    PWarningCreateComponent,
    PValidationFailComponent,
    PFileDeletWarnningComponent
  ],
  imports: [
    CommonModule,
    DialogModule
  ],
  exports: [
    PConfirmDialogComponent,
    PErrorComponent,
    PWarningCreateComponent,
    PValidationFailComponent,
    PFileDeletWarnningComponent
  ]
})
export class PConfirmDialogModule { }
