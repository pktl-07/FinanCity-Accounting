import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { PModalFormComponent } from 'src/@common/components/p-modal-form/p-modal-form.component';
import { FormBuilder, Validators, FormControl, FormGroup } from '@angular/forms';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { VendorService } from 'src/@sales/services/vendor.service';
import { CountryService } from 'src/@sales/services/country.service';
import { DropDownFilterSettings } from '@progress/kendo-angular-dropdowns';
import { catchError, finalize } from 'rxjs/operators';
import { WindowState } from '@progress/kendo-angular-dialog';
import { TaxDetailComponent } from 'src/@accounts/components/tax-detail/tax-detail.component';
import { TaxService } from 'src/@accounts/services/tax.service';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { vendorType, makeFormTouched, kendoFilter, createJsonFormGroup, SubSink } from 'src/@common/service/utility-service';
import { PImageUploaderComponent } from 'src/@common/components/p-image-uploader/p-image-uploader.component';

@Component({
  selector: 'app-vendor-management-detail',
  templateUrl: './vendor-management-detail.component.html',
  providers: [VendorService, CountryService, TaxService]
})
export class VendorManagementDetailComponent extends DetailFormBase implements OnInit, OnDestroy {

  @ViewChild(PModalFormComponent, { static: true }) public _modalForm: PModalFormComponent;
  @ViewChild(PImageUploaderComponent) _imageUploader: PImageUploaderComponent;

  public _vendorType = [];
  private _subsink = new SubSink()

  constructor(fb: FormBuilder,
    public _windowService: PWindowComponentService,
    public _tax: TaxService,
    public _vendorService: VendorService,
    public _country: CountryService) {

    super(fb);
  }

  filterSettings: DropDownFilterSettings = {
    caseSensitive: false,
    operator: 'contains'
  };

  ngOnInit() {

    this._vendorType = vendorType();
    this._country.asyncFinds();
    this._tax.asyncFinds();

    this._modalForm
      .controls
      .Ready(this.formReady)
      .Success(this.OnSuccess)
      .StateKey(['Vendor', 'RecordStatus'])
      .NewButton({
        action: () => this._vendorService.default().pipe(finalize(() => {
          this._imageUploader.clear();
        })),
      })
      .SaveButton({
        action: () => this._vendorService.save(this.form.value),
        validate: () => { makeFormTouched(this.form); return this.form.valid; }
      })
      .UpdateButton({
        action: () => this._vendorService.update(this.form.value),
        validate: () => { makeFormTouched(this.form); return this.form.valid; }
      })
      .DeleteButton({
        action: () => this._vendorService.delete(this.form.value.Vendor.SysKey)
      })
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  loadData(param: { SysKey?: string, TransType?: number, CompanyName?: string }): Promise<FormGroup> {

    return new Promise<FormGroup>((resolve, reject) => {

      let subscribe = param.SysKey ? this._vendorService.findByKey(param.SysKey) : this._vendorService.default();
      this._subsink.sink = subscribe.pipe(
        catchError(error => {

          reject(error);
          return error;
        }),
        finalize(() => {
          resolve(this.form)
        })).subscribe(data => {

          const c = createJsonFormGroup(data);

          this.form = this.fb.group(c);
          (this.form.get('Vendor') as FormGroup).addControl('IsGST', new FormControl());

          if (param.CompanyName)
            this.form.get('Vendor').get('CompanyName').patchValue(param.CompanyName);

          this.form.get('Vendor').get('IsGST').patchValue(this.form.get('Vendor').get('TaxSK').value != null);

          if (param.TransType) {
            this.form.get('Vendor').get('TransType').patchValue(param.TransType);
          }

          this._subsink.sink = this.form.get('Vendor').get('IsGST').valueChanges
            .subscribe((value: boolean) => {

              if (value == true)
                this.form.get('Vendor').get('TaxSK').patchValue('201307051129191477965001');
              else
                this.form.get('Vendor').get('TaxSK').patchValue(null);
            });

          this.form.get('Vendor').get('CompanyReg').setValidators([Validators.required]);
          this.form.get('Vendor').get('CompanyName').setValidators([Validators.required]);
        })
    });
  }

  private _gstFilterText: string;
  onHandleGSTFilterChange(value: string) {
    this._gstFilterText = value;
  }

  addNewTax() {

    let state: WindowState = 'default';
    let window = this._windowService.open({ title: 'GST Setup', componentType: TaxDetailComponent, state: state });
    let windowComponent = window.content.instance as TaxDetailComponent;
    windowComponent.loadData({ TaxCode: this._gstFilterText });

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

  public itemDisabled(itemArgs: { dataItem: any, index: number }) {

    if (typeof itemArgs.dataItem.IsActive != 'undefined')
      return !itemArgs.dataItem.IsActive;

    return false;
  }
}
