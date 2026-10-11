import { Component, ViewChild, ViewContainerRef } from '@angular/core';
import { DialogService } from '@progress/kendo-angular-dialog';
import { Observable, Subject } from 'rxjs';
import { RecapDailogGridComponent } from './p-recap-dialog-grid.component';

@Component({
    selector: 'recap-dialog',
    template: `
        <div kendoDialogContainer #recapDialogContainer></div>`,
    providers: [DialogService]
})
export class RecapDailogComponent {

    @ViewChild("recapDialogContainer", { read: ViewContainerRef, static: true }) containerRef: ViewContainerRef;

    loading = new Subject<boolean>();

    constructor(private dialogService: DialogService) { }

    public open(stream: Observable<any>) {

        this.loading.next(true);
        return new Promise<any>(resolve => {

            let dialog = this.dialogService.open({
                appendTo: this.containerRef,
                title: 'Recap',
                content: RecapDailogGridComponent,
                actions: [{ text: 'Close' }],
                width: 1000,
                height: 500
            });
            const instance = (dialog.content.instance as RecapDailogGridComponent)
            instance.fetch = stream;
            resolve();
        });
    }
}

