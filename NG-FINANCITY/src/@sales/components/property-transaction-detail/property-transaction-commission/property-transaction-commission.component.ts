import { Component, OnInit, Input, Output, EventEmitter, OnDestroy } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { TaxService } from 'src/@accounts/services/tax.service';
import { DropDownFilterSettings } from '@progress/kendo-angular-dropdowns';
import { WindowState } from '@progress/kendo-angular-dialog';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { TaxDetailComponent } from 'src/@accounts/components/tax-detail/tax-detail.component';
import { getComissionContext, kendoFilter, SubSink } from 'src/@common/service/utility-service';
import { PageUtilityService } from '../page.utility.service';

@Component({
  selector: 'app-property-transaction-commission',
  templateUrl: './property-transaction-commission.component.html',
  providers: [
    TaxService,
    PWindowComponentService
  ]
})
export class PropertyTransactionCommissionComponent implements OnDestroy, OnInit {

  @Output() taxChange: EventEmitter<number> = new EventEmitter<number>();
  @Input() formGroup: FormGroup;

  private _subsink = new SubSink();

  get readonly() {
    return this.formGroup.get('RecordStatus').value >= 6;
  }

  constructor(
    private _pageUtilityService: PageUtilityService,
    public _taxService: TaxService,
    public _windowService: PWindowComponentService) {

    this._taxService.asyncFinds();
  }

  ngOnInit(): void {

    this._subsink.sink = this.formGroup.get('TaxSK').valueChanges
      .subscribe((key: string) => {

        this._subsink.sink = this._taxService.findByKey(key)
          .subscribe(tax => {

            if (tax != null) {

              let taxPercent = tax['TaxPercent'];
              this.formGroup.get('TaxPercent').setValue(taxPercent);
              let gross = this.formGroup.get('GrossCommission').value;

              let context = getComissionContext({ tax: taxPercent, amount: gross, call: 'gross' });
              this.formGroup.get('TaxAmount').setValue(context.taxAmount);
              this.formGroup.get('TotalAmount').setValue(context.total);

              this._pageUtilityService.taxSubject.next({
                Key: tax['SysKey'],
                Value: tax['TaxCode'],
                TaxPercent: tax['TaxPercent']
              })

              this.taxChange.next(taxPercent);
            } else {

              this.formGroup.get('TaxPercent').setValue(0);
              let gross = this.formGroup.get('GrossCommission').value;

              let context = getComissionContext({ tax: 0, amount: gross, call: 'gross' });
              this.formGroup.get('TaxAmount').setValue(context.taxAmount);
              this.formGroup.get('TotalAmount').setValue(context.total);

              this._pageUtilityService.taxSubject.next({
                Key: null,
                Value: 'N/A',
                TaxPercent: 0
              })

              this.taxChange.next(0);
            }
          })
      })

    this._subsink.sink = this.formGroup.get('TransactedPrice').valueChanges.subscribe(data => {

      this.onTransactionPriceChange(data);
    })
  }

  ngOnDestroy(): void {

    this._subsink.unsubscribe();
  }

  public itemDisabled(itemArgs: { dataItem: any, index: number }) {

    if (typeof itemArgs.dataItem.IsActive != 'undefined')
      return !itemArgs.dataItem.IsActive;

    return false;
  }

  private _taxFilterText: string;
  handelTaxAccountFilterChange(value: string) {

    this._taxFilterText = value;
    if (value.length >= 3) {
      this._taxService.asyncFinds({
        filter: {
          logic: 'or',
          filters: kendoFilter(this.taxDropDownSetting(value))
        }
      })
    }
  }

  taxDropDownSetting(value: string) {

    let filterInfo = [];

    filterInfo.push({ field: 'TaxCode', value: value, operator: 'contains', logic: 'or' });
    filterInfo.push({ field: 'Description', value: value, operator: 'contains', logic: 'or' });

    return filterInfo;
  }

  onTransactionPriceChange(value: number) {

    let price = value;
    let rate = this.formGroup.get('CommissionRate').value;
    let tax = this.formGroup.get('TaxPercent').value;
    let gross = (price / 100) * rate;

    let context = getComissionContext({ tax: tax, amount: gross, call: 'gross' })

    this.formGroup.get('GrossCommission').setValue(gross);
    this.formGroup.get('TaxAmount').setValue(context.taxAmount);
    this.formGroup.get('TotalAmount').setValue(context.total);
  }

  onChangeCommissionRate() {

    let rate = this.formGroup.get('CommissionRate').value;
    let price = this.formGroup.get('TransactedPrice').value;
    let tax = this.formGroup.get('TaxPercent').value;
    let gross = (price / 100) * rate;
    let context = getComissionContext({ tax: tax, amount: gross, call: 'gross' })

    this.formGroup.get('GrossCommission').setValue(gross);
    this.formGroup.get('TaxAmount').setValue(context.taxAmount);
    this.formGroup.get('TotalAmount').setValue(context.total);
  }

  onChangeGrossCommission() {

    let rate = this.formGroup.get('CommissionRate').value;
    let price = this.formGroup.get('TransactedPrice').value;
    let tax = this.formGroup.get('TaxPercent').value;
    let gross = this.formGroup.get('GrossCommission').value;
    let context = getComissionContext({ tax: tax, amount: gross, call: 'gross' })

    this.formGroup.get('CommissionRate').setValue((gross / price) * 100);
    this.formGroup.get('TaxAmount').setValue(context.taxAmount);
    this.formGroup.get('TotalAmount').setValue(context.total);
  }

  onChangeTotalCommission() {

    let rate = this.formGroup.get('CommissionRate').value;
    let price = this.formGroup.get('TransactedPrice').value;
    let tax = this.formGroup.get('TaxPercent').value;
    let total = this.formGroup.get('TotalAmount').value;
    let context = getComissionContext({ tax: tax, amount: total, call: 'total' })

    this.formGroup.get('CommissionRate').setValue((context.gross / price) * 100);
    this.formGroup.get('TaxAmount').setValue(context.taxAmount);
    this.formGroup.get('GrossCommission').setValue(context.gross);
  }

  addNewTax() {

    let state: WindowState = 'default';
    let window = this._windowService.open({ title: 'GST Setup', componentType: TaxDetailComponent, state: state });
    let windowComponent = window.content.instance as TaxDetailComponent;
    
    windowComponent.loadData({ TaxCode: this._taxFilterText });

    this._subsink.sink = windowComponent.OnSuccess
      .subscribe(item => {
        this._taxService.asyncFinds();

        this.formGroup.get('TaxPercent').setValue(item.data.SysKey);
        this._windowService.close();
      })
  }

  filterSettings: DropDownFilterSettings = {
    caseSensitive: false,
    operator: 'contains'
  };
}
