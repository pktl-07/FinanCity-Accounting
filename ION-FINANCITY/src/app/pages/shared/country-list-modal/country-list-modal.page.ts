import { Component } from '@angular/core';
import { CountryService } from 'src/app/services/country-service';
import { FilterConnectorService } from 'src/@common/components/validator/fc.filter-connector.service';
import { ModalController } from '@ionic/angular';

@Component({
  selector: 'fc-country-modal',
  templateUrl: './country-list-modal.page.html',
  providers: [CountryService,
    FilterConnectorService]
})
export class CountryListModalPage {

  constructor(public _countryService: CountryService,
    public _modalController: ModalController,
    public _filterConnectorService: FilterConnectorService) { }


  async handleCancel() {
    await this._modalController.dismiss();
  }

  async handleSelect(item: any) {
    await this._modalController.dismiss(item);
  }
}
