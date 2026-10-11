import { Directive, TemplateRef, Optional } from '@angular/core';

@Directive({
    selector: '[fcModalContentTemplate]'
})
export class ModalContentTemplateDirective {
    constructor(@Optional() public templateRef: TemplateRef<any>) { }
}
