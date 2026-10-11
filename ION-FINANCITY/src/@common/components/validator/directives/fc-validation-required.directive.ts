import { forwardRef, Directive } from '@angular/core';
import { FCValidatorBase } from '../fc-validator-base';

@Directive({
    selector: 'fc-validation-required',
    providers: [
        {
            provide: FCValidatorBase,
            useExisting: forwardRef(() => FCValidationRequiredDirective)
        }
    ]
})
export class FCValidationRequiredDirective extends FCValidatorBase {
    
    constructor() {
        super('should not be empty.', 'required');
    }
}
