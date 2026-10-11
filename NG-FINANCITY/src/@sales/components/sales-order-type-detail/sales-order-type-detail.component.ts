import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { PModalFormComponent } from 'src/@common/components/p-modal-form/p-modal-form.component';
import { catchError, finalize } from 'rxjs/operators';
import { createJsonFormGroup, kendoFilter, makeFormTouched, SubSink } from 'src/@common/service/utility-service';
import { PDatePeriodComponent } from 'src/@common/components/p-date-period/p-date-period.component';
import { AccountSetupService } from 'src/@accounts/services/account-setup.service';
import { AccountSetupDetailComponent } from 'src/@accounts/components/account-setup-detail/account-setup-detail.component';
import { WindowState } from '@progress/kendo-angular-dialog';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { ReferralSalesOrderTypeService } from 'src/@sales/services/referral-sales-order-type.service';

@Component({
  selector: 'app-sales-order-type-detail',
  templateUrl: './sales-order-type-detail.component.html',
  providers: [
    ReferralSalesOrderTypeService,
    AccountSetupService,
    PWindowComponentService]
})
export class SalesOrderTypeDetailComponent extends DetailFormBase implements OnInit, OnDestroy {

  @ViewChild(PModalFormComponent, { static: true }) public _modalForm: PModalFormComponent;
  @ViewChild(PDatePeriodComponent, { static: true }) public _datePeriod: PDatePeriodComponent;

  private _subsink = new SubSink();

  constructor(fb: FormBuilder,
    public _windowService: PWindowComponentService,
    public _accountService: AccountSetupService,
    public _salesOrderTypeService: ReferralSalesOrderTypeService) {

    super(fb);
  }

  ngOnDestroy(): void {

    this._subsink.unsubscribe();
  }

  ngOnInit() {

    this._accountService.asyncFinds({
      filter: {
        logic: 'and',
        filters: kendoFilter([
          { field: 'SysKey', value: '4', logic: 'and', operator: 'startswith' },
          { field: 'AccountType', value: '1', logic: 'and', operator: 'eq' },
        ])
      }
    });

    this._modalForm
      .controls
      .Ready(this.formReady)
      .Success(this.OnSuccess)
      .StateKey(['RecordStatus'])
      .NewButton({
        action: () => this._salesOrderTypeService.default()
      })
      .SaveButton({
        action: () => this._salesOrderTypeService.save(this.form.value),
        validate: () => { makeFormTouched(this.form); return this.form.valid; }
      })
      .UpdateButton({
        action: () => this._salesOrderTypeService.update(this.form.value),
        validate: () => { makeFormTouched(this.form); return this.form.valid; }
      })
      .DeleteButton({
        action: () => this._salesOrderTypeService.delete(this.form.value.SysKey)
      })
  }

  private _accountCodeFilterText: string;
  handelAccountFilterChange(param: { value: string }) {

    this._accountCodeFilterText = param.value;

    this._accountService.asyncFinds({
      filter: {
        logic: 'and',
        filters: kendoFilter([
          { field: 'SysKey', value: '4', logic: 'and', operator: 'startswith' },
          { field: 'AccCode', value: `<# ${param.value}`, logic: 'or', operator: 'contains' },
          { field: 'AccDescription', value: `${param.value} #>`, logic: 'and', operator: 'contains' },
          { field: 'AccountType', value: '1', logic: 'and', operator: 'eq' },
        ])
      }
    });
  }

  loadData(param: { SysKey?: string, Description?: string }): Promise<FormGroup> {

    return new Promise<FormGroup>((resolve, reject) => {

      let subscribe = param.SysKey ? this._salesOrderTypeService.findByKey(param.SysKey) : this._salesOrderTypeService.default();
      this._subsink.sink = subscribe.pipe(
        catchError(error => {

          reject(error);
          return error;
        }),
        finalize(() => resolve(this.form)))
        .subscribe(data => {

          const c: any = createJsonFormGroup(data);
          this.form = this.fb.group(c);

          if (param.Description)
            this.form.get('Description').patchValue(param.Description);

          this.form.get('Description').setValidators([Validators.required]);
          this.form.get('IncomeAccountSK').setValidators([Validators.required]);
        })
    });
  }

  addNewAccount(param: { field: string }) {

    let state: WindowState = 'default';
    let window = this._windowService.open({ title: 'Account Setup', componentType: AccountSetupDetailComponent, state: state });
    let windowComponent = window.content.instance as AccountSetupDetailComponent;
    windowComponent.loadData({ AccDescription: this._accountCodeFilterText });

    this._subsink.sink = windowComponent.OnSuccess
      .subscribe(item => {

        this._accountService.asyncFinds({
          filter: {
            logic: 'and',
            filters: kendoFilter([
              { field: 'SysKey', value: '4', logic: 'and', operator: 'startswith' },
              { field: 'AccountType', value: '1', logic: 'and', operator: 'eq' },
            ])
          }
        });

        this.form.get(param.field).setValue(item.data.SysKey);
        this._windowService.close();
      });
  }

  public itemDisabled(itemArgs: { dataItem: any, index: number }) {

    if (typeof itemArgs.dataItem.IsActive != 'undefined')
      return !itemArgs.dataItem.IsActive;

    return false;
  }
}
