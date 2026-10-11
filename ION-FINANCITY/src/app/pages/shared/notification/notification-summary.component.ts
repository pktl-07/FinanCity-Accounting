import { Component, OnDestroy, OnInit } from '@angular/core';
import { SubSink } from 'src/@common/utils';
import { PopoverController } from '@ionic/angular';
import { mergeMap, map } from 'rxjs/operators';
import { BehaviorSubject } from 'rxjs';
import { NotificationSummaryModalComponent } from './notification-summary-modal.component';
import { environment } from 'src/environments/environment';

@Component({
    selector: 'fc-notification-summary',
    template: `
        <ion-button fill="clear" class="notification-icon-button" (click)="handleClick($event)">
            <ion-badge class="badge-icon" color="danger">{{ counter$ | async }}</ion-badge>
            <ion-icon slot="icon-only" name="notifications"></ion-icon>
        </ion-button>
    `,
    styles: [`
            .notification-icon-button {
            --overflow: visible;
        }

        .badge-icon {
            position: absolute;
            top: 0px;
            // right: -5px;
            font-size: 0.8em;
            --padding-start: 5px;
            --padding-end: 5px;
          }
     `]
})
export class NotificationSummaryComponent implements OnInit, OnDestroy {

    private _subsink = new SubSink();

    constructor(private _popoverController: PopoverController) {
    }

    ngOnDestroy(): void {

        this._subsink.unsubscribe();
    }

    counter$ = new BehaviorSubject<number>(0);

    ngOnInit(): void {

        this._subsink.sink = environment.general.chattingCounter
            .pipe(mergeMap((chatting: any) => {
                return environment.general.mortageSalesCounter
                    .pipe(map(mortgage => {
                        return chatting + mortgage;
                    }))
            }),
                mergeMap((response) => {
                    return environment.general.leadFollowUpCounter
                        .pipe(map(followup => {
                            return response + followup;
                        }))
                })
            ).subscribe((count) => {
                this.counter$.next(count);
            })
    }

    async handleClick(eve: any) {

        const p = await this._popoverController.create({
            component: NotificationSummaryModalComponent,
            event: eve,
            translucent: true,
        });

        return p.present();
    }
}
