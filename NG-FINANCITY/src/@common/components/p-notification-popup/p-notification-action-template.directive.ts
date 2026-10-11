import { Directive, TemplateRef, Optional } from '@angular/core';

@Directive({
    selector: '[fcNotificationActionTemplate]'
})
export class NotificationActionTemplateDirective {
    constructor(@Optional() public templateRef: TemplateRef<any>) { }
}
