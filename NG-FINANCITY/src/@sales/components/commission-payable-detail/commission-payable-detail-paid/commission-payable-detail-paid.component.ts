import { Component, OnInit, OnDestroy, Input, ViewChild, Inject } from '@angular/core';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { FormBuilder, FormControl, Validators, FormGroup } from '@angular/forms';
import { CashbookSetupService } from 'src/@accounts/services/cashbook-setup.service';
import { WindowState } from '@progress/kendo-angular-dialog';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { catchError, finalize, mergeMap, map, tap } from 'rxjs/operators';
import { PModalFormComponent } from 'src/@common/components/p-modal-form/p-modal-form.component';
import { CommissionPayableService } from 'src/@sales/services/commission-payable.service';
import { paymentMode, makeFormTouched, createJsonFormGroup, kendoFilter, multiCurrency, paymentType, SubSink } from 'src/@common/service/utility-service';
import { PageUtilityService } from '../page.utility.service';
import { CommissionReceivedAllocationService } from 'src/@sales/services/commission-received-allocation.service';
import { SalespersonManagementService } from 'src/@sales/services/salesperson-management.service';
import { MarketingExpenseAllocationService } from 'src/@sales/services/marketing-expense-allocation.service';
import { CashbookSetupDetailComponent } from 'src/@accounts/components/cashbook-setup-detail/cashbook-setup-detail.component';
import { CurrencyRateService } from 'src/@sales/services/currency-rate.service';
import { CurrencyService } from 'src/@sales/services/currency.service';
import { RecapDailogComponent } from 'src/@common/components/p-recap-dialog/p-recap-dialog.component';
import { CommissionPaymentPrintingService } from 'src/@printing/services/commission-payment-printing';
import { PropertyTransactionDetailService } from 'src/@sales/services/property-transaction-detial.service';
import { environment } from 'src/environments/environment';

export interface MarketingExpenseInfo {
  MarketingExpense: number,
  TotalExpenseAllocated: number,
  BatchAllocated: number
}

@Component({
  selector: 'app-commission-payable-detail-paid',
  templateUrl: './commission-payable-detail-paid.component.html',
  providers: [
    CommissionPayableService,
    CashbookSetupService,
    PWindowComponentService,
    CommissionReceivedAllocationService,
    SalespersonManagementService,
    MarketingExpenseAllocationService,
    CurrencyRateService,
    CurrencyService,
    PropertyTransactionDetailService,
    CommissionPaymentPrintingService,
    { provide: 'payable-1', useClass: CommissionPayableService },
    { provide: 'payable-2', useClass: CommissionPayableService }
  ]
})
export class CommissionPayableDetailPaidComponent extends DetailFormBase implements OnInit, OnDestroy {

  @ViewChild(PModalFormComponent, { static: true }) public _modalForm: PModalFormComponent;
  @ViewChild('recapDailog', { static: true }) private _recap: RecapDailogComponent;

  public _paymentMode = [];
  private _subsink = new SubSink();
  public _paymentType = [];
  public baseCurr = environment.general.currSK;

  @Input('InvoiceSK') public InvoiceSK: string;
  @Input('DetailSK') public DetailSK: string;
  @Input('SPSK') public SPSK: string;
  @Input('ALOSK') public ALOSK: string;
  @Input('MarketingExpense') public MarketingExpense: number;

  get readonly() {
    return this.form.get('RecordStatus').value >= 6;
  }

  constructor(fb: FormBuilder,
    private _currencyRateService: CurrencyRateService,
    private _currencyService: CurrencyService,
    private _printingService: CommissionPaymentPrintingService,
    private _transactionDetailService: PropertyTransactionDetailService,
    private _allocationService: CommissionReceivedAllocationService,
    private _salespersonManagement: SalespersonManagementService,
    private _windowService: PWindowComponentService,
    private _pageUtilityService: PageUtilityService,
    private _marketingExpenseAllocate: MarketingExpenseAllocationService,
    @Inject('payable-1') private _commissionPayableService1: CommissionPayableService,
    @Inject('payable-2') private _commissionPayableService2: CommissionPayableService,
    public _cashbookService: CashbookSetupService) {

    super(fb);
    this._paymentMode = paymentMode();
  }

  ngOnInit() {

    this._cashbookService.asyncFinds();
    this._paymentType = paymentType();

    this._modalForm
      .controls
      .Ready(this.formReady)
      .Success(this.OnSuccess)
      .StateKey(['RecordStatus'])
      .NewButton({
        action: () => this._commissionPayableService1.default().pipe(
          finalize(() => {
            this.setFullAmount('Required to set form value');
          })
        )
      })
      .SaveButton({
        action: () => this._commissionPayableService1.save(this.form.value)
          .pipe(
            tap((data) => {
              this._subsink.sink = this._marketingExpenseAllocate.save(this.MarketingExpenseAllocateEntity())
                .subscribe();
            }),
            finalize(() => this._pageUtilityService.statusChangeSubject.next(1))),
        validate: () => { makeFormTouched(this.form); return this.form.valid }
      })
      .UpdateButton({
        action: () => this._commissionPayableService1.update(this.form.value).pipe(tap(() => {
          this._subsink.sink = this.getMarketingExpenseAllocate().subscribe()
        }),
          finalize(() => this._pageUtilityService.statusChangeSubject.next(3))),
        validate: () => { makeFormTouched(this.form); return this.form.valid }
      })
      .PostButton({
        action: () => this._commissionPayableService1.post(this.form.value).pipe(
          tap(() => {
            this._subsink.sink = this.getMarketingExpenseAllocate()
              .subscribe()
          }),
          finalize(() => this._pageUtilityService.statusChangeSubject.next(8))),
        validate: () => { makeFormTouched(this.form); return this.form.valid; }
      })
      .ReverseButton({
        action: () => this._commissionPayableService1.reverse(this.form.value.SysKey).pipe(
          tap(() => {
            this._subsink.sink = this.getMarketingExpenseAllocate()
              .pipe(
                tap((response: any[]) => {

                  let a = response[0];
                  this._subsink.sink = this._marketingExpenseAllocate.reverse(a.SysKey).subscribe()
                })
              ).subscribe()
          }),
          finalize(() => this._pageUtilityService.statusChangeSubject.next(3))),
        validate: () => { makeFormTouched(this.form); return this.form.valid; }
      })
      .VoidButton({
        action: () => this._commissionPayableService1.void(this.form.value.SysKey).pipe(
          tap(() => {
            this._subsink.sink = this.getMarketingExpenseAllocate()
              .pipe(
                tap((response: any[]) => {
                  let a = response[0];
                  this._subsink.sink = this._marketingExpenseAllocate.void(a.SysKey)
                    .pipe(
                      tap(alo => { })
                    ).subscribe()
                })
              ).subscribe()
          }),
          finalize(() => this._pageUtilityService.statusChangeSubject.next(8))),
        validate: () => { makeFormTouched(this.form); return this.form.valid; }
      })
      .RecapButton({
        action: () =>
          this._recap.open(this._commissionPayableService1.recap(this.form.value.SysKey))
      })
      .ExportToPdf({
        action: () =>
          this._printingService.pdf({
            filter: {
              logic: 'and',
              filters: kendoFilter([{
                field: 'SysKey', value: this.form.value.SysKey, logic: 'and', operator: 'eq'
              }])
            }
          })
      });;
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  public loadData(param: { SysKey?: string }): Promise<FormGroup> {

    return new Promise((resolve, reject) => {

      let subscribe = param.SysKey ? this._commissionPayableService1.findByKey(param.SysKey)
        : this._commissionPayableService1.default();

      this._subsink.sink = subscribe.pipe(
        map(response => {
          response['NetPaid'] = multiCurrency({
            op: response['CURROP'],
            rate: response['CURRATE'],
            value: response['NetPaid'],
          });
          return response;
        }),
        catchError(error => {

          reject(error);
          return error;
        }),
        finalize(() => resolve(this.form)))

        .subscribe(data => {

          const c: any = createJsonFormGroup(data);
          this.form = this.fb.group(c);

          this.form.addControl('CURRCode', new FormControl());
          this.form.addControl('PaymentType', new FormControl());
          this.form.addControl('SalespersonType', new FormControl(0));

          this.form.get('ALOSK').setValue(this.ALOSK);
          this._subsink.sink = this._allocationService.findByKey(this.ALOSK)
            .pipe(
              mergeMap((data: any) => {
                return this._salespersonManagement.findByKey(this.SPSK)
                  .pipe(
                    map(sp => {
                      data['SalespersonType'] = sp['Person']['TransType'];
                      return data;
                    })
                  )
              })
            ).subscribe(data => {
              this.form.get('SalespersonType').patchValue(data['SalespersonType']);
            });

          if (typeof param.SysKey == 'undefined')
            this.setFullAmount('Required to set form value');
          else
            this.setFullAmount('Do not required to set form value');

          this._subsink.sink = this.form.get('PaymentType').valueChanges.subscribe(d => {
            if (d == 2)
              this.setFullAmount('Required to set form value');
            // this.form.get('NetPaid').patchValue(this.CommissionPayable);
          });

          this._subsink.sink = this.form.get('CBSK').valueChanges
            .subscribe(cbsk => {

              this._subsink.sink = this._cashbookService.findByKey(cbsk)
                .subscribe(cash => {
                  this._subsink.sink = this._currencyService.findByKey(cash['CURRSK'])
                    .subscribe(curr => {

                      this.form.get('CURRSK').patchValue(curr['SysKey']);
                      this.form.get('CURRCode').patchValue(curr['Currency_AlphaCode']);

                      this._subsink.sink = this._currencyRateService.findByKey(curr['SysKey']).subscribe(rate => {

                        this.form.get('CURROP').patchValue(rate['Operator']);
                        this.form.get('CURRATE').patchValue(rate['Rate']);
                      });
                    });
                });
            })

          //force change CBSK intially
          if (this.form.get('CBSK').value != null)
            this.form.get('CBSK').patchValue(this.form.get('CBSK').value);

          this.form.get('DatePaid').setValidators([Validators.required])
          this.form.get('CBSK').setValidators([Validators.required]);
        })
    })
  }

  handleNetPaidChange(value: number) {
    // this._transactionDetailService.finds({
    //   filter: {
    //     logic: 'and',
    //     filters: kendoFilter([
    //       { field: 'SPSK', value: this.SPSK, logic: 'and', operator: 'eq' },
    //       { field: 'PTOSK', value: this.InvoiceSK, logic: 'and', operator: 'eq' },
    //     ])
    //   }
    // }).pipe(takeUntil(this._unsubscribe))
    //   .subscribe((response: any[]) => {

    //     const gross = response[0]['GrossComm'];
    //     const expense = response[0]['MarketingExpense'];
    //   })
  }

  private setFullAmount(required: 'Required to set form value' | 'Do not required to set form value') {

    if (required == 'Required to set form value') {

      this._subsink.sink = this._commissionPayableService2
        .getSalespersonBalance(this.ALOSK)
        .subscribe(response => {
          this.form.get('NetPaid').patchValue(response)
        })
    }
  }

  addNewCashbook() {

    let state: WindowState = 'default';
    let window = this._windowService.open({ title: 'Cashbook Setup', componentType: CashbookSetupDetailComponent, state: state });
    let windowComponent = window.content.instance as CashbookSetupDetailComponent;
    windowComponent.loadData({ CashbookName: this._cashbookFilterText });

    this._subsink.sink = windowComponent.OnSuccess
      .subscribe(item => {

        this._cashbookService.asyncFinds();
        this.form.get('CBSK').setValue(item.data.SysKey);
        this._windowService.close();
      });
  }

  private _cashbookFilterText: string;
  onCashbookFilterChange(value: string) {

    this._cashbookFilterText = value;
  }

  private MarketingExpenseAllocateEntity() {
    return {
      DetailSK: this.DetailSK,
      AllocationDate: this.form.get('DatePaid').value,
      SPSK: this.SPSK,
      HeaderSK: this.InvoiceSK,
      TransType: 61,
      GSTPercent: 0,
      GSTValue: 0,
      AllocateAmount: this.form.get('NetPaid').value,
      AmountBeforeGST: this.form.get('NetPaid').value,
    };
  }

  private getMarketingExpenseAllocate() {

    return this._marketingExpenseAllocate.finds({
      filter: {
        filters: kendoFilter([
          { field: 'HeaderSK', value: this.InvoiceSK, operator: 'eq', logic: 'and' },
          { field: 'DetailSK', value: this.DetailSK, operator: 'eq', logic: 'and' },
          { field: 'TransType', value: 61, operator: 'eq', logic: 'and' }
        ]),
        logic: 'and'
      }
    });
  }
}
