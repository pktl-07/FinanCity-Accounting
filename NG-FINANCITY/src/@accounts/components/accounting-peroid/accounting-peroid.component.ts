import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { FormBuilder, FormControl } from '@angular/forms';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { Subject, from } from 'rxjs';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { Router } from '@angular/router';
import { groupBy, map, finalize, catchError } from 'rxjs/operators';
import { AccountPeriodService } from 'src/@accounts/services/account-period.service';
import { kendoFilter, SubSink } from 'src/@common/service/utility-service';
import { PConfirmDialogComponent } from 'src/@common/components/p-confirm-dialog/p-confirm-dialog.component';
import { DialogAction } from '@progress/kendo-angular-dialog';
import { AccountingPeriodCreateComponent } from './accounting-period-create/accounting-period-create.component';
import { PageLoaderService } from 'src/@common/components/p-page-loader/p-page-loader.service';
import { PErrorComponent } from 'src/@common/components/p-confirm-dialog/p-error-component';
import { ToastService } from 'src/@common/components/p-toast/p-toast.service';

@Component({
  selector: 'app-accounting-peroid',
  templateUrl: './accounting-peroid.component.html',
  providers: [
    AccountPeriodService,
    ToastService
  ]
})
export class AccountingPeriodComponent extends DetailFormBase implements OnInit, OnDestroy {

  @ViewChild(PToolbarComponent, { static: true }) _toolbar: PToolbarComponent;
  @ViewChild('confirmDialog', { static: true }) confirmDialog: PConfirmDialogComponent;

  private _subsink = new SubSink();
  accountingPeriod = new Array<{ value: string, text: string }>();
  currentAccountingYear = new Subject();

  constructor(fb: FormBuilder,
    public _toastService: ToastService,
    public _router: Router,
    public _pageLoaderService: PageLoaderService,
    public _accountingPeriod: AccountPeriodService) {

    super(fb);
  }

  ngOnInit() {

    this.form = this.fb.group({
      'AccountingPeriod': new FormControl(),
    });

    this._toolbar
      .controls
      .Success(this.OnSuccess)
      .Ready(this.formReady)
      .NavigationButton({ action: () => this._router.navigateByUrl('') });

    this.poppulateUI();

    this.setCurrentAccountingYear();

    this._subsink.sink = this.form.get('AccountingPeriod').valueChanges.subscribe(data => {
      this._accountingPeriod.asyncGetAccountingPeriod(data);
    });
  }

  ngOnDestroy() {
    this._subsink.unsubscribe();
  }

  createAccountingYear() {

    let ref = this.confirmDialog.open({
      title: 'Confirm',
      content: AccountingPeriodCreateComponent,
      action: [{ text: 'OK', primary: true },
      { text: 'Cancel' }]
    });

    this._subsink.sink = ref.result
      .subscribe((d: DialogAction) => {
        if (d && d.text.toLowerCase() == 'ok') {

          this._pageLoaderService.loading = true;

          this._subsink.sink = this._accountingPeriod.save()
            .subscribe(() => {
              this.poppulateUI()
                .then(() => {
                  this._pageLoaderService.loading = false;
                  this._toastService.open({
                    data: {
                      message: `${this.accountingPeriod[this.accountingPeriod.length - 1].text} has created.`,
                      title: 'Information'
                    }
                  })
                })
            })
        }
      })
  }

  handleUpdate({ status, module, pc, yc }) {

    this._pageLoaderService.loading = true;

    this._subsink.sink = this._accountingPeriod.update({
      Status: status == 1 ? 0 : 1,
      ModuleCode: module,
      YC: yc,
      PC: pc
    })
      .pipe(catchError(err => {

        this._pageLoaderService.loading = false;
        this.setErrorDialog(err);
        return err;
      }),
        finalize(() => this._pageLoaderService.loading = false))
      .subscribe(() => {

        this._accountingPeriod.asyncGetAccountingPeriod(yc);
      })
  }

  private setErrorDialog(err) {

    let d = (err.error.Message as string).replace(/--(.*):/g, '*');
    let m = d.split('*');

    let ref = this.confirmDialog.open({
      title: 'Errors',
      content: PErrorComponent,
      action: [{ text: 'OK' }]
    });

    ref.content.instance.errors = m;
  }

  private compareFn = (a, b) => {

    if (a.YC < b.YC) return -1;
    if (a.YC > b.YC) return 1;
    return 0;
  }

  private setCurrentAccountingYear() {

    this._subsink.sink = this._accountingPeriod.finds({
      filter: {
        logic: 'and',
        filters: kendoFilter([
          { field: 'IsDefaultYear', value: true, operator: 'eq', logic: 'and' }
        ])
      }
    })
      .subscribe((data: any[]) => {

        if (data) {
          const yc = data[0]['YC'];
          const _yc = data[0]['Date1'].toString().substring(0, 4);

          this.currentAccountingYear.next(`${yc} [${_yc}]`);
        }
      })
  }

  private poppulateDropDown() {

    this.accountingPeriod = [];

    return new Promise<any>((resolve, reject) => {

      this._subsink.sink = this._accountingPeriod.finds()
        .pipe(
          map(data => data.sort(this.compareFn)),
          map((data: { YC: string, Date1: number }[]) => data),
        )
        .subscribe(data => {

          this._subsink.sink = from(data)
            .pipe(
              map(({ YC }) => YC),
              groupBy(group => group),
            ).subscribe(g => {

              this.accountingPeriod.push({
                value: g.key,
                text: `${g.key} [${data.filter(x => x.YC == g.key)[0].Date1.toString().substring(0, 4)}]`
              })
              resolve(data);
            })
        })
    })
  }

  private poppulateUI() {

    return new Promise<any>((resolve) => {
      this.poppulateDropDown()
        .then((data: any[]) => {

          let d = data.sort(this.compareFn);
          const lastAccountingYear = d[d.length - 1]['YC'];
          this.form.get('AccountingPeriod').patchValue(lastAccountingYear);
          this._accountingPeriod.asyncGetAccountingPeriod(lastAccountingYear);
          this.setCurrentAccountingYear();

          resolve(null);
        });
    });
  }
}
