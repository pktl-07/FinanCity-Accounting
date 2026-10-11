import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GridModule } from '@progress/kendo-angular-grid';
import { PGridIncellEditingComponent } from './p-grid-incell-editing.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { PopupModule } from '@progress/kendo-angular-popup';
import { GridValidationPopupAnchor } from './p-grid-validation-popup-directive';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { DatePickerModule } from '@progress/kendo-angular-dateinputs';
import { TooltipModule } from '@progress/kendo-angular-tooltip';

@NgModule({
  declarations: [PGridIncellEditingComponent, GridValidationPopupAnchor],
  imports: [
    CommonModule,
    GridModule,
    FormsModule,
    ButtonModule,
    ReactiveFormsModule,
    DropDownsModule,
    InputsModule,
    PopupModule,
    DatePickerModule,
    TooltipModule,
  ],
  exports: [PGridIncellEditingComponent]
})
export class PGridIncellEditingModule { }
