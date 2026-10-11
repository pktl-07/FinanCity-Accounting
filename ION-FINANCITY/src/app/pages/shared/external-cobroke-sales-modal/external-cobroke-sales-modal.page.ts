import { Component, OnInit, Input, OnDestroy, Inject } from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';
import { ModalController } from '@ionic/angular';
import { FormUtilityService } from 'src/app/services/form-utility.service';
import { getComissionContext, IFCConfig, SubSink } from 'src/@common/utils';
import { ExtenralCobrokeListModalPage } from '../external-cobroke-list-modal/external-cobroke-list-modal.page';
import { VendorService } from 'src/app/services/vendor.service';
import { TaxService } from 'src/app/services/tax.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'fc-external-cobroke-sales-modal',
  templateUrl: 'external-cobroke-sales-modal.page.html',
  providers: [FormUtilityService,
    VendorService,
    TaxService]
})
export class ExternalSalesSalesModalPage implements OnInit, OnDestroy {

  @Input() form: FormGroup;
  @Input() transactionAmout: number = 0;
  @Input() disabledCompanyOption = false;

  private _subsink = new SubSink();

  constructor(private _modalController: ModalController,
    private _taxService: TaxService,
    private _vendorService: VendorService,
    private _formUtilityService: FormUtilityService) {
  }

  ngOnInit() {

    this._subsink.sink = this.form.get('HasGST').valueChanges
      .subscribe((value: boolean) => {

        if (value) {

          this.form.get('TaxSK').patchValue(environment.defaultPurchaseTax.taxSK);
          this.form.get('TaxPercent').patchValue(environment.defaultPurchaseTax.taxPercent);

          let gross = this.form.controls['GrossComm'].value;
          let context = getComissionContext({ tax: environment.defaultPurchaseTax.taxPercent, amount: gross, call: 'gross' })

          this.form.controls['GSTValue'].patchValue(context.taxAmount);
          this.form.controls['TotalPayable'].patchValue(context.total);

        } else {

          this.form.get('TaxSK').patchValue(environment.defaultPurchaseZeroTax.taxSK);
          this.form.get('TaxPercent').patchValue(environment.defaultPurchaseZeroTax.taxPercent);

          let gross = this.form.controls['GrossComm'].value;
          let context = getComissionContext({ tax: environment.defaultPurchaseZeroTax.taxPercent, amount: gross, call: 'gross' })

          this.form.controls['GSTValue'].patchValue(context.taxAmount);
          this.form.controls['TotalPayable'].patchValue(context.total);
        }
      })

    this.form.get('SalespersonName').setValidators([Validators.required]);
    this.form.get('SharePercent').setValidators([Validators.required, Validators.min(0), Validators.max(100)]);
    this.form.get('GrossComm').setValidators([Validators.required, Validators.min(0)]);
    this.form.get('TotalPayable').setValidators([Validators.required, Validators.min(0)]);
  }

  ngOnDestroy(): void {

    this._subsink.unsubscribe();
  }

  async handleSelect() {

    const m = await this._modalController.create({
      component: ExtenralCobrokeListModalPage,
      componentProps: {
        disabledCompanyOption: this.disabledCompanyOption
      }
    })

    m.onWillDismiss().then((item: any) => {
      if (item.data) {

        this.form.get('SalespersonName').patchValue(item.data.Name);
        this.form.get('SPSK').patchValue(item.data.SysKey);
        this.form.get('VendorSK').patchValue(item.data.VendorSK);

        this._subsink.sink = this._vendorService.findByKey(item.data.VendorSK)
          .subscribe((vendor) => {

            if (vendor.Vendor['TaxSK'] != null) {
              this._subsink.sink = this._taxService.findByKey(vendor.Vendor['TaxSK'])
                .subscribe((tax) => {
                  this.form.get('TaxSK').patchValue(tax['SysKey']);
                  this.form.get('TaxPercent').patchValue(tax['TaxPercent']);
                  this.form.get('HasGST').patchValue(tax['TaxPercent'] > 0)

                });
            } else {
              this.form.get('TaxPercent').patchValue(environment.defaultPurchaseZeroTax.taxSK);
              this.form.get('TaxPercent').patchValue(environment.defaultPurchaseZeroTax.taxPercent);
              this.form.get('HasGST').patchValue(false)
            }
          })
      }
      //needs manually emit form touch.
      this.form.get('SalespersonName').markAllAsTouched();
      this.form.get('SalespersonName').markAsTouched();
    })

    return await m.present();
  }

  handleSharePercent() {

    let rate = this.form.get('SharePercent').value;
    let price = this.transactionAmout;
    let tax = this.form.get('TaxPercent').value;
    let gross = (price / 100) * rate;
    let context = getComissionContext({ tax: tax, amount: gross, call: 'gross' })

    this.form.get('GrossComm').patchValue(gross);
    this.form.get('GSTValue').patchValue(context.taxAmount);
    this.form.get('TotalPayable').patchValue(context.total);
  }

  handleGrossComm() {

    let price = this.transactionAmout;
    let gross = this.form.get('GrossComm').value;
    let tax = this.form.get('TaxPercent').value;
    let context = getComissionContext({ tax: tax, amount: gross, call: 'gross' });

    this.form.get('SharePercent').patchValue((gross / price) * 100);
    this.form.get('GSTValue').patchValue(context.taxAmount);
    this.form.get('TotalPayable').patchValue(context.total);
  }

  handleTotalAmount() {

    let total = this.form.get('TotalPayable').value;
    let price = this.transactionAmout;
    let tax = this.form.get('TaxPercent').value;
    let context = getComissionContext({ tax: tax, amount: total, call: 'total' })

    this.form.get('SharePercent').patchValue((context.gross / price) * 100);
    this.form.get('GSTValue').patchValue(context.taxAmount);
    this.form.get('GrossComm').patchValue(context.gross);
  }

  handleClose() {
    this._modalController.dismiss();
  }

  handleOK() {

    this._formUtilityService.makeFormTouched(this.form);

    if (this.form.valid) {
      this._modalController.dismiss(this.form.value);
    }
  }
}
