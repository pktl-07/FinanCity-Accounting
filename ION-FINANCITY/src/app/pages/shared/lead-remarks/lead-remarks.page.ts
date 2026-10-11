import { Component, Input, Inject, ViewChild, OnInit, OnDestroy } from '@angular/core';
import { AlertController, IonContent, ToastController } from '@ionic/angular';
import { IFCConfig, fcDate, SubSink } from 'src/@common/utils';
import { BehaviorSubject } from 'rxjs';
import { ChattingRemarkService } from 'src/app/services/chatting-remark.service';
import { Chooser } from '@ionic-native/chooser/ngx';
import { FileService } from 'src/@common/components/native-devices/fc-file-service';
import { FCFileTransferService } from 'src/@common/components/native-devices/fc-file-transfer.service';
import { FCFileIconPipe } from 'src/@common/pipes/fc-file-icon.pipe';
import { FCFileOpenerService } from 'src/@common/components/file-opener/fc-file-openr.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'fc-lead-remarks',
  templateUrl: './lead-remarks.page.html',
  providers: [
    ChattingRemarkService,
    Chooser,
    FileService,
    FCFileTransferService,
    FCFileIconPipe,
    FCFileOpenerService
  ],
  styles: [`
  ion-footer{
     position: fixed;
     bottom: 0px;
   }

   .card {
    --background: yellow !important;
  }

   ion-grid{
     padding-bottom: 100px;
   }`],
})
export class LeadRemarkPage implements OnInit, OnDestroy {

  @Input() key: string;
  @Input() InsertedItems = new BehaviorSubject([]);
  @Input() DeleteItems = new BehaviorSubject([]);
  @ViewChild(IonContent, { read: IonContent }) _content: IonContent;

  private timeouts = [];
  private _subsink = new SubSink();

  config : IFCConfig = environment;

  constructor(public _alertController: AlertController,
    private _fcFileOpenerService: FCFileOpenerService,
    public _fileUploadService: FCFileTransferService,
    private _fcFileIconPipe: FCFileIconPipe,
    public _fileService: FileService,
    private _toastController: ToastController,
    private _chooser: Chooser,
    private _chatting: ChattingRemarkService) {
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
    this.timeouts.forEach((timeout) => {
      clearTimeout(timeout)
    })
  }

  ngOnInit(): void {
    this.scrollToBottom();
  }

  scrollToBottom() {
    this.timeouts.push(setTimeout(() => {

      if (this._content.scrollToBottom)
        this._content.scrollToBottom(400);
    }, 1));
  }

  messageTemplate: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  handleInputRemark(e: any) {
    if (e.target.value != '')
      this.messageTemplate.next(true);
    else
      this.messageTemplate.next(false);
  }

  handleAddMessage(message: any) {

    this.InsertedItems.value.push({
      SenderSK: environment.auth.SPSK,
      RefSK: this.key,
      Type: 1,
      CreatedDate: fcDate(),
      Message: message.value
    });

    if (this.key != null) {
      const lastItem = this.InsertedItems.value[this.InsertedItems.value.length - 1]
      this._subsink.sink = this._chatting.save(lastItem)
        .subscribe(() => this.scrollToBottom());
    }
    this.messageTemplate.next(false);
    message.value = null;
  }

  handleAttachment() {

    this._chooser.getFile().then(file => {
      if (file) {
        this._fileUploadService.toBlob(file.dataURI).then((blob) => {
          const dataItem = {
            SenderSK: environment.auth.SPSK,
            Message: file.name,
            Type: 2,
            Size: blob.size,
            RefSK: this.key,
            CreatedDate: fcDate(),
          }
          this._subsink.sink = this._chatting.save(dataItem)
            .subscribe((response: any) => {
              dataItem['SysKey'] = response.SysKey;
              this._fileUploadService.uploadBase64({
                path: file.dataURI,
                fileType: 15,
                fileName: file.name,
                uploadId: response.SysKey,
                mediaType: file.mediaType
              }).then(() => {
                this.InsertedItems.value.push(dataItem);
                this.scrollToBottom();
              }).catch((error) => this.presentToast(error))
            }, () => this.presentToast('Error occur while saving the record.'))
        }).catch((err) => this.presentToast(err));
      }
    }).catch((err) => this.presentToast(err));
  }

  private async presentToast(message: string) {
    const toast = await this._toastController.create({
      message: message,
      duration: 3000
    })
    await toast.present();
  }

  public icon(name: string) {
    return this._fcFileIconPipe.transform(name);
  }

  public downlaod(dataItem: any) {
    if (dataItem && dataItem.Type == 2) {

      dataItem['Loading'] = true;

      this._fcFileOpenerService.openDownload({
        fileType: 15,
        uploadId: dataItem.SysKey
      }).then(() => {

        dataItem['Loading'] = false;
      }).catch((err) => {

        dataItem['Loading'] = false;
        this.presentToast(err);
      })
    }
  }
}