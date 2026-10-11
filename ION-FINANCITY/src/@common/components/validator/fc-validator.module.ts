import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FcValidationContainerComponent } from './fc-validation-container.component';
import { FCValidationRequiredDirective } from './directives/fc-validation-required.directive';
import { FCValidationEmailDirective } from './directives/fc-validation-email.directive';
import { FCValidationMinLengthDirective } from './directives/fc-validation-minlength.directive';
import { FCValidationMinDirective } from './directives/fc-validation-min';
import { FCValidationMaxDirective } from './directives/fc-validation-max';

@NgModule({
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
    declarations: [
        FcValidationContainerComponent,
        FCValidationEmailDirective,
        FCValidationRequiredDirective,
        FCValidationMinLengthDirective,
        FCValidationMinDirective,
        FCValidationMaxDirective
    ],
    imports: [
        CommonModule,
    ],
    exports: [
        FcValidationContainerComponent,
        FCValidationEmailDirective,
        FCValidationRequiredDirective,
        FCValidationMinLengthDirective,
        FCValidationMinDirective,
        FCValidationMaxDirective
    ],
})
export class FCValidatorModule { }
