import { Component, Input, OnInit, OnDestroy, Inject } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { VendorType } from 'src/@common/common.constant';
import { IFCConfig, SubSink } from 'src/@common/utils';
import { TaxService } from 'src/app/services/tax.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'fc-company',
  templateUrl: './company.page.html',
  providers: [TaxService]
})
export class CompanyPage implements OnInit, OnDestroy {

  @Input() form: FormGroup;

  _vendorType = [];

  private _subsink = new SubSink();

  constructor(private _taxService: TaxService) {
  }

  ngOnInit(): void {
    this._vendorType = VendorType;
    this.form.addControl("HasGST", new FormControl());

    this._subsink.sink = this._taxService.findByKey(this.form.get('TaxSK').value)
      .subscribe((tax) => {
        if (tax) {
          this.form.get('HasGST').patchValue(tax['TaxPercent'] > 0)
        }
      })

    this._subsink.sink = this.form.get('HasGST').valueChanges
      .subscribe((gst) => {
        if (gst == true)
          this.form.get('TaxSK').patchValue(environment.defaultPurchaseTax.taxSK)
        else
          this.form.get('TaxSK').patchValue(environment.defaultPurchaseZeroTax.taxSK)
      })
    this.form.get('CompanyName').setValidators([Validators.required]);
    this.form.get('CompanyReg').setValidators([Validators.required]);
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }
}
