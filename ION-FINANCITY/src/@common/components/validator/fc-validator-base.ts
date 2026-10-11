import { Input, Directive } from '@angular/core';
import { AbstractControl } from '@angular/forms';

@Directive()
export class FCValidatorBase {

    @Input() public error: string;
    @Input() public message: string;
    @Input() public label: string;
    @Input() public control: AbstractControl;

    constructor(private _message: string,
        private _error: string) {

        this.message = _message;
        this.error = _error;
    }
}