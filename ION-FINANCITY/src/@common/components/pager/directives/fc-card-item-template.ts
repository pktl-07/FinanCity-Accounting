import { Directive, TemplateRef, Optional } from '@angular/core';

@Directive({
    selector: '[fcCardItemTemplate]'
})
export class FCCardItemTemplate {

    constructor(@Optional() public templateRef: TemplateRef<any>) { }
}
