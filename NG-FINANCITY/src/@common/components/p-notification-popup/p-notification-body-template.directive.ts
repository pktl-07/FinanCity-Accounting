import { Directive, TemplateRef, Optional } from '@angular/core';

@Directive({
    selector: '[fcNotificationBodyTemplate]'
})
export class NotificationBodyTemplateDirective {
    constructor(@Optional() public templateRef: TemplateRef<any>) { }
}
