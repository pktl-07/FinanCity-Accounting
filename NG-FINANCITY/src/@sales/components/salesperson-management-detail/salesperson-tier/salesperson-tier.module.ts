import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { InputsModule, TextBoxModule } from '@progress/kendo-angular-inputs';
import { SalespersonTierComponent } from './salesperson-tier.component';
import { TreeViewModule } from '@progress/kendo-angular-treeview';
import { PInputIconDirectiveModule } from 'src/@common/components/p-input-icon/p-input-icon.module';

@NgModule({
  declarations: [SalespersonTierComponent],
  imports: [
    CommonModule,
    ButtonModule,
    DropDownsModule,
    ReactiveFormsModule,
    FormsModule,
    InputsModule,
    TextBoxModule,
    TreeViewModule,
    PInputIconDirectiveModule
  ],
  exports: [SalespersonTierComponent]
})
export class SalespersonTierModule { }
