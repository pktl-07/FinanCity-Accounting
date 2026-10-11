import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { FormBuilder, Validators, FormControl, FormGroup } from '@angular/forms';
import { CountryService } from 'src/@sales/services/country.service';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { MyCompanyService } from 'src/@sales/services/my-company.service';
import { TaxService } from 'src/@accounts/services/tax.service';
import { WindowState } from '@progress/kendo-angular-dialog';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { TaxDetailComponent } from 'src/@accounts/components/tax-detail/tax-detail.component';
import { Router, ActivatedRoute } from '@angular/router';
import { VendorService } from 'src/@sales/services/vendor.service';
import { makeFormTouched, kendoFilter, createJsonFormGroup, SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-my-company-setup',
  templateUrl: './my-company-setup.component.html',
  providers: [
    MyCompanyService,
    VendorService,
    CountryService,
    TaxService,
    PWindowComponentService]
})
export class MyCompanySetupComponent extends DetailFormBase implements OnInit, OnDestroy {

  @ViewChild(PToolbarComponent, { static: true }) _toolbar: PToolbarComponent;

  private _subsink = new SubSink();

  _salutation = [];
  _martialStatus = [];
  _gender = [];
  clientType = "";

  constructor(fb: FormBuilder,
    public _myCompanyservice: MyCompanyService,
    public _windowService: PWindowComponentService,
    public _tax: TaxService,
    public _activatedRoute: ActivatedRoute,
    public _router: Router,
    public _country: CountryService) {

    super(fb);
  }

  ngOnInit() {

    this._country.asyncFinds();
    this._tax.asyncFinds();

    this._toolbar
      .controls
      .Success(this.OnSuccess)
      .Ready(this.formReady)
      .StateKey(['Vendor', 'RecordStatus'])
      .NavigationButton({ action: () => this._router.navigateByUrl('') })
      .UpdateButton({
        action: () => this._myCompanyservice.update(this.form.value),
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

  loadData(data: any) {

    const c: any = createJsonFormGroup(data);
    this.form = this.fb.group(c);

    (this.form.get('Vendor') as FormGroup).addControl('IsGST', new FormControl());

    this.form.get('Vendor').get('IsGST').setValue(this.form.get('Vendor').get('TaxSK').value != null);
    this.form.get('Vendor').get('CompanyName').setValidators(Validators.required);
    this.form.get('Vendor').get('CompanyReg').setValidators(Validators.required);

    this._subsink.sink = this.form.get('Vendor').get('IsGST').valueChanges
      .subscribe((value: boolean) => {

        if (value == true)
          this.form.get('Vendor').get('TaxSK').setValue('201307051139513774165001');
        else
          this.form.get('Vendor').get('TaxSK').setValue(null);
      })
  }
}
