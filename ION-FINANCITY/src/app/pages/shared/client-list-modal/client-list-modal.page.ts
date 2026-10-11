import { Component, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { FilterConnectorService } from 'src/@common/components/validator/fc.filter-connector.service';
import { ModalController } from '@ionic/angular';
import { ClientService } from 'src/app/services/client-service';
import { ClientCreateModalPage } from '../client-create-modal/client-create-modal.page';
import { FormUtilityService } from 'src/app/services/form-utility.service';
import { FCSearchableToolbarComponent } from 'src/@common/components/toolbar/fc-searchable-toolbar.component';

@Component({
  selector: 'fc-client-list-modal',
  templateUrl: './client-list-modal.page.html',
  providers: [FilterConnectorService,
    ClientService,
    FormUtilityService]
})
export class ClientListModalPage {

  @ViewChild(FCSearchableToolbarComponent, { static: true }) filterToolBar: FCSearchableToolbarComponent;
  constructor(public _clientService: ClientService,
    private _fb: FormBuilder,
    private _formUtilityService: FormUtilityService,
    public _modalController: ModalController,
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
    form.get('Person').get('Name').patchValue(value);

    let m = await this._modalController.create({
      component: ClientCreateModalPage,
      componentProps: {
        form: form
      }
    })

    m.onWillDismiss().then(() => {

      this.filterToolBar.refresh();
    })

    await m.present();
  }

  async createForm() {

    let data = await this._clientService.default().toPromise();
    let c = this._formUtilityService.formGroupJson(data);

    return this._fb.group(c);
  }
}
