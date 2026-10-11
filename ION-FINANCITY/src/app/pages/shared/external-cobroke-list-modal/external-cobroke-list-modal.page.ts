import { Component, ViewChild, Input } from '@angular/core';
import { FilterConnectorService } from 'src/@common/components/validator/fc.filter-connector.service';
import { ModalController } from '@ionic/angular';
import { ExternalSalespersonService } from 'src/app/services/external-salesperson.service';
import { FCSearchableToolbarComponent } from 'src/@common/components/toolbar/fc-searchable-toolbar.component';
import { FormUtilityService } from 'src/app/services/form-utility.service';
import { FormBuilder, FormGroup } from '@angular/forms';
import { VendorService } from 'src/app/services/vendor.service';
import { ExternalCobrokeCreateModalPage } from '../external-cobroke-create-modal/external-cobroke-create-modal.page';

@Component({
  selector: 'fc-external-cobroke-list-modal',
  templateUrl: './external-cobroke-list-modal.page.html',
  providers: [ExternalSalespersonService,
    FilterConnectorService,
    VendorService,
    FormUtilityService]
})
export class ExtenralCobrokeListModalPage {

  @ViewChild(FCSearchableToolbarComponent, { static: true }) filterToolBar: FCSearchableToolbarComponent;

  @Input() vendorSK: string;
  @Input() vendorType: number;
  @Input() disabledCompanyOption = false;

  constructor(public _externalSalespersonService: ExternalSalespersonService,
    private _formUtilityService: FormUtilityService,
    private _formBuilder: FormBuilder,
    public _modalController: ModalController,
    public _filterConnectorService: FilterConnectorService) { }

  async handleCreate(value: string) {

    let form = await this.createForm();
    //set default value for salesperson accroding to write down in the search bar and copany selected.
    form.get('Person').get('Name').patchValue(value);
    form.get('Person').get('VendorSK').patchValue(this.vendorSK);

    let m = await this._modalController.create({
      component: ExternalCobrokeCreateModalPage,
      componentProps: {
        form: form,
        disabledCompanyOption: this.disabledCompanyOption
      }
    })

    m.onWillDismiss().then((data) => {
      if (data.data) {
        this.filterToolBar.refresh();
      }
    });

    await m.present();
  }

  async handleSelect(item: any) {

    await this._modalController.dismiss(item);
  }

  async handleClose() {

    await this._modalController.dismiss();
  }

  async createForm() {

    let form: FormGroup;
    let data: any = await this._externalSalespersonService.default().toPromise();
    let c = this._formUtilityService.formGroupJson(data);
    form = this._formBuilder.group(c);

    return form;
  }
}
