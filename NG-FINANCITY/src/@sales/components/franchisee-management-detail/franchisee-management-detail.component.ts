import { Component, OnInit, ViewChild, OnDestroy, } from '@angular/core';
import { PModalFormComponent } from 'src/@common/components/p-modal-form/p-modal-form.component';
import { FormBuilder, Validators, FormControl, FormGroup } from '@angular/forms';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { VendorService } from 'src/@sales/services/vendor.service';
import { CountryService } from 'src/@sales/services/country.service';
import { Observable } from 'rxjs';
import { catchError, finalize } from 'rxjs/operators';
import { WindowState } from '@progress/kendo-angular-dialog';
import { TaxDetailComponent } from 'src/@accounts/components/tax-detail/tax-detail.component';
import { TaxService } from 'src/@accounts/services/tax.service';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { makeFormTouched, kendoFilter, createJsonFormGroup, SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-franchisee-management-detail',
  templateUrl: './franchisee-management-detail.component.html',
  providers: [
    TaxService,
    VendorService,
    PWindowComponentService,
    CountryService]
})
export class FranchiseeManagementDetailComponent extends DetailFormBase implements OnInit, OnDestroy {

  @ViewChild(PModalFormComponent, { static: true }) public _modalForm: PModalFormComponent;

  private _subsink = new SubSink();

  constructor(fb: FormBuilder,
    public _windowService: PWindowComponentService,
    public _tax: TaxService,
    public _vendorService: VendorService,
    public _country: CountryService) {

    super(fb);
  }

  ngOnInit() {

    this._modalForm
      .controls
      .Success(this.OnSuccess)
      .Ready(this.formReady)
      .StateKey(['Vendor', 'RecordStatus'])
      .NewButton({ action: () => this._vendorService.default() })
      .SaveButton({
        action: () => this._vendorService.save(this.form.value),
        validate: () => { makeFormTouched(this.form); return this.form.valid },
      })
      .UpdateButton({
        action: () => this._vendorService.update(this.form.value),
        validate: () => { makeFormTouched(this.form); return this.form.valid },
      })
      .DeleteButton({
        action: () => this._vendorService.delete(this.form.value.Vendor.SysKey),
      })

    this._country.asyncFinds();
    this._tax.asyncFinds();
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  _key: string;
  _transType: number;

  loadData(param: { SysKey?: string, FranchiseeName?: string, TransType?: number, TaxName?: string }): Promise<FormGroup> {

    this._key = param.SysKey;
    this._transType = param.TransType;

    return new Promise<any>((resolve, reject) => {
      let subscribe: Observable<any>;

      if (param.SysKey)
        subscribe = this._vendorService.findByKey(param.SysKey);
      else
        subscribe = this._vendorService.default();

      this._subsink.sink = subscribe.pipe(
        catchError((e) => {
          reject(e);
          return e;
        }),
        finalize(() => {
          resolve(this.form);
        })
      ).subscribe(data => {

        const c: any = createJsonFormGroup(data);
        this.form = this.fb.group(c);

        (this.form.get('Vendor') as FormGroup).addControl('IsGST', new FormControl());

        if (param.FranchiseeName)
          this.form.get('Vendor').get('CompanyName').patchValue(param.FranchiseeName); //set default 6 for franchisee

        this.form.get('Vendor').get('TransType').setValue(6); //set default 6 for franchisee
        this.form.get('Vendor').get('IsGST').setValue(this.form.get('Vendor').get('TaxSK').value != null);

        if (param.TransType) {
          this.form.get('Vendor').get('TransType').setValue(param.TransType);
        }

        this._subsink.sink = this.form.get('Vendor').get('IsGST').valueChanges
          .subscribe((value: boolean) => {
            if (value == true)
              this.form.get('Vendor').get('TaxSK').setValue('201307051129191477965001');
            else
              this.form.get('Vendor').get('TaxSK').setValue(null);
          });

        this.form.get('Vendor').get('CompanyName').setValidators([Validators.required]);
        this.form.get('Vendor').get('CompanyReg').setValidators([Validators.required]);
      })
    })
  }

  addNewTax() {

    let state: WindowState = 'default';
    let window = this._windowService.open({ title: 'GST Setup', componentType: TaxDetailComponent, state: state });
    let windowComponent = window.content.instance as TaxDetailComponent;
    windowComponent.loadData({});

    this._subsink.sink = windowComponent.OnSuccess
      .subscribe(item => {
        this._tax.asyncFinds({
          filter: {
            filters: kendoFilter([{ field: 'SysKey', value: item.data.SysKey, operator: 'eq', logic: 'and' }]),
            logic: 'and'
          }
        });
        this.form.get('Vendor').get('TaxSK').setValue(item.data.SysKey);
        this._windowService.close();
      });
  }
}
