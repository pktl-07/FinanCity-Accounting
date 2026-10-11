import { forwardRef, Directive } from '@angular/core';
import { FCValidatorBase } from '../fc-validator-base';

@Directive({
    selector: 'fc-validation-max',
    providers: [
        {
            provide: FCValidatorBase,
            useExisting: forwardRef(() => FCValidationMaxDirective)
        }
    ]
})
export class FCValidationMaxDirective extends FCValidatorBase {

    constructor() {
        super('', 'max');
    }
}
