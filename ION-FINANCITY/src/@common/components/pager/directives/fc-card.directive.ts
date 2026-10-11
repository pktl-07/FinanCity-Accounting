import { Output, EventEmitter, Directive } from '@angular/core';

@Directive({
  selector: 'fc-card',
})
export class FCCardDirective {

  @Output('click') click: EventEmitter<any> = new EventEmitter();
}
