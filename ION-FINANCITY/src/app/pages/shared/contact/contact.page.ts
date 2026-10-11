import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';
import { PostalCodeService } from 'src/app/services/postal-code.service';
import { ModalController } from '@ionic/angular';
import { CountryListModalPage } from '../country-list-modal/country-list-modal.page';
import { SubSink } from 'src/@common/utils';

@Component({
  selector: 'fc-contact',
  templateUrl: './contact.page.html',
  providers: [PostalCodeService]
})
export class ContactPage implements OnInit, OnDestroy {

  @Input() form: FormGroup;
  @Input() visibleTelephone = true;
  @Input() visibleMobileNumber = true;
  @Input() visibleEmailAddress = true;

  private _subsink = new SubSink();

  constructor(private _postalCodeService: PostalCodeService,
    private _modalController: ModalController) { }

  ngOnInit() {
    this.form.get('Country').setValidators([Validators.required])
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  handlePostalCodeChange(event: any) {
    let p = event.target.value;
    this._subsink.sink = this._postalCodeService
      .findByKey(p)
      .subscribe(d => {
        if (d && d != null) {
          this.form.get('BuildingHouseNo').setValue(d['Blk']);
          this.form.get('Address1').setValue(d['Address1']);
          this.form.get('Address2').setValue(d['Address2']);
        }
      })
  }

  async handleCountryClick() {
    const m = await this._modalController.create({
      component: CountryListModalPage
    })
    m.onWillDismiss().then((data) => {
      if (data.data)
        this.form.get('Country').setValue(data.data.Description);
    })
    return await m.present();
  }
}
