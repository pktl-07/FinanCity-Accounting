import { forwardRef, Directive } from '@angular/core';
import { FCValidatorBase } from '../fc-validator-base';

@Directive({
    selector: 'fc-validation-email',
    providers: [
        {
            provide: FCValidatorBase,
            useExisting: forwardRef(() => FCValidationEmailDirective)
        }
    ]
})
export class FCValidationEmailDirective extends FCValidatorBase {

    constructor() {
        super('must be valid email format.', 'email');
    }
}
