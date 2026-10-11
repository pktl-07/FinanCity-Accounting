import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { FilterConnectorService } from 'src/@common/components/validator/fc.filter-connector.service';
import { ModalController } from '@ionic/angular';
import { FormUtilityService } from 'src/app/services/form-utility.service';
import { ExternalSalespersonService } from 'src/app/services/external-salesperson.service';
import { SubSink } from 'src/@common/utils';

@Component({
  selector: 'fc-external-cobroke-create-modal',
  templateUrl: './external-cobroke-create-modal.page.html',
  providers: [ExternalSalespersonService, FilterConnectorService, FormUtilityService]
})
export class ExternalCobrokeCreateModalPage implements OnInit, OnDestroy {

  @Input() form: FormGroup;
  @Input() disabledCompanyOption = true;

  private _subsink = new SubSink();

  constructor(public _modalController: ModalController,
    private _externalSalespersonService: ExternalSalespersonService,
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
      this._formUtilityService.action(this._externalSalespersonService.save(this.form.value), 'save');
    }
  }
}
