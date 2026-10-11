import { Input, Directive, ContentChild } from '@angular/core';
import { FCCardItemTemplate } from './fc-card-item-template';

@Directive({
    selector: 'fc-card-item',
})
export class FCCardItemDirective {

    @Input() field: string;
    @ContentChild(FCCardItemTemplate, { static: true }) itemTemplate: FCCardItemTemplate;
}
