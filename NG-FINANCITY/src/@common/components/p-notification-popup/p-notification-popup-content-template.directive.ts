import { Directive, TemplateRef, Optional } from '@angular/core';

@Directive({
    selector: '[fcNotificationContentTemplate]'
})
export class NotificationContentTemplateDirective {
    constructor(@Optional() public templateRef: TemplateRef<any>) { }
}
