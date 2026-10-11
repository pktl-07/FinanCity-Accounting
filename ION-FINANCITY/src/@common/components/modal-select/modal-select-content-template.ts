import { Directive, TemplateRef, Optional } from '@angular/core';

@Directive({
    selector: '[fcModalSelectContentTemplate]'
})
export class FCModalSelectContentDirective {

    constructor(@Optional() public templateRef: TemplateRef<any>) { }
}
