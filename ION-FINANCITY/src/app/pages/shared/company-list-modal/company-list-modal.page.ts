import { Component, ViewChild, Input } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { FilterConnectorService } from 'src/@common/components/validator/fc.filter-connector.service';
import { ModalController } from '@ionic/angular';
import { VendorService } from 'src/app/services/vendor.service';
import { FormUtilityService } from 'src/app/services/form-utility.service';
import { CompanyCreateModalPage } from '../company-create-modal/company-create-modal.page';
import { FCSearchableToolbarComponent } from 'src/@common/components/toolbar/fc-searchable-toolbar.component';
import { TaxService } from 'src/app/services/tax.service';

@Component({
  selector: 'fc-company-list-modal',
  templateUrl: './company-list-modal.page.html',
  providers: [
    VendorService,
    FilterConnectorService,
    FormUtilityService,
    TaxService]
})
export class CompanyListModalPage {

  @ViewChild(FCSearchableToolbarComponent, { static: true }) filterToolBar: FCSearchableToolbarComponent;
  @Input() transType: number;

  constructor(public _vendorService: VendorService,
    public _modalController: ModalController,
    private _formBuilder: FormBuilder,
    private _formUtilityService: FormUtilityService,
    public _filterConnectorService: FilterConnectorService) {
  }

  handleSelect(item: any) {

    this._modalController.dismiss(item);
  }

  handleClose() {

    this._modalController.dismiss();
  }

  async handleCreate(value: string) {

    let form = await this.createForm();

    form.get('Vendor').get('CompanyName').patchValue(value);

    if (this.transType)
      form.get('Vendor').get('TransType').patchValue(this.transType);

    let m = await this._modalController.create({
      component: CompanyCreateModalPage,
      componentProps: {
        form: form
      }
    })

    m.onWillDismiss().then((data) => {
      if (data.data) {
        this.filterToolBar.refresh();
      }
    });

    await m.present();
  }

  async createForm() {

    let data: any = await this._vendorService.default().toPromise();
    let c = this._formUtilityService.formGroupJson(data);

    return this._formBuilder.group(c);
  }
}
