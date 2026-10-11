import { Component, ViewChild, OnDestroy, ContentChild, TemplateRef, OnInit, Input } from '@angular/core';
import { PConfirmDialogComponent } from '../p-confirm-dialog/p-confirm-dialog.component';
import { ButtonControlConfig, IDataToolbarConfig } from 'src/@common/Infrastructure/common-interface';
import { ButtonToolbarControlService } from '../p-toolbar/p-toolbar-control.service';
import { Subject, from } from 'rxjs';
import { ButtonControlBuilder } from '../p-toolbar/button-control.builder';
import { ModalContentTemplateDirective } from './p-modal-content-template.directive';
import { ModalToolbarTemplateDirective } from './p-modal-toobar-template.directive';
import { tap } from 'rxjs/operators';
import { findObject, rebindJsonFormGroup, SubSink, verd_ed } from 'src/@common/service/utility-service';
import { PErrorComponent, PWarningCreateComponent, PValidationFailComponent } from '../p-confirm-dialog/p-error-component';
import { ToastService } from '../p-toast/p-toast.service';

@Component({
  selector: 'p-modal-form',
  exportAs: 'modal',
  template: `
  <div class="card fc-modal-app">
   <div class="card-body fc-modal-content" *ngIf="loading">
      <ng-container>
        <ng-container *ngTemplateOutlet="skeletonLoading"></ng-container>
      </ng-container>
    </div>
     <h5 class="card-header" *ngIf="!loading && statusTemplate?.templateRef" 
      style="background-color:var(--light);text-align:right">
      <ng-template [ngTemplateOutlet]="statusTemplate?.templateRef"></ng-template>
    </h5>
    <div class="card-body fc-modal-content" *ngIf="!loading">
      <ng-template [ngIf]="!contentTemplateDirective?.templateRef"
       [ngTemplateOutlet]="contentTemplateDirective?.templateRef">
      </ng-template>
    </div>
    <div class="card-footer">
      <ng-container *ngFor="let b of config?.buttons">
        <button type="button" [class.disabled]="b.disabled" (click)="do(b)"
          class="btn btn-primary ml-2 fw-1" 
          [ngClass]="{'ml-5': b.label.toLocaleLowerCase() == 'post',
          'btn-danger': b.label.toLocaleLowerCase() == 'void'}">{{ buttonLabel(b) }}</button>
      </ng-container>
    </div>
  </div>
  <ng-template #skeletonLoading>
    <p-skeleton-loading></p-skeleton-loading>
  </ng-template>
  <confirm-dialog #confirmDialog></confirm-dialog>`,
  providers: [
    ButtonControlBuilder,
    ButtonToolbarControlService,
  ],
  styles: [
    `
    .visible  {
      visibility: hidden;
    }`
  ]
})
export class PModalFormComponent implements OnInit, OnDestroy {

  @Input() loading: boolean = false;
  @Input() allowUpdateAfterPost: boolean = true;
  @ViewChild('confirmDialog', { static: true }) confirmDialog: PConfirmDialogComponent;
  @ContentChild(ModalContentTemplateDirective, { static: true }) contentTemplateDirective: ModalContentTemplateDirective;
  @ContentChild(ModalToolbarTemplateDirective, { static: true }) statusTemplate: ModalToolbarTemplateDirective;

  public contentTemplate: TemplateRef<any>;

  private _config: IDataToolbarConfig = { buttons: [] };
  get config(): IDataToolbarConfig {

    this._config.buttons.sort(x => x.orderyBy);
    return this._config;
  }

  private _subsink = new SubSink();
  private _error = new Subject();
  private _toast = new Subject<ButtonControlConfig>();

  constructor(private _buttonControlService: ButtonToolbarControlService,
    private _toastService: ToastService,
    public controls: ButtonControlBuilder) {

    controls._control = this;
  }

  changeState(state: number = 0) {
    this._buttonControlService.changeState(state, this._config.buttons);
  }

  buttonLabel(btn: ButtonControlConfig) {

    return btn.label.toLowerCase() == 'download pdf' ? 'Print' : btn.label;
  }

  ngOnInit(): void {

    this._buttonControlService.allowUpdateAfterPost = this.allowUpdateAfterPost;

    if (typeof this._config.start == 'undefined')
      this.config.start = new Subject();

    //initially set the buttons to disalbed all.
    this._buttonControlService.disableAll(this._config.buttons);
    //set initial button control state.  
    if (this._config.fromReady) {
      this._subsink.sink = this._config.fromReady
        .subscribe(item => {
          if (item.ready) {

            let a = findObject(this._config.fromReady.value.form.value, this._config.statekey);
            this.changeState(a);
          }
        });
    }

    //successful call back when action done.
    if (this._config.success) {

      this._subsink.sink = this._config.success
        .subscribe(item => {
          if (this._config.fromReady && this._config.statekey) {

            let s = findObject(item.data, this._config.statekey);

            this.changeState(s);
            rebindJsonFormGroup(this._config.fromReady.value.form, item.data, s == 0 ? 'null custom form data' : 'none');
          }
        });
    }

    //present toast if registered.
    this._subsink.sink = this._toast
      .subscribe(t => {
        this.presentToast(t);
      })

    //present error if present.
    this._subsink.sink = this._error.subscribe(e => {

      let s = findObject(this._config.fromReady.value.form.value, this._config.statekey);
      this.changeState(s);
      this.presentError(e);
    })

    this._subsink.sink = this._config.start
      .subscribe((item: boolean) => {
        if (item == true) {
          this._buttonControlService.disableAll(this._config.buttons);
        }
      })
  }

  ngOnDestroy(): void {

    this._subsink.unsubscribe();
    this._buttonControlService.destory();
  }

  do(item: ButtonControlConfig) {

    if (item.disabled == undefined || item.disabled == false) {
      if (item.action.requireConfirmation) {

        this._buttonControlService.action(
          this.confirmDialog,
          item,
          this._error,
          this._toast,
          PWarningCreateComponent,
          PValidationFailComponent,
          this._config.success,
          this.config.start);

      } else {

        this._subsink.sink = from(item.action.action())
          .pipe(
            tap(data => {
              if (this._config.success
                && item.label.toLowerCase() != 'recap'
                && item.label.toLowerCase() != 'download pdf'
                && item.label.toLowerCase() != 'download cvs') {
                this._config.success.next({ data: data, action: item.label })
              }
            }),
          ).subscribe();
      }
    }
  }

  presentToast(item: ButtonControlConfig) {

    this._toastService.open({
      data: {
        message: `Record has been ${verd_ed[item.label.toLowerCase()]}.`,
        title: 'Information'
      }
    });
  }

  presentError(error: any) {

    let d = (error?.error?.Message as string).replace(/--(.*):/g, '*');
    let m = d.split('*');

    let a = this.confirmDialog.open({
      title: 'Errors',
      content: PErrorComponent,
      action: [{ text: 'OK' }],
    });
    a.content.instance.errors = m;
  }
}


