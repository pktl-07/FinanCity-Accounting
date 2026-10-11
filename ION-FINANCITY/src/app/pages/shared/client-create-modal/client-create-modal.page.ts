import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { FilterConnectorService } from 'src/@common/components/validator/fc.filter-connector.service';
import { ModalController } from '@ionic/angular';
import { FormUtilityService } from 'src/app/services/form-utility.service';
import { ClientService } from 'src/app/services/client-service';
import { SubSink } from 'src/@common/utils';

@Component({
  selector: 'fc-client-create-modal',
  templateUrl: './client-create-modal.page.html',
  providers: [ClientService, FilterConnectorService, FormUtilityService]
})
export class ClientCreateModalPage implements OnInit, OnDestroy {

  @Input() form: FormGroup;
  @Input() action: 'save' | 'edit' = 'save';

  private _subsink = new SubSink();

  constructor(public _modalController: ModalController,
    private _clientService: ClientService,
    private _formUtilityService: FormUtilityService) {
  }

  ngOnInit(): void {

    this._subsink.sink = this._formUtilityService.OnSuccess
      .subscribe((data: any) => {
        this._modalController.dismiss(data);
      })
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  handleClose() {
    this._modalController.dismiss();
  }

  async handleOk() {

    this._formUtilityService.makeFormTouched(this.form);

    if (this.form.valid) {

      if (this.action == 'save')
        this._formUtilityService.action(this._clientService.save(this.form.value), 'save');
      else if (this.action == 'edit') {
        this._formUtilityService.action(this._clientService.update(this.form.value), 'update');
      }
    }
  }
}
