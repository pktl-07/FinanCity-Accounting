import { Component, Input, ViewChild, ElementRef, HostListener, ViewEncapsulation, ContentChild, Output, EventEmitter } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { NotificationContentTemplateDirective } from './p-notification-popup-content-template.directive';
import { Collision, Offset, Align } from '@progress/kendo-angular-popup';

@Component({
  selector: 'p-notification-popup',
  template: `
    <button #anchor  (click)="toggle()" class="btn btn-secondary btn-icon" [style.margin-right.px]='mr'>
      <span class="badge badge-pill badge-danger badge-notification">{{ badgetCount | async}}</span>
      <i class="{{ icon }}" style="font-size: 1.5rem;"></i>
    </button>
    <kendo-popup #popup [anchor]="anchor" popupClass="popup-content" *ngIf="show" 
     [offset]="offset" [collision]="collision" [anchorAlign]="anchorAligh">
      <div class='inner-content'>
        <ng-template [ngIf]="!contentTemplateDirective?.templateRef" 
        [ngTemplateOutlet]="contentTemplateDirective?.templateRef"></ng-template>
      </div>
    </kendo-popup>
   
  `,
  providers: [],
  styles: [`

    .badge-notification{
      position:relative;
      left: 35px;
    }

    .popup-content {
      color: #787878;
      max-height:400px !important;
      border: 1px solid rgba(0,0,0,.05);
    }

    .inner-content{
      overflow:auto;
    }
  `],
  encapsulation: ViewEncapsulation.None
})
export class PNotificationPopupComponent {

  @ViewChild('anchor', { read: ElementRef, static: true }) public anchor: ElementRef;
  @ViewChild('popup', { read: ElementRef, static: true }) public popup: ElementRef;
  @ContentChild(NotificationContentTemplateDirective, { static: true }) contentTemplateDirective: NotificationContentTemplateDirective;

  @Output() open = new EventEmitter();
  @Output() close = new EventEmitter();

  collision: Collision = { horizontal: 'flip', vertical: 'fit' };
  offset: Offset = { left: 100, top: 100 };
  anchorAligh: Align = { horizontal: 'left', vertical: 'bottom' };

  @Input() icon: string;
  @Input() mr: number = 0;
  @Input() badgetCount: BehaviorSubject<number>;
  public show = false;


  @HostListener('document:click', ['$event'])
  public documentClick(event: any): void {

    if (!this.contains(event.target)) {

      this.toggle(false);
    }
  }

  constructor() { }

  public toggle(show?: boolean): void {
    this.show = show !== undefined ? show : !this.show;

    if (this.show)
      this.open.emit(event);
    else
      this.close.emit(event);
  }

  private contains(target: any): boolean {

    return this.anchor.nativeElement.contains(target) ||
      (this.popup ? this.popup.nativeElement.contains(target) : false);
  }
}

