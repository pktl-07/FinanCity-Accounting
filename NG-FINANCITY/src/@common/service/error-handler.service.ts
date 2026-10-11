import { Injectable, NgZone, ErrorHandler } from '@angular/core';
import { DialogService } from '@progress/kendo-angular-dialog';

@Injectable({ providedIn: 'root' })
export class ErrorsHandler implements ErrorHandler {
    constructor(
        private dialogService: DialogService,
        private ngZone: NgZone
    ) { }

    handleError() {
        this.ngZone.run(() => {
            this.dialogService.open({
                title: 'Error Dialog',
                content: 'An error occured',
                actions: [
                    { text: 'No' },
                    { text: 'Yes', primary: true }
                ],
            }).result.subscribe(result => console.log(result));
        });
    }
}