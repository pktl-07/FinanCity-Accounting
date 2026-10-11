import { forwardRef, Directive } from '@angular/core';
import { FCValidatorBase } from '../fc-validator-base';

@Directive({
    selector: 'fc-validation-minlength',
    providers: [
        {
            provide: FCValidatorBase,
            useExisting: forwardRef(() => FCValidationMinLengthDirective)
        }
    ]
})
export class FCValidationMinLengthDirective extends FCValidatorBase {

    constructor() {
        super('is invalid length.', 'minlength');
    }
}
