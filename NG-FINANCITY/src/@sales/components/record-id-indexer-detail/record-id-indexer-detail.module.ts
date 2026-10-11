import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RecordIdIndexerDetailComponent } from './record-id-indexer-detail.component';
import { PModalFormModule } from 'src/@common/components/p-modal-form/p-modal-form.module';
import { IDReferenceIndexService } from 'src/@sales/services/Id-reference-Index-service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { InputsModule,TextBoxModule } from '@progress/kendo-angular-inputs';
import { ButtonModule } from '@progress/kendo-angular-buttons';

@NgModule({
  declarations: [RecordIdIndexerDetailComponent],
  entryComponents: [RecordIdIndexerDetailComponent],
  imports: [
    
    CommonModule,
    ButtonModule,
    DropDownsModule,
    PModalFormModule,
    ReactiveFormsModule,
    FormsModule,
    InputsModule,TextBoxModule,
  ],
  exports: [RecordIdIndexerDetailComponent]
})
export class RecordIdIndexerDetailModule { }
