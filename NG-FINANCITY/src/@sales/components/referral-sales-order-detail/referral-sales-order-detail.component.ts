import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { ButtonControlBuilder } from 'src/@common/components/p-toolbar/button-control.builder';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { TaxService } from 'src/@accounts/services/tax.service';
import { finalize } from 'rxjs/operators';
import { ActivatedRoute, Router } from '@angular/router';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { makeFormTouched, createJsonFormGroup, multiCurrency, SubSink } from 'src/@common/service/utility-service';
import { ReferralSalesOrderService } from 'src/@sales/services/referral-sales-order.service';
import { ClientService } from 'src/@sales/services/client.service';
import { PropertyTransactionActivityComponent } from '../property-transaction-detail/property-transaction-activity/property-transaction-activity.component';
import { PageUtilityService } from '../property-transaction-detail/page.utility.service';

@Component({
  selector: 'app-referral-sales-order-detail',
  templateUrl: './referral-sales-order-detail.component.html',
  providers: [
    TaxService,
    ReferralSalesOrderService,
    ClientService,
  ]
})
export class ReferralSalesOrderDetailComponent extends DetailFormBase implements OnInit, OnDestroy {

  @ViewChild(PToolbarComponent, { static: true }) private _toolbar: PToolbarComponent;
  @ViewChild('transactionActivity') private _transactionActivity: PropertyTransactionActivityComponent;

  private _subsink = new SubSink()

  constructor(fb: FormBuilder,
    private _pageUtilityServie: PageUtilityService,
    public _router: Router,
    public _buttonControlBuilder: ButtonControlBuilder,
    public _activatedRoute: ActivatedRoute,
    public _taxService: TaxService,
    public _clientService: ClientService,
    public _referralSaleOrderService: ReferralSalesOrderService) {

    super(fb);

    this._subsink.sink = this.OnSuccess
      .subscribe((item) => {

        if (item.data.PropertyTransaction.RecordStatus == 0) {

          this._transactionActivity.gridResult.next([]);
          this._transactionActivity.payableStatus.next(1);
          this._transactionActivity.receivableStatus.next(1);

        } else {

          const rate = item.data.PropertyTransaction.CURRATE;
          const op = item.data.PropertyTransaction.CURROP;

          item.data.PropertyTransaction

          item.data['TNC']['LoanAmount'] = multiCurrency({
            op: op,
            rate: rate,
            value: item.data['TNC']['LoanAmount']
          });

          item.data['PropertyTransaction']['TransactedPrice'] = multiCurrency({
            op: op,
            rate: rate,
            value: item.data['PropertyTransaction']['TransactedPrice']
          });

          item.data['PropertyTransaction']['TaxAmount'] = multiCurrency({
            op: op,
            rate: rate,
            value: item.data['PropertyTransaction']['TaxAmount']
          });

          item.data['PropertyTransaction']['TotalAmount'] = multiCurrency({
            op: op,
            rate: rate,
            value: item.data['PropertyTransaction']['TotalAmount']
          });

          item.data['PropertyTransaction']['GrossCommission'] = multiCurrency({
            op: op,
            rate: rate,
            value: item.data['PropertyTransaction']['GrossCommission']
          });
        }

        this._subsink.sink = this._taxService.findByKey(item.data.PropertyTransaction.TaxSK)
          .subscribe(tax => {
            if (tax != null) {
              this._pageUtilityServie.taxSubject.next({
                Key: tax.SysKey,
                Value: tax.TaxCode,
                TaxPercent: item.data.PropertyTransaction.TaxPercent
              });
            }
          });
      })
  }

  ngOnInit() {

    this._toolbar
      .controls
      .Success(this.OnSuccess)
      .Ready(this.formReady)
      .StateKey(['PropertyTransaction', 'RecordStatus'])
      .NavigationButton({ action: () => this._router.navigateByUrl('/Business/SalesTransaction/ReferralSalesTransaction') })
      .NewButton({
        action: () =>
          this._referralSaleOrderService.default().pipe(
            finalize(() => {

              this.form.get('Client').get('ClientNRIC').patchValue(null);
              this.form.get('Client').get('ClientAddress').patchValue(null);

              this.form.get('PropertyTransaction').get('VendorSK').patchValue(null);
              this.form.get('PropertyTransaction').get('BankerSK').patchValue(null);
            })
          )
      })
      .SaveButton({
        action: () =>
          this._referralSaleOrderService.save(this.form.value),
        validate: () => { makeFormTouched(this.form); return this.form.valid }
      })
      .UpdateButton({
        action: () =>
          this._referralSaleOrderService.update(this.form.value),
        validate: () => { makeFormTouched(this.form); return this.form.valid }
      })
      .PostButton({
        action: () =>
          this._referralSaleOrderService.post(this.form.value),
        validate: () => { makeFormTouched(this.form); return this.form.valid }
      })
      .ReverseButton({
        action: () =>
          this._referralSaleOrderService.reverse(this.form.get('PropertyTransaction').get('SysKey').value),
        validate: () => { makeFormTouched(this.form); return this.form.valid }
      })
      .VoidButton({
        action: () =>
          this._referralSaleOrderService.void(this.form.get('PropertyTransaction').get('SysKey').value),
        validate: () => { makeFormTouched(this.form); return this.form.valid }
      });

    this._subsink.sink = this._activatedRoute.data
      .subscribe((data) => {

        this.loadData(data['data']);
      });
  }

  ngOnDestroy(): void {

    this._subsink.unsubscribe();
  }

  loadData(data: any): Promise<FormGroup> {

    return new Promise<FormGroup>((resolve, reject) => {

      try {

        const c: any = createJsonFormGroup(data);
        this.form = this.fb.group(c);

        this.form.get('PropertyTransaction').get('TransactionDate').setValidators([Validators.required]);
        this.form.get('PropertyTransaction').get('CommissionRate').setValidators([Validators.required]);
        this.form.get('PropertyTransaction').get('SaleOrderTypeSK').setValidators([Validators.required]);

        this.form.get('Internal').get('SPSK').setValidators([Validators.required]);
        this.form.get('Internal').get('CurrentCommission').setValidators([Validators.required, Validators.min(0.001), Validators.max(100)]);
        this.form.get('Client').get('ClientSK').setValidators([Validators.required]);

        this._subsink.sink = this.form.get('PropertyTransaction').get('TransactedPrice').valueChanges
          .subscribe(data => {
            this.form.get('TNC').get('LoanAmount').patchValue(data, { emitEvent: false });
          })

        resolve(null);
      } catch (error) {
        reject(error)
      }
    })
  }
}
