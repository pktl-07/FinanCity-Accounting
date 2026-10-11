import { Input, Directive } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Directive({
    selector: 'fc-filter-item',
})
export class FCFilterItemDirective {

    @Input() field: string;
    @Input() label: string;
    @Input() type: 'text' | 'dropdown' | 'date' | 'hidden' | 'currency' | 'number';
    @Input() default: boolean = false;
    @Input() logic: 'or' | 'and' = 'or';
    @Input() value_1: any;
    @Input() value_2: any;
    @Input() items: Array<any> | BehaviorSubject<any>;
    @Input() value: string;
    @Input() text: string;
}
