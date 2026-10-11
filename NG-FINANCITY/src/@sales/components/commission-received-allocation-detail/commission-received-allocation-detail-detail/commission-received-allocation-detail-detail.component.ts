import { Component, OnInit, ViewChild, Input, OnDestroy } from '@angular/core';
import { FormBuilder, FormControl, Validators, FormGroup } from '@angular/forms';
import { tap, catchError, finalize, map } from 'rxjs/operators';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { PModalFormComponent } from 'src/@common/components/p-modal-form/p-modal-form.component';
import { ActivatedRoute } from '@angular/router';
import { SalespersonManagementService } from 'src/@sales/services/salesperson-management.service';
import { CommissionReceivedAllocationService } from 'src/@sales/services/commission-received-allocation.service';
import { PropertyTransactionService } from 'src/@sales/services/property-transaction.service';
import { makeFormTouched, getComissionContext, kendoFilter, createJsonFormGroup, SubSink } from 'src/@common/service/utility-service';
import { PageUtilityService } from '../../commission-received-detail/page.utility.service';
import { RecapDailogComponent } from 'src/@common/components/p-recap-dialog/p-recap-dialog.component';

@Component({
  selector: 'app-commission-received-allocation-detail-detail.component',
  templateUrl: './commission-received-allocation-detail-detail.component.html',
  providers: [
    SalespersonManagementService,
    CommissionReceivedAllocationService,
    PropertyTransactionService,
    PWindowComponentService
  ]
})
export class CommissionReceivedAllocationDetailDetailComponent extends DetailFormBase implements OnInit, OnDestroy {

  @ViewChild('recapDailog', { static: true }) private _recap: RecapDailogComponent;
  @ViewChild(PModalFormComponent, { static: true }) public _modalForm: PModalFormComponent;

  @Input('InvoiceSK') public invoiceSK: string;
  @Input('CommissionBalance') public commissionBalance: number;
  @Input('PercentComplete') public percentComplete = 0;

  private _subsink = new SubSink();

  get readonly() {
    return this.form.get('RecordStatus').value >= 6;
  }

  constructor(fb: FormBuilder,
    private _pageUtilityService: PageUtilityService,
    public _activatedRoute: ActivatedRoute,
    public _propertyTransactionService: PropertyTransactionService,
    public _allocationService: CommissionReceivedAllocationService,
    public _salespersonManagementService: SalespersonManagementService,
    public _windowService: PWindowComponentService) {

    super(fb);

    this._salespersonManagementService.next([]);
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  ngOnInit(): void {

    this._modalForm
      .controls
      .Ready(this.formReady)
      .Success(this.OnSuccess)
      .StateKey(['RecordStatus'])
      .NewButton({
        action: () => this._allocationService.default()
      })
      .SaveButton({
        action: () => this._allocationService.save(this.form.value),
        validate: () => { makeFormTouched(this.form); return this.form.valid; }
      })
      .UpdateButton({
        action: () => this._allocationService.update(this.form.value),
        validate: () => { makeFormTouched(this.form); return this.form.valid }
      })
      .PostButton({
        action: () => this._allocationService.post(this.form.value).pipe(
          tap(() => this._pageUtilityService.receivedBalanceChangeSubject.next())
        ),
        validate: () => { makeFormTouched(this.form); return this.form.valid; }
      })
      .ReverseButton({
        action: () => this._allocationService.reverse(this.form.value.SysKey).pipe(
          tap(() => this._pageUtilityService.receivedBalanceChangeSubject.next())
        ),
        validate: () => { makeFormTouched(this.form); return this.form.valid }
      })
      .VoidButton({
        action: () => this._allocationService.void(this.form.value.SysKey).pipe(
          tap(() => this._pageUtilityService.receivedBalanceChangeSubject.next())),
        validate: () => { makeFormTouched(this.form); return this.form.valid; }
      })

    this.loadSalespersonList();
  }

  public loadData(param?: { SysKey?: string }): Promise<FormGroup> {

    return new Promise((resolve, reject) => {

      let subscribe = param.SysKey ? this._allocationService.findByKey(param.SysKey) : this._allocationService.default();

      this._subsink.sink = subscribe.pipe(
        catchError(error => {
          reject(error);
          return error;
        }), finalize(() => {
          resolve(this.form)
        })).subscribe(data => {

          const c: any = createJsonFormGroup(data);
          this._modalForm.changeState(data.RecordStatus);
          this.form = this.fb.group(c);
          this.form.addControl('SharePercent', new FormControl(this.percentComplete));
          this.form.addControl('TotalReceivable', new FormControl(0));
          this.form.addControl('TotalReceived', new FormControl(0));
          this.form.get('HeaderSK').setValue(this.invoiceSK);
          this.form.get('AllocationDate').setValidators([Validators.required]);
          this.form.get('SPSK').setValidators([Validators.required]);
          this.form.get('SharePercent').setValidators(Validators.max(100));

          this.setTotalReceivable(data.SPSK).then(() => {

          })
        })
    });
  }

  private setTotalReceivable(spsk: string): Promise<any> {

    return new Promise<any>((resolve, reject) => {

      if (spsk != null) {

        this._subsink.sink = this._allocationService.SalespersonTotalReceivable(spsk, this.invoiceSK)
          .pipe(
            catchError(err => {
              reject(err);
              return err;
            }),
            map((receivable: number) => {
              return receivable;
            }),
            tap(receivable => this.form.get('TotalReceivable').patchValue(receivable)),
          )
          .subscribe(() => resolve(null))
      } else
        reject();
    })
  }

  public onSalespersonChange(spsk: string) {

    this.form.get('SharePercent').setValue(0);
    this.form.get('AmountBeforeGST').setValue(0);
    this.form.get('GSTPercent').setValue(0);
    this.form.get('GSTValue').setValue(0);
    this.form.get('AllocateAmount').setValue(0);
    this.form.get('HeaderSK').setValue(this.invoiceSK);

    this._subsink.sink = this._propertyTransactionService.findByKey(this.invoiceSK)
      .subscribe(data => {

        let s = (data['Internals'] as []).concat(data['Externals'] as []);
        let detail = s.find(f => f['SPSK'] == spsk);

        this.setTotalReceivable(spsk).then(() => {

          this.form.get('GSTPercent').setValue(detail['TaxPercent']);
          this.form.get('DetailSK').setValue(detail['SysKey']);

          let tax = this.form.get('GSTPercent').value;
          let amt = this.form.get('AllocateAmount').value;

          let context = getComissionContext({ tax: tax, amount: amt, call: 'gross' })
          this.form.get('GSTValue').setValue(context.taxAmount);
          this.form.get('AllocateAmount').setValue(context.total);
        })
      })
  }

  public onSharePercentChange(rate: number) {

    let tax = this.form.get('GSTPercent').value;
    //let balance = this.form.get('TotalReceivable').value - this.form.get('TotalReceived').value;
    let total = (this.commissionBalance / 100) * rate;

    let context = getComissionContext({ tax: tax, amount: total, call: 'total' })
    this.form.get('AmountBeforeGST').setValue(context.gross);
    this.form.get('GSTValue').setValue(context.taxAmount);
    this.form.get('AllocateAmount').setValue(context.total);
  }

  onAmountBeforeGSTChange(value: number) {

    let tax = this.form.get('GSTPercent').value;
    let context = getComissionContext({ tax: tax, amount: value, call: 'gross' })
    //let balance = this.form.get('TotalReceivable').value - this.form.get('TotalReceived').value;
    this.form.get('SharePercent').setValue((context.total / this.commissionBalance) * 100);
    this.form.get('GSTValue').setValue(context.taxAmount);
    this.form.get('AllocateAmount').setValue(context.total);
  }

  onAllocateAmountChange(value: number) {

    let tax = this.form.get('GSTPercent').value;
    let context = getComissionContext({ tax: tax, amount: value, call: 'total' })

    // let balance = this.form.get('TotalReceivable').value - this.form.get('TotalReceived').value;
    this.form.get('SharePercent').setValue((context.total / this.commissionBalance) * 100);
    this.form.get('GSTValue').setValue(context.taxAmount);
    this.form.get('AmountBeforeGST').setValue(context.gross);
  }

  public loadSalespersonList() {

    this._subsink.sink = this._propertyTransactionService.findByKey(this.invoiceSK)
      .subscribe(data => {

        let s = (data['Internals'] as []).concat(data['Externals'] as []);
        let filterInfo = [];

        s.forEach(value => {
          filterInfo.push({ field: 'SysKey', value: value['SPSK'], operator: 'eq', logic: 'or' });
        });

        this._salespersonManagementService.asyncEveryBody({
          filter: {
            logic: 'or',
            filters: kendoFilter(filterInfo)
          }
        })
      })
  }
}
