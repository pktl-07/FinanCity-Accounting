import { Directive, TemplateRef, Optional } from '@angular/core';

@Directive({
    selector: '[fcCardHeaderTemplate]'
})
export class FCCardHeaderTemplate {

    constructor(@Optional() public templateRef: TemplateRef<any>) { }
}
