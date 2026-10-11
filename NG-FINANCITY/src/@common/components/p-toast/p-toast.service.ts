import { Injectable } from '@angular/core';
import { PToastChildComponent } from './p-toast-child.component';
import { ToastConfig } from './p-toast.config';
import { NotificationService } from '@progress/kendo-angular-notification';

@Injectable({ providedIn: 'root' })
export class ToastService {

    constructor(private _notificationService: NotificationService) { }

    open(config: ToastConfig<{ title: string, message: string }>) {

        let notificationRef = this._notificationService.show({
            content: PToastChildComponent,
            position: { horizontal: 'right', vertical: 'bottom' },
            animation: { type: 'fade', duration: 500 },
            type: { style: 'warning', icon: false },
            hideAfter: 3000,
            height: 100,
            width: 300,
        })

        notificationRef.content.instance.message = config.data.message;
    }
}