import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { SalutationSource, GenderSource, MartialStatusSource } from 'src/@common/common.constant';
import { NameConventionService } from 'src/app/services/name-conventions.service';
import { FormGroup, Validators, FormBuilder, FormControl } from '@angular/forms';
import { ModalController } from '@ionic/angular';
import { CompanyListModalPage } from '../company-list-modal/company-list-modal.page';
import { VendorService } from 'src/app/services/vendor.service';
import { FormUtilityService } from 'src/app/services/form-utility.service';
import { SubSink } from 'src/@common/utils';

@Component({
  selector: 'fc-external-cobroke',
  templateUrl: './external-cobroke.page.html',
  providers: [VendorService, FormUtilityService]
})
export class ExternalCobrokePage implements OnInit, OnDestroy {

  @Input() form: FormGroup;
  @Input() disabledCompanyOption: boolean = true;

  private _subsink = new SubSink();

  public salutation = [];
  public gender = [];
  public martialStatus = [];

  constructor(private _modalController: ModalController,
    private _formUtilityService: FormUtilityService,
    private _formBuilder: FormBuilder,
    private _nameConventionService: NameConventionService,
    private _vendorService: VendorService) {
  }
  
  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  ngOnInit() {

    this.salutation = SalutationSource;
    this.gender = GenderSource;
    this.martialStatus = MartialStatusSource;

    this.form.addControl('CompanyName', new FormControl());
    //bind company name
    if (this.form.get('VendorSK').value != null) {

      this._subsink.sink = this._vendorService.findByKey(this.form.get('VendorSK').value)
        .subscribe((vendor) => {
          if (vendor)
            this.form.get('CompanyName').patchValue(vendor['Vendor']['CompanyName']);
        })
    }

    this._nameConventionService.register({

      sourceName: this.form.get('Name'),
      fristName: this.form.get('FirstName'),
      lastName: this.form.get('LastName'),
      isLastNameFirst: this.form.get('IsLastNameFirst'),
    })

    this._nameConventionService.watch();

    this.form.get('Name').setValidators([Validators.required]);
    this.form.get('NRIC').setValidators([Validators.required]);
  }

  async handleSelect() {

    const m = await this._modalController.create({
      component: CompanyListModalPage,
      componentProps: {
        form: await this.createForm()
      }
    })

    m.onWillDismiss().then((data: any) => {

      if (data) {

        if (Object.getOwnPropertyNames(data.data).find((f) => f == 'Vendor') &&
          Object.getOwnPropertyNames(data.data).find((f) => f == 'Contact')) {

          this.form.get('CompanyName').patchValue(data.data.Vendor.CompanyName);
          this.form.get('VendorSK').patchValue(data.data.Vendor.SysKey);

        } else {

          this.form.get('CompanyName').patchValue(data.data.CompanyName);
          this.form.get('VendorSK').patchValue(data.data.SysKey);
        }
      }
      this.form.get('CompanyName').markAsTouched();
    })

    return await m.present();
  }

  async createForm() {

    let data = await this._vendorService.default().toPromise();
    let c = this._formUtilityService.formGroupJson(data);

    return this._formBuilder.group(c);
  }
}
