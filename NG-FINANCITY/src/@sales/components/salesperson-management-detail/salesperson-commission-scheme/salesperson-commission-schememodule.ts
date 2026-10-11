import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { InputsModule, TextBoxModule } from '@progress/kendo-angular-inputs';
import { PCustomKendoDatePickerModule } from 'src/@common/components/p-custom-kendo-date-picker/p-custom-kendo-date-picker.module';
import { PWindowModule } from 'src/@common/components/p-window/p-window.module';
import { SalespersonCommissionSchemeComponent } from "./salesperson-commission-scheme.component";
import { GridModule } from '@progress/kendo-angular-grid';
import { PGridViewModule } from 'src/@common/components/p-grid-view/p-grid-view.module';
import { TooltipModule } from '@progress/kendo-angular-tooltip';
import { CommissionSchemeDetailModule } from '../../commission-scheme-detail/commission-scheme-detail.module';
import { PInputIconDirectiveModule } from 'src/@common/components/p-input-icon/p-input-icon.module';

@NgModule({
  declarations: [SalespersonCommissionSchemeComponent],
  imports: [
    CommonModule,
    ButtonModule,
    DropDownsModule,
    ReactiveFormsModule,
    FormsModule,
    InputsModule, TextBoxModule,
    PCustomKendoDatePickerModule,
    PWindowModule,
    GridModule,
    PGridViewModule,
    TooltipModule,
    CommissionSchemeDetailModule,
    PInputIconDirectiveModule
  ],
  exports: [SalespersonCommissionSchemeComponent],
})
export class SalespersonCommissionSchemeModule { }
