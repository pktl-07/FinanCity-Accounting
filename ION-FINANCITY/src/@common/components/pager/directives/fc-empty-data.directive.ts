import { Directive, TemplateRef, Optional } from '@angular/core';

@Directive({
    selector: '[fcEmptyDataTemplate]'
})
export class FCEmptyDataTemplate {
    constructor(@Optional() public templateRef: TemplateRef<any>) { }
}
