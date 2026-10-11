import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { Observable } from 'rxjs';
import { SubSink } from 'src/@common/utils';

/**
 * @example
 *  <fc-filter-footer visible="true" [info]="infoSubject"></fc-filter-footer>
 */
@Component({
    selector: 'fc-filter-footer',
    template: `
        <ion-text class="text-muted" slot="end">
            {{ data }}
        </ion-text>
  `,
})
export class FCFilterFooterComponent implements OnInit, OnDestroy {

    private _subsink = new SubSink();

    @Input() visible: boolean = true;
    @Input() info = new Observable();
    data: string;

    ngOnInit(): void {
        
        this._subsink.sink = this.info
            .subscribe((d: string) => {
                this.data = d;
            })
    }

    ngOnDestroy(): void {
        this._subsink.unsubscribe();
    }
}
