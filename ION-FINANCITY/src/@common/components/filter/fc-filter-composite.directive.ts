import { ContentChildren, QueryList, Input, Directive } from '@angular/core';
import { FCFilterItemDirective } from './fc-filter-item.directive';

@Directive({
    selector: 'fc-filter-composite',
})
export class FCFilterCompositeDirective {

    @Input() title: string;
    @ContentChildren(FCFilterItemDirective) filterItemsComponent: QueryList<FCFilterItemDirective>;
}
