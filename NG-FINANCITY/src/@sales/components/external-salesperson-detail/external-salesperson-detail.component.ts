import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { PModalFormComponent } from 'src/@common/components/p-modal-form/p-modal-form.component';
import { ExternalSalespersonService } from 'src/@sales/services/external-salesperson.service';
import { VendorService } from 'src/@sales/services/vendor.service';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { WindowState } from '@progress/kendo-angular-dialog';
import { catchError, finalize } from 'rxjs/operators';
import { VendorManagementDetailComponent } from '../vendor-management-detail/vendor-management-detail.component';
import { InternationalNameSpliterService } from 'src/@common/service/internation.name.splitter.service';
import { salutation, martialStatus, gender, makeFormTouched, kendoFilter, createJsonFormGroup, SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-external-salesperson-detail',
  templateUrl: './external-salesperson-detail.component.html',
  providers: [
    VendorService,
    ExternalSalespersonService,
    PWindowComponentService,
    InternationalNameSpliterService
  ]
})
export class ExternalSalespersonDetailComponent extends DetailFormBase implements OnInit, OnDestroy {

  @ViewChild(PModalFormComponent, { static: true }) public _modalForm: PModalFormComponent;

  _salutation = [];
  _martialStatus = [];
  _gender = [];

  private _subsink = new SubSink();

  constructor(fb: FormBuilder,
    public _nameConventionService: InternationalNameSpliterService,
    public _windowService: PWindowComponentService,
    public _externalSalesperson: ExternalSalespersonService,
    public _vendorService: VendorService) {

    super(fb);
  }

  ngOnInit() {

    this._salutation = salutation();
    this._martialStatus = martialStatus();
    this._gender = gender();

    this._modalForm
      .controls
      .Ready(this.formReady)
      .Success(this.OnSuccess)
      .StateKey(['Person', 'RecordStatus'])
      .NewButton({ action: () => this._externalSalesperson.default() })
      .SaveButton({
        action: () => this._externalSalesperson.save(this.form.value),
        validate: () => { makeFormTouched(this.form); return this.form.valid; }
      })
      .UpdateButton({
        action: () => this._externalSalesperson.update(this.form.value),
        validate: () => { makeFormTouched(this.form); return this.form.valid; }
      })
      .DeleteButton({
        action: () => this._externalSalesperson.delete(this.form.value.Person.SysKey),
      })
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  loadData(param: { SysKey?: string, VendorSK?: string, Name?: string }): Promise<FormGroup> {

    return new Promise<FormGroup>((resolve, reject) => {

      let subscribe = param.SysKey ? this._externalSalesperson.findByKey(param.SysKey) : this._externalSalesperson.default();
      this._subsink.sink = subscribe.pipe(
        catchError(error => {

          reject(error)
          return error;
        }),
        finalize(() => {
          resolve(this.form)
        })).subscribe(data => {

          const c: any = createJsonFormGroup(data);
          this.form = this.fb.group(c);

          if (param.VendorSK) {
            this.form.get('Person').get('VendorSK').setValue(param.VendorSK);
          }
          if (param.Name)
            this.form.get('Person').get('Name').patchValue(param.Name)

          this._nameConventionService.register({
            sourceName: this.form.get('Person').get('Name'),
            fristName: this.form.get('Person').get('FirstName'),
            lastName: this.form.get('Person').get('LastName'),
            isLastNameFirst: this.form.get('Person').get('IsLastNameFirst'),
          });

          this._nameConventionService.watch();

          this.form.get('Person').get('VendorSK').setValidators([Validators.required]);
          this.form.get('Person').get('Name').setValidators([Validators.required]);
          this.form.get('Person').get('NRIC').setValidators([Validators.required]);

          if (this.form.get('Person').get('VendorSK').value != null)
            this._vendorService.asyncFinds({
              filter: {
                filters: kendoFilter([{ field: 'SysKey', value: this.form.get('Person').get('VendorSK').value, operator: 'eq', logic: 'and', }]),
                logic: 'and'
              }
            });
        })
    });
  }

  addNewCompany() {

    let state: WindowState = 'default';
    let window = this._windowService.open({ title: 'Vendor Management', componentType: VendorManagementDetailComponent, state: state });
    let windowComponent = window.content.instance as VendorManagementDetailComponent;
    windowComponent.loadData({ CompanyName: this._companyFilterText });

    this._subsink.sink = windowComponent.OnSuccess
      .subscribe(item => {
        this._vendorService.asyncFinds({
          filter: {
            filters: kendoFilter([{ field: 'SysKey', value: item.data.Vendor.SysKey, operator: 'eq', logic: 'and' }]),
            logic: 'and'
          }
        });
        this.form.get('Person').get('VendorSK').setValue(item.data.Vendor.SysKey);
        this._windowService.close();
      });
  }

  private _companyFilterText: string;
  handelVendorFilterChange(param: string) {

    this._companyFilterText = param;
    if (param.length > 2) {

      this._vendorService.asyncFinds({
        filter: {
          filters: kendoFilter([
            { field: 'CompanyName', value: param, operator: 'contains', logic: 'or' },
            { field: 'R1', value: param, operator: 'contains', logic: 'or' },
          ]), logic: 'and'
        }
      })
    }
  }

  public itemDisabled(itemArgs: { dataItem: any, index: number }) {

    if (typeof itemArgs.dataItem.IsActive != 'undefined')
      return !itemArgs.dataItem.IsActive;

    return false;
  }
}
