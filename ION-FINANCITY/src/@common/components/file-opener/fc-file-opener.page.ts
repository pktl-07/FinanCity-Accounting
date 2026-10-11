import { Component, OnInit, Input } from '@angular/core';
import { ToastController } from '@ionic/angular';
import { FCFileOpenerService, } from './fc-file-openr.service';

@Component({
  selector: 'fc-file-opener',
  template: `
    <ng-container *ngIf="showDownloadButton; else defaultTemplate">
      <ion-button color="primary" expand="full" (click)="handleDownload()">
        <ion-icon name="download"></ion-icon>
        Download
        &nbsp;&nbsp;
        <ion-spinner *ngIf="loading" name="dots"></ion-spinner>
      </ion-button>
    </ng-container>
    <ng-template #defaultTemplate>
      <ion-button color="primary" expand="full" disabled="true" (click)="handleDownload()">
        <ion-icon name="download"></ion-icon>
        The download is unavailable.
      </ion-button>
    </ng-template>
  `,
  providers: [
    FCFileOpenerService
  ]
})
export class FCFileOpenerPage implements OnInit {

  @Input() uploadId: string;
  @Input() fileType: number;
  loading: boolean = false;

  showDownloadButton: boolean = false;

  constructor(private _fcFileOpenerService: FCFileOpenerService,
    private _toastController: ToastController) {
  }

  ngOnInit() {

    this._fcFileOpenerService.isDownloadAvailable({ fileType: this.fileType, uploadId: this.uploadId })
      .then((value) => this.showDownloadButton = value)
      .catch((err) => this.presentToast(err));
  }

  handleDownload() {

    this.loading = true;
    this._fcFileOpenerService.openDownload({ fileType: this.fileType, uploadId: this.uploadId })
      .then(() => this.loading = false)
      .catch((err: string) => {
        this.presentToast(err);
        this.loading = false;
      })
  }

  private async presentToast(message: string) {

    const toast = await this._toastController.create({
      message: message,
      position: "bottom",
      duration: 5000,
    });
    await toast.present();
  }
}
