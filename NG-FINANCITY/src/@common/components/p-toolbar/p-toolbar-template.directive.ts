import { Directive, TemplateRef, Optional } from '@angular/core';

@Directive({
    selector: '[fcToolbarTemplate]'
})
export class PToolbarTemplateDirective {
    constructor(@Optional() public templateRef: TemplateRef<any>) { }
}
