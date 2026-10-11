import { Component, OnInit, OnDestroy, Input, Output, EventEmitter, } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { ActionSheetController, AlertController, ToastController } from '@ionic/angular';
import { FileService } from '../native-devices/fc-file-service';
import { FormBuilder, FormGroup } from '@angular/forms';
import { FCCameraService } from '../native-devices/fc-camera.service';
import { PictureSourceType } from '@ionic-native/camera/ngx';
import { SubSink } from 'src/@common/utils';

@Component({
  selector: 'fc-image-loader',
  template: `
    <div>
        <ion-thumbnail class="p-image-loader-main" (click)="showActionSheet()">
          <ion-spinner name="lines" color="primary" *ngIf="loading"></ion-spinner>
          <ion-img [src]="form.get('file').value || 'assets/icon/empty-picture.png'" style="width:100px;height:100px"></ion-img>
        </ion-thumbnail>
    </div>
  `,
  styles: [
    `
    .p-image-loader-main {
      position: relative;
      width: 200px;
      height: 250px;
      margin: auto;
      padding: auto;
      display: flex;
      align-items: center;
      justify-content: center;
    }  

    ion-spinner {
      position: absolute;
      z-index: 999;
      overflow: show;
      margin: auto;
      padding: auto;
    }
    `
  ],
  providers: [
    FileService,
    FCCameraService
  ]
})
export class FCImageLoaderPage implements OnInit, OnDestroy {

  @Input() public uploadId: string;
  @Input() public fileType: number;
  @Output() public onComplete = new EventEmitter<string>();

  form: FormGroup;
  loading: boolean = true;

  private _subsink = new SubSink();
  private get has(): boolean {

    return this.form.get('file').value != null;
  }

  constructor(public _fileService: FileService,
    private _alertCtrl: AlertController,
    private _camera: FCCameraService,
    private _toastController: ToastController,
    private _actionSheetCtrl: ActionSheetController,
    _formBuilder: FormBuilder) {

    this.form = _formBuilder.group({
      file: null
    })

    this._subsink.sink = this.form.get('file').valueChanges
      .subscribe(() => {
        this.loading = false;
      })
  }

  ngOnInit() {

    this._subsink.sink = this._fileService.url({ fileType: this.fileType, uploadId: this.uploadId })
      .pipe(finalize(() => this.loading = false))
      .subscribe((url) => {

        this.form.get('file').patchValue(url);
      })
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  async showActionSheet() {

    const actionSheet = await this._actionSheetCtrl.create({
      cssClass: 'photo-action-sheet',
      header: 'Your photo',
      buttons: [
        { text: 'Camera', icon: 'camera', handler: () => this.loadFromCamera() },
        { text: 'Photo Library', icon: 'person', handler: () => this.loadFromPhotoLibrary() },
        { text: 'Delete', role: 'destructive', cssClass: 'action-sheet-danger', icon: 'close', handler: () => { this.delete() } },
      ],
    });
    actionSheet.present();
  }

  private loadFromCamera() {

    this.loading = true;
    this._camera.takePhoto({ fileType: this.fileType, uploadId: this.uploadId, sourceType: PictureSourceType.CAMERA })
      .then((photo) => {

        this.form.get('file').patchValue(photo.url);
        this.loading = false;
        this.onComplete.emit(photo.url);

      }).catch((error) => {

        this.loading = false;
        this.presentToast(error);
      });
  }

  private loadFromPhotoLibrary() {

    this.loading = true;
    this._camera.takePhoto({ fileType: this.fileType, uploadId: this.uploadId, sourceType: PictureSourceType.PHOTOLIBRARY })
      .then((photo) => {

        this.loading = false;
        this.form.get('file').patchValue(photo.url);
        this.onComplete.emit(photo.url);

      }).catch((error) => {

        this.loading = false;
        this.presentToast(error);
      });
  }

  private async presentToast(message: string) {

    const toast = await this._toastController.create({
      message: message,
      duration: 5000,
    })

    await toast.present();
  }

  private async delete() {

    if (this.has) {

      const alert = await this._alertCtrl.create({
        header: 'Confirm',
        subHeader: 'Are you sure you want to delete?',
        message: 'The deleted file can not be restored in the future.',
        buttons: [{
          text: 'OK', handler: () => {

            this._subsink.sink = this._fileService.delete({ fileType: this.fileType, uploadId: this.uploadId })
              .subscribe(() => {

                this.form.get('file').patchValue(null);
                this.onComplete.emit(null);
              })
          }
        }, { text: 'Cancel' }]
      })
      alert.present();
    }
  }
}
