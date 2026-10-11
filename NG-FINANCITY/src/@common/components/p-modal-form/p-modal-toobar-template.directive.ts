import { Directive, TemplateRef, Optional } from '@angular/core';

@Directive({
    selector: '[fcToolbarTemplate]'
})
export class ModalToolbarTemplateDirective {
    constructor(@Optional() public templateRef: TemplateRef<any>) { }
}
