import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { DropDownFilterSettings } from '@progress/kendo-angular-dropdowns';
import { CurrencyRateService } from 'src/@sales/services/currency-rate.service';
import { WindowState } from '@progress/kendo-angular-dialog';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { CurrencyRateDetailComponent } from '../../currency-rate-detail/currency-rate-detail.component';
import { kendoFilter, SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-multi-currency-component',
  templateUrl: './multi-currency-component.html',
  providers: [
    CurrencyRateService,
    PWindowComponentService
  ]
})
export class MultiCurrencyComponent implements OnInit, OnDestroy {

  @Input() currencyCodeFromControl: FormControl;
  @Input() currencyRateFormControl: FormControl;
  @Input() currencyOpFormControl: FormControl;
  @Input() readonly: boolean;

  private _subsink = new SubSink();

  constructor(
    public _windowService: PWindowComponentService,
    public _currencyRateService: CurrencyRateService) {
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  filterSettings: DropDownFilterSettings = {
    caseSensitive: false,
    operator: 'contains'
  };

  ngOnInit() {

    this._currencyRateService.asyncFinds();

    this.currencyCodeFromControl.setValidators(Validators.required);
    this.currencyRateFormControl.setValidators(Validators.required);
    this.currencyRateFormControl.setValidators(Validators.min(0));
  }

  onChangeCurrencyCode(value: string) {

    this._subsink.sink = this._currencyRateService.findByKey(value)
      .subscribe(d => {
        this.currencyRateFormControl.setValue(d.Rate);
        this.currencyOpFormControl.setValidators(d.Operator);
      })
  }

  private _currencyFilterText: string;
  handelCurrencyCodeFilterChange(value: string) {

    this._currencyFilterText = value;
    this._currencyRateService.asyncFinds({
      filter: {
        filters: kendoFilter([
          { field: 'CurrencyCode', value: `<# ${value}`, operator: 'contains', logic: 'or' },
          { field: 'Country', value: `${value} #>`, operator: 'contains', logic: 'or' },
        ]),
        logic: 'or'
      }
    });
  }

  addNewCurrency() {

    let state: WindowState = 'default';
    let window = this._windowService.open({ title: 'Currency Setup', componentType: CurrencyRateDetailComponent, state: state });
    let windowComponent = window.content.instance as CurrencyRateDetailComponent;
    windowComponent.loadData({ CurrencyName: this._currencyFilterText });

    this._subsink.sink = windowComponent.OnSuccess.subscribe((data: any) => {

      this._currencyRateService.asyncFinds();
      this.currencyCodeFromControl.setValue(data.SysKey);
      this.currencyRateFormControl.setValue(data.Rate);
      this._windowService.close();
    });
  }

  public itemDisabled(itemArgs: { dataItem: any, index: number }) {

    if (typeof itemArgs.dataItem.IsActive != 'undefined')
      return !itemArgs.dataItem.IsActive;

    return false;
  }
}
