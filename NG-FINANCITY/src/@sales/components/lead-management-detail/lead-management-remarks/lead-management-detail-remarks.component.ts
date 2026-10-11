import { Component, OnInit, OnDestroy, Input, ViewChild, AfterViewInit } from '@angular/core';
import { LeadService } from 'src/@sales/services/lead.service';
import { fcDate, SubSink } from 'src/@common/service/utility-service';
import { PConfirmDialogComponent } from 'src/@common/components/p-confirm-dialog/p-confirm-dialog.component';
import { ChattingRemarkService } from 'src/@sales/services/chatting-remark.service';
import { FileService } from 'src/@common/service/file.service';
import { BehaviorSubject } from 'rxjs';
import { mergeMap, tap } from 'rxjs/operators';

@Component({
    selector: 'app-lead-management-detail-remark',
    template: `
    <input id="fileInput" type="file" style="display:none;" />
    <div class='çard fc-message-body'>
        <div class="card-body fc-modal-content" id='content' #scroll>
            <div class="row m-5" *ngFor="let dataItem of insertedItems | async">
                <ng-container *ngIf="(dataItem?.SenderSK == '1')">
                    <ng-template [ngTemplateOutlet]="senderTemplate" [ngTemplateOutletContext]="{dataItem : dataItem}">
                    </ng-template>       
                </ng-container>
                <ng-container *ngIf="(dataItem?.SenderSK != '1')">
                    <ng-template [ngTemplateOutlet]="receiveTemplate" [ngTemplateOutletContext]="{dataItem: dataItem}">
                    </ng-template>
                </ng-container>
            </div>
        </div>
        <div class="card-footer">
            <div class='input-group-append'>
                <input #message (input)='handleMessageChange($event)' kendoTextBox placeholder='Type any message' type="text" class="form-control">
                <button *ngIf="(messageTemplate | async) == true" kendoButton (click)="handleAdd(message)" [primary]="true" [look]="'flat'">
                    <i class="fas fa-paper-plane"></i>
                </button>
                <button *ngIf="(messageTemplate | async) == false" 
                    type='file' kendoButton (click)="handleAttachment()" [primary]="true" [look]="'flat'">
                    <i class="fas fa-paperclip"></i>
                </button>
            </div>
        </div>
    </div>
    <ng-template #receiveTemplate let-dataItem="dataItem">
        <div>
            <div class="d-flex justify-content-start">
                <img [src]='dataItem.SenderSK | pSalespersonImage | async' 
                    class="fc-message-body fc-rounded-photo" onerror="this.src='assets/imgs/empty-picture.png'">
            </div>
        </div>
        <div class='col-md-6 mt-2'>
            <ng-template [ngTemplateOutlet]="infotemplate" [ngTemplateOutletContext]="{dataItem: dataItem}">
            </ng-template>
            <div class='card'>
                <div class='card-body p-3'>
                    <div>
                        <div style='position:relative;left:5px;right:5px'>
                            {{ dataItem?.Message | uppercase }}
                        </div>
                        <div style='position:relative;left:5px;right:5px' *ngIf="dataItem?.Type == 2">
                            <ng-template [ngTemplateOutlet]="filetemplate" [ngTemplateOutletContext]="{dataItem: dataItem}">
                            </ng-template>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </ng-template>
    <ng-template #senderTemplate let-dataItem="dataItem">
        <div class="col-md-12">
            <div class="d-flex justify-content-end">
                <ng-template [ngTemplateOutlet]="infotemplate" [ngTemplateOutletContext]="{dataItem: dataItem}">
                </ng-template>
            </div>
        </div>
        <div class='col-md-6 offset-md-6 mt-2'>
            <div class='card'>
                <div class='çard-body p-3'>
                    <div>
                        <div style='padding:3px'>
                           <div style='text-align:right'>{{ dataItem?.Message | uppercase }}</div>
                        </div>
                        <div style='padding:3px' *ngIf="dataItem?.Type == 2">
                            <div style='text-align:right'>
                                <ng-template [ngTemplateOutlet]="filetemplate" [ngTemplateOutletContext]="{dataItem: dataItem}">
                                </ng-template>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </ng-template>
    <ng-template #filetemplate let-dataItem='dataItem'>
        <br>
        <div>
            <div [ngClass]='{"k-i-loading" : dataItem?.Loading == true}'></div>
            <i class="{{dataItem?.Message | pFileIcon}} fa-3x"></i>
            <p>{{ dataItem?.Size | pFileSize }}</p>
            <p>
                <button class='btn btn-primary' (click)="handleDownload(dataItem)">
                    Download
                </button>
            </p>
        </div>
    </ng-template>
    <ng-template #infotemplate let-dataItem='dataItem'>
        <span class="text-muted"><b>{{ dataItem?.SenderSK | pSalespersonName | async }}</b>&nbsp;has posted on:&nbsp;{{dataItem?.CreatedDate | pDateformat}}</span>
    </ng-template>
    <confirm-dialog #confirmDialog></confirm-dialog>
    `,
    styles: [`
        .fc-rounded-photo{
            width:50px !important;
            height:50px !important;
        }
        .fc-message-body{
            width: 100%;
            height: 90%;
            position: relative;
        }
    `],
    providers: [LeadService,
        FileService
    ]
})
export class LeadManagementRemarkComponent implements OnInit, AfterViewInit, OnDestroy {

    @ViewChild('confirmDialog', { static: true }) dialog: PConfirmDialogComponent;
    @ViewChild('scroll', { static: true }) scroll: any;

    @Input() insertedItems = new BehaviorSubject([]);
    @Input() deletedItems = new BehaviorSubject([]);

    @Input() key: string;

    private _subsink = new SubSink();
    private _listeners = [];

    constructor(public _leadService: LeadService,
        public _fileService: FileService,
        private _chattingRemark: ChattingRemarkService) {
    }

    ngOnInit() {

        const listener = document.getElementById('fileInput').onchange = () => {

            const files = (event.target as any).files;
            const file = files[0];
            const dataItem = {
                SenderSK: '1',
                Message: file.name,
                Size: file.size,
                Type: 2,
                RefSK: this.key,
                Loading: false,
                CreatedDate: fcDate(),
            }

            this._subsink.sink = this._chattingRemark.save(dataItem)
                .pipe(
                    tap((response) => {
                        dataItem['SysKey'] = response.SysKey
                    }),
                    mergeMap((data) => {
                        return this._fileService.upload({
                            file: files,
                            fileType: 15,
                            uploadId: data.SysKey
                        })
                    })
                )
                .subscribe(() => {

                    this.insertedItems.value.push(dataItem);
                    this.scrollToBottom();
                    this._listeners.push(listener);
                })
        }
    }

    ngAfterViewInit(): void {

        this.scrollToBottom();
    }

    ngOnDestroy(): void {

        this._subsink.unsubscribe();
        this._listeners.forEach(listener => listener());
    }

    handleAdd(message) {

        this.insertedItems.value.push({
            SenderSK: '1',
            Message: message.value,
            Type: 1,
            RefSK: this.key,
            CreatedDate: fcDate(),
        });

        if (this.key != null) {

            const lastItem = this.insertedItems.value[this.insertedItems.value.length - 1];
            this._subsink.sink = this._chattingRemark.save(lastItem)
                .pipe(
                    tap(response => {

                        lastItem.SysKey = response.SysKey;
                    }))
                .subscribe(() => this.scrollToBottom());
        }

        this.messageTemplate.next(false);
        message.value = null;
    }

    messageTemplate: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
    handleMessageChange(e: any) {

        if (e.target.value != '')
            this.messageTemplate.next(true);
        else
            this.messageTemplate.next(false);
    }

    handleAttachment() {

        document.getElementById('fileInput').click();
    }

    handleDownload(dataItem: any) {

        this.downloadFile(dataItem.SysKey);
    }

    private downloadFile(key: string) {

        this._subsink.sink = this._fileService.download({
            fileType: 15,
            uploadId: key,
        }).subscribe();
    }

    private scrollToBottom() {

        this.scroll.nativeElement.scrollTop = this.scroll.nativeElement.scrollHeight;
    }

    private removeFile(dataItem) {

        const confirm = this.dialog.open({
            title: 'Confirm',
            content: 'The selected file type is not supported.',
            action: [
                { text: 'OK' },
            ]
        })
        this._subsink.sink = confirm.result
            .subscribe((data: any) => {
                if (data.text == 'OK') {
                    if (dataItem.SysKey)
                        this.deletedItems.value.push(dataItem);

                    this.insertedItems.value.splice(this.insertedItems.value.indexOf(dataItem), 1)
                }
            });
    }
}
