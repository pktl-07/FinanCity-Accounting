import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { ButtonControlBuilder } from 'src/@common/components/p-toolbar/button-control.builder';
import { FormBuilder, Validators } from '@angular/forms';
import { PropertyTransactionService } from 'src/@sales/services/property-transaction.service';
import { TaxService } from 'src/@accounts/services/tax.service';
import { finalize } from 'rxjs/operators';
import { ActivatedRoute, Router } from '@angular/router';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { PropertyTransactionClientsComponent } from './property-transaction-clients/property-transaction-clients.component';
import { PropertyTransactionExternalComponent } from './property-transaction-externals/property-transaction-externals.component';
import { PropertyTransactionInternalComponent } from './property-transaction-internals/property-transaction-internals.component';
import { makeFormTouched, createJsonFormGroup, SubSink, multiCurrency } from 'src/@common/service/utility-service';
import { PageUtilityService } from './page.utility.service';
import { PropertyTransactionActivityComponent } from './property-transaction-activity/property-transaction-activity.component';
import { PropertyTransactionTNCComponent } from './property-transaction-tnc/property-transaction-tnc.component';

@Component({
  selector: 'app-property-transaction-detail',
  templateUrl: './property-transaction-detail.component.html',
  providers: [
    TaxService,
    PropertyTransactionService,
  ]
})
export class PropertyTransactionDetailComponent extends DetailFormBase implements OnInit, OnDestroy {

  @ViewChild('clientComponent') clientComponent: PropertyTransactionClientsComponent;
  @ViewChild('internalComponent') internalComponent: PropertyTransactionInternalComponent;
  @ViewChild('externalComponent') externalComponent: PropertyTransactionExternalComponent;
  @ViewChild('tnc') tncComponent: PropertyTransactionTNCComponent;
  @ViewChild(PToolbarComponent, { static: true }) private _toolbar: PToolbarComponent;
  @ViewChild('transactionActivity') private _transactionActivity: PropertyTransactionActivityComponent;

  clients = [];
  internals = [];
  externals = [];

  private _subsink = new SubSink();

  constructor(private _pageUtilityServie: PageUtilityService,
    fb: FormBuilder,
    public _router: Router,
    public _buttonControlBuilder: ButtonControlBuilder,
    public _activatedRoute: ActivatedRoute,
    public _taxService: TaxService,
    public _propertyTransactionService: PropertyTransactionService) {

    super(fb);

    this._subsink.sink = this.OnSuccess
      .subscribe((item) => {

        this._pageUtilityServie.externalSubject.next(item.data.Externals);
        this._pageUtilityServie.clientSubject.next(item.data.Clients);
        this._pageUtilityServie.internalSubject.next(item.data.Internals);
        this._pageUtilityServie.promotionItemExpenseSubject.next(item.data.PromotionExpense);

        if (item.data.PropertyTransaction.RecordStatus == 0) {

          this._transactionActivity.gridResult.next([]);
          this._transactionActivity.payableStatus.next(1);
          this._transactionActivity.receivableStatus.next(1);
        }
        else {

          const rate = item.data.PropertyTransaction.CURRATE;
          const op = item.data.PropertyTransaction.CURROP;

          this._transactionActivity.refresh(item.data['PropertyTransaction']['SysKey']);

          item.data['TNC']['LoanAmount'] = multiCurrency({
            op: op,
            rate: rate,
            value: item.data['TNC']['LoanAmount']
          })

          item.data['PropertyTransaction']['TransactedPrice'] = multiCurrency({
            op: op,
            rate: rate,
            value: item.data['PropertyTransaction']['TransactedPrice']
          })

          item.data['PropertyTransaction']['TaxAmount'] = multiCurrency({
            op: op,
            rate: rate,
            value: item.data['PropertyTransaction']['TaxAmount']
          })

          item.data['PropertyTransaction']['TotalAmount'] = multiCurrency({
            op: op,
            rate: rate,
            value: item.data['PropertyTransaction']['TotalAmount']
          })

          item.data['PropertyTransaction']['GrossCommission'] = multiCurrency({
            op: op,
            rate: rate,
            value: item.data['PropertyTransaction']['GrossCommission']
          })
        }
      })
  }

  ngOnInit() {

    this._toolbar
      .controls
      .Success(this.OnSuccess)
      .Ready(this.formReady)
      .StateKey(['PropertyTransaction', 'RecordStatus'])
      .NavigationButton({ action: () => this._router.navigateByUrl('/Business/SalesTransaction/SalesOrderTransaction') })
      .NewButton({
        action: () =>
          this._propertyTransactionService.default().pipe(
            finalize(() => {
              this.form.get('PropertyTransaction').get('VendorSK').patchValue(null);
              this.form.get('PropertyTransaction').get('BankerSK').patchValue(null);
            }))
      })
      .ApproveButton({
        action: () => this._propertyTransactionService.approve(this.bindFormValue()),
        validate: () => { makeFormTouched(this.form); return this.form.valid }
      })
      .RejectButton({
        action: () => this._propertyTransactionService.reject(this.bindFormValue()),
        validate: () => { makeFormTouched(this.form); return this.form.valid }
      })
      .SaveButton({
        action: () => this._propertyTransactionService.save(this.bindFormValue()),
        validate: () => { makeFormTouched(this.form); return this.form.valid }
      })
      .UpdateButton({
        action: () => this._propertyTransactionService.update(this.bindFormValue()),
        validate: () => { makeFormTouched(this.form); return this.form.valid }
      })
      .PostButton({
        action: () => this._propertyTransactionService.post(this.bindFormValue()),
        validate: () => { makeFormTouched(this.form); return this.form.valid }
      })
      .ReverseButton({
        action: () => this._propertyTransactionService.reverse(this.form.get('PropertyTransaction').get('SysKey').value),
        validate: () => { makeFormTouched(this.form); return this.form.valid }
      })
      .VoidButton({
        action: () =>
          this._propertyTransactionService.void(this.form.get('PropertyTransaction').get('SysKey').value),
        validate: () => { makeFormTouched(this.form); return this.form.valid }
      });

    this._subsink.sink = this._activatedRoute.data
      .subscribe((data) => {
        this.loadData(data['data']).then(() => {
          this._pageUtilityServie.commissionSource.next(data['data']['CommissionSource']);
        })
      });
  }

  ngOnDestroy(): void {

    this._subsink.unsubscribe();
  }

  onTaxChange() {

    this.changeSalesTax();
  }

  public changeTax(dataItem: {
    TaxSK: string, TaxCode: string,
    TaxPercent: number,
    GrossComm: number,
    GSTValue: number,
    TotalPayable: number
  }) {

    dataItem['TaxSK'] = this.form.get('PropertyTransaction').get('TaxSK').value;
    dataItem['TaxPercent'] = this.form.get('PropertyTransaction').get('TaxPercent').value;
    dataItem['GSTValue'] = (dataItem['GrossComm'] / 100) * dataItem['TaxPercent'];
    dataItem['TotalPayable'] = dataItem['GrossComm'] + dataItem['GSTValue'];
  }

  private changeSalesTax() {
    this.internalComponent.dataGrid.service.data.map((o) => {
      this.changeTax(o);
    });
    this.internalComponent.dataGrid.service.createdItems.map((o) => {
      this.changeTax(o);
    });
    this.internalComponent.dataGrid.service.updatedItems.map((o) => {
      this.changeTax(o);
    });
  }

  loadData(data: any) {

    return new Promise((resolve, reject) => {

      try {

        const c: any = createJsonFormGroup(data);
        this.form = this.fb.group(c);

        this._subsink.sink = this._taxService.findByKey(data.PropertyTransaction.TaxSK)
          .subscribe(tax => {
            if (tax)
              this._pageUtilityServie.taxSubject.next({
                Key: tax.SysKey,
                Value: tax.TaxCode,
                TaxPercent: data.PropertyTransaction.TaxPercent
              }); //trigger default tax code for internal grid.
          });

        this._pageUtilityServie.externalSubject.next(this.form.get('Externals').value);
        this._pageUtilityServie.clientSubject.next(this.form.get('Clients').value);
        this._pageUtilityServie.internalSubject.next(this.form.get('Internals').value);
        this._pageUtilityServie.promotionItemExpenseSubject.next(this.form.get('PromotionExpense').value);

        this.form.get('PropertyTransaction').get('TransactionDate').setValidators([Validators.required]);
        this.form.get('PropertyTransaction').get('CommissionRate').setValidators([Validators.required]);
        this.form.get('PropertyTransaction').get('TransactedPrice').setValidators([Validators.required, Validators.min(1)]);

        this._subsink.sink = this.form.get('TNC').get('LoanAmount').valueChanges
          .subscribe(data => {
            this.form.get('PropertyTransaction').get('TransactedPrice').patchValue(data);
          })
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

  public bindFormValue(): any {

    return {

      Property: this.form.get('PropertyTransaction').value,
      Contact: this.form.get('Contact').value,
      TNC: this.form.get('TNC').value,

      PromotionItemExpenseInsertedItems: this.tncComponent.expenseComponent.dataGrid.service.data.filter(x => x['SysKey'] == null),
      PromotionItemExpenseUpdatedItems: this.tncComponent.expenseComponent.dataGrid.service.data.filter(x => x['SysKey'] != null),
      PromotionItemExpenseDeletedITems: this.tncComponent.expenseComponent.dataGrid.service.deletedItems,

      ClientInsertedItems: this.clientComponent.dataGrid.service.data.filter(x => x['SysKey'] == null),
      ClientUpdatedItems: this.clientComponent.dataGrid.service.data.filter(x => x['SysKey'] != null),
      ClientDeletedItems: this.clientComponent.dataGrid.service.deletedItems,

      InternalInsertedItems: this.internalComponent.dataGrid.service.data.filter(x => x['SysKey'] == null),
      InternalUpdatedItems: this.internalComponent.dataGrid.service.data.filter(x => x['SysKey'] != null),
      InternalDeletedItems: this.internalComponent.dataGrid.service.deletedItems,

      ExternalInsertedItems: this.externalComponent.dataGrid.service.data.filter(x => x['SysKey'] == null),
      ExternalUpdatedItems: this.externalComponent.dataGrid.service.data.filter(x => x['SysKey'] != null),
      ExternalDeletedItems: this.externalComponent.dataGrid.service.deletedItems,
    }
  }
}
