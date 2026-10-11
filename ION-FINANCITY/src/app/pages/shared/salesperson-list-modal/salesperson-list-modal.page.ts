import { Component } from '@angular/core';
import { FilterConnectorService } from 'src/@common/components/validator/fc.filter-connector.service';
import { ModalController } from '@ionic/angular';
import { SalespersonManagementService } from 'src/app/services/salesperson-management.service';

@Component({
  selector: 'fc-salesperson-list-modal',
  templateUrl: './salesperson-list-modal.page.html',
  providers: [SalespersonManagementService,
    FilterConnectorService]
})
export class SalespersonListModalPage {

  constructor(public _salespersonManagementService: SalespersonManagementService,
    public _modalController: ModalController,
    public _filterConnectorService: FilterConnectorService) { }

  async handleSelect(item: any) {
    await this._modalController.dismiss(item);
  }

  handleClose() {
    this._modalController.dismiss();
  }
}
