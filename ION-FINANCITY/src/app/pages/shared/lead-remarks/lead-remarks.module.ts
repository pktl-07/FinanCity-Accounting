import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { LeadRemarkPage } from './lead-remarks.page';
import { FCValidatorModule } from 'src/@common/components/validator/fc-validator.module';
import { FCDateFormatPipeModule, FCFileSizePipeModule, FCImagePipeModule, FCNamePipeModule } from 'src/@common/pipes/fc.pipe.module';
import { FaIconLibrary, FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { FCFileOpenerModule } from 'src/@common/components/file-opener/fc-file-opener.module';

@NgModule({
  declarations: [LeadRemarkPage],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    IonicModule,
    FCValidatorModule,
    FontAwesomeModule,
    FCFileOpenerModule,
    FCNamePipeModule,
    FCDateFormatPipeModule,
    FCImagePipeModule,
    FCFileSizePipeModule
  ],
  exports: [LeadRemarkPage],
  providers: [
    FaIconLibrary
  ]
})
export class LeadRemarkPageModule { }
