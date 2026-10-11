import { Directive, TemplateRef, Optional } from '@angular/core';

@Directive({
    selector: '[fcCustomDataDirective]'
})
export class FCCustomDataDirective {
    constructor(@Optional() public templateRef: TemplateRef<any>) { }
}
