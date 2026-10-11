import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PConfirmDialogModule } from '../p-confirm-dialog/p-confirm-dialog.module';
import { PModalFormComponent } from './p-modal-form.component';
import { ModalContentTemplateDirective } from './p-modal-content-template.directive';
import { ModalToolbarTemplateDirective } from './p-modal-toobar-template.directive';
import { LoadingMoule } from '../p-loading/p-loading.module';
import { SkeletonLoadingModule } from '../p-skeleton-loading/p-skeleton-loading.module';

@NgModule({
  entryComponents: [PModalFormComponent],
  declarations: [PModalFormComponent,
    ModalContentTemplateDirective,
    ModalToolbarTemplateDirective],
  imports: [
    CommonModule,
    PConfirmDialogModule,
    LoadingMoule,
    SkeletonLoadingModule
  ],
  exports: [PModalFormComponent,
    ModalContentTemplateDirective,
    ModalToolbarTemplateDirective]
})
export class PModalFormModule { }
