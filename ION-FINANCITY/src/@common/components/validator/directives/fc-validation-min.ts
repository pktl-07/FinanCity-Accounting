import { forwardRef, Directive } from '@angular/core';
import { FCValidatorBase } from '../fc-validator-base';

@Directive({
    selector: 'fc-validation-min',
    providers: [
        {
            provide: FCValidatorBase,
            useExisting: forwardRef(() => FCValidationMinDirective)
        }
    ]
})
export class FCValidationMinDirective extends FCValidatorBase {

    constructor() {
        super('', 'min');
    }
}
