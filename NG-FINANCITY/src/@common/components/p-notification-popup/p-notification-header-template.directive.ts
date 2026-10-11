import { Directive, TemplateRef, Optional } from '@angular/core';

@Directive({
    selector: '[fcNotificationHeaderTemplate]'
})
export class NotificationHeaderTemplateDirective {
    constructor(@Optional() public templateRef: TemplateRef<any>) { }
}
