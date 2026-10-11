import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { FormBuilder } from '@angular/forms';
import { Subject } from 'rxjs';
import { Router } from '@angular/router';
import { AccountPeriodService } from 'src/@accounts/services/account-period.service';
import { takeUntil, map, mergeMap, finalize } from 'rxjs/operators';
import { PageLoaderService } from 'src/@common/components/p-page-loader/p-page-loader.service';
import { AccountYearClosingService } from 'src/@accounts/services/account-year-closing.service';
import { ToastService } from 'src/@common/components/p-toast/p-toast.service';
import { PConfirmDialogComponent } from 'src/@common/components/p-confirm-dialog/p-confirm-dialog.component';
import { DialogAction } from '@progress/kendo-angular-dialog';

@Component({
  selector: 'app-year-closing.component',
  templateUrl: './year-closing.component.html',
  providers: [
    AccountPeriodService,
    AccountYearClosingService
  ]
})
export class AccountYearClosingComponent extends DetailFormBase implements OnInit, OnDestroy {

  @ViewChild('confirmDialog', { static: true }) confirmDialog: PConfirmDialogComponent;

  private _unsubscribe = new Subject<any>();

  currentYearSubject = new Subject();
  currentYearCode: string;
  currentYear: number;

  constructor(fb: FormBuilder,
    private _router: Router,
    public _toastService: ToastService,
    private _accountYearClosingService: AccountYearClosingService,
    public _pageLoaderService: PageLoaderService,
    public _accountingPeriod: AccountPeriodService) {

    super(fb);
  }

  ngOnInit() {

    this.poppulateCurrentYear().then();
  }

  private poppulateCurrentYear() {

    return new Promise((resolve) => {
      this._accountingPeriod.getCurrentAccountingYearCode()
        .pipe(mergeMap((code: string) => {
          return this._accountingPeriod.getCurrentAccountingYear().pipe(takeUntil(this._unsubscribe),
            map((response: number) => {
              response = response == null ? new Date().getUTCFullYear() - 1 : response;
              return {
                yearcode: code,
                year: response,
              }
            })
          )
        })
        ).subscribe(data => {

          this.currentYearCode = data.yearcode;
          this.currentYear = data.year;
          this.currentYearSubject.next(`${this.currentYearCode} [${this.currentYear}]`);
          resolve(null);
        })
    })
  }


  handleCloseAccoountingYear() {

    let a = this.confirmDialog.open({
      title: 'Close Accounting Year',
      content: `Are you sure you want to close accounting year for ${this.currentYearCode} [${this.currentYear}].`,
      action: [{ text: 'OK' }, { text: 'Cancel' }],
    }).result
      .pipe(takeUntil(this._unsubscribe))
      .subscribe((d: DialogAction) => {

        if (d.text.toLocaleLowerCase() == 'ok') {
          this._pageLoaderService.loading = true;
          this._accountYearClosingService.post({})
            .pipe(finalize(() => this._pageLoaderService.loading = false))
            .subscribe(() => {
              this.poppulateCurrentYear().then(() => {
                this._toastService.open({
                  data: {
                    message: `${this.currentYearCode} [${this.currentYear}] has been closed.`,
                    title: 'Information'
                  }
                })
              })
            })
        }
      })
  }

  handleClose() {

    this._router.navigate(['']);
  }

  ngOnDestroy(): void {

    this._unsubscribe.next();
    this._unsubscribe.complete();
  }
}
