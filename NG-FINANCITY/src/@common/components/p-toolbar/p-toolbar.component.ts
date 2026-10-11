import { Component, ViewChild, Input, OnDestroy, ContentChild, OnInit, AfterViewInit } from '@angular/core';
import { PConfirmDialogComponent } from '../p-confirm-dialog/p-confirm-dialog.component';
import { ButtonControlConfig, IDataToolbarConfig } from 'src/@common/Infrastructure/common-interface';
import { ButtonToolbarControlService } from './p-toolbar-control.service';
import { Subject, from, throwError } from 'rxjs';
import { ButtonControlBuilder } from './button-control.builder';
import { PToolbarTemplateDirective } from './p-toolbar-template.directive';
import { PErrorComponent, PValidationFailComponent } from '../p-confirm-dialog/p-error-component';
import { ToastService } from '../p-toast/p-toast.service';
import { tap, finalize, catchError } from 'rxjs/operators';
import { findObject, rebindJsonFormGroup, SubSink, verd_ed } from 'src/@common/service/utility-service';
import { PWarningCreateComponent } from '../p-confirm-dialog/p-error-component';
import { PageLoaderService } from '../p-page-loader/p-page-loader.service';

@Component({
  selector: 'p-toolbar',
  templateUrl: './p-toolbar.component.html',
  styleUrls: ['./p-toolbar.component.scss'],
  providers: [
    ButtonToolbarControlService,
    ToastService,
    ButtonControlBuilder]
})
export class PToolbarComponent implements OnInit, AfterViewInit, OnDestroy {

  @ContentChild(PToolbarTemplateDirective, { static: true }) statusTemplate: PToolbarTemplateDirective;
  @ViewChild('confirmDialog', { static: true }) confirmDialog: PConfirmDialogComponent;
  @Input() allowUpdateAfterPost: boolean = true;
  @Input() title: string = '';
  @Input() type: number = 0;

  private _subsink = new SubSink();
  private _error = new Subject();
  private _toast = new Subject<ButtonControlConfig>();

  private _config: IDataToolbarConfig = { buttons: [] };
  get config(): IDataToolbarConfig {
    return this._config;
  }

  constructor(public _toobarService: ButtonToolbarControlService,
    private _toastService: ToastService,
    public controls: ButtonControlBuilder,
    private pageLoaderServie: PageLoaderService) {

    controls._control = this;
    _toobarService.allowUpdateAfterPost = this.allowUpdateAfterPost;
  }

  ngOnInit(): void {

    if (typeof this._config.start == 'undefined') {
      this._config.start = new Subject();
    }

    //initially set the buttons to disalbed all.
    //set initial button control state.  
    if (this._config.fromReady) {
      this._subsink.sink = this._config.fromReady
        .subscribe(item => {
          if (item.ready) {

            let a = findObject(this._config.fromReady.value.form.value, this._config.statekey)
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
        })
    }

    //present toast if registered.
    this._subsink.sink = this._toast
      .subscribe(t => {
        this.presentToast(t);
      })

    //present error if present.
    this._subsink.sink = this._error
      .subscribe(e => {

        let s = findObject(this._config.fromReady.value.form.value, this._config.statekey);
        this.changeState(s);
        this.presentError(e);
      })

    this._subsink.sink = this._config.start
      .subscribe((item: boolean) => {
        if (item == true) {
          this._toobarService.disableAll(this._config.buttons);
        }
      })
  }

  ngAfterViewInit(): void {
  }

  ngOnDestroy(): void {

    this._subsink.unsubscribe();
    this._toobarService.destory();
  }

  changeState(state: number = 0) {

    this._toobarService.changeState(state, this._config.buttons);
  }

  getNavigateNewAction(): ButtonControlConfig {

    return this._config.buttons.filter(x => x.label.toLowerCase() == 'navigate-new')[0];
  }

  getNavigateAction(): ButtonControlConfig {

    return this._config.buttons.filter(x => x.label.toLowerCase() == 'navigate')[0];
  }

  getExceptNavigateAction(): Array<ButtonControlConfig> {

    return this._config.buttons.filter(x => x.label.toLowerCase() != 'navigate').sort(x => x.orderyBy);
  }

  do(item: ButtonControlConfig): any {

    if (item?.disabled == undefined || item?.disabled == false || item?.hide == false) {

      if (item.action.requireConfirmation) {

        this._toobarService.action(
          this.confirmDialog,
          item,
          this._error,
          this._toast,
          PWarningCreateComponent,
          PValidationFailComponent,
          this._config.success,
          this._config.start,
          this.config.fromReady.value.form.dirty);
      }
      else {

        if (item.label.toLowerCase() == 'navigate-new' || item.label.toLowerCase() == 'navigate')
          item.action.action();
        else {
          const validate: boolean = (typeof item.action.validate == 'undefined') ? true : item.action.validate();
          if (validate) {

            if (item.action.requireLoading) {
              this.pageLoaderServie.loading = true;
              this.pageLoaderServie.circularLoading.next(true);
            }

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
                catchError((e) => {

                  this.presentError({ error: { Message: 'Error while generating the report. Please inform our administrator.' } })
                  return throwError(e);
                }),
                finalize(() => {
                  if (item.action.requireLoading) {
                    this.pageLoaderServie.loading = false;
                    this.pageLoaderServie.circularLoading.next(false);
                  }
                })
              ).subscribe();
          }
        }
      }
    }
  }

  private presentToast(item: ButtonControlConfig) {
    this._toastService.open({
      data: {
        message: `Your data has been ${verd_ed[item.label.toLowerCase()]}.`,
        title: 'Information'
      }
    })
  }

  private presentError(error: any) {

    let message = "";
    if (error.error.Message)
      message = (error.error.Message as string).replace(/--(.*):/g, '*');
    else
      message = error?.message

    let m = message.split('*');
    let a = this.confirmDialog.open({
      title: 'Errors',
      content: PErrorComponent,
      action: [{ text: 'OK' }],
    });
    a.content.instance.errors = m;
  }
}
