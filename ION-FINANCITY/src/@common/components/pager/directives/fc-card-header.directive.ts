import { Input, Directive, ContentChild } from '@angular/core';
import { FCCardHeaderTemplate } from './fc-card-header-template';

@Directive({
    selector: 'fc-card-header'
})
export class FCCardHeaderDirective {

    @Input() field: string;
    @Input() icon: any;
    @Input() image: any;

    @ContentChild(FCCardHeaderTemplate, { static: true }) headerTemplate: FCCardHeaderTemplate;
}
