import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { FilterConnectorService } from 'src/@common/components/validator/fc.filter-connector.service';
import { ModalController } from '@ionic/angular';
import { FormUtilityService } from 'src/app/services/form-utility.service';
import { VendorService } from 'src/app/services/vendor.service';
import { SubSink } from 'src/@common/utils';

@Component({
  selector: 'fc-company-create-modal',
  templateUrl: './company-create-modal.page.html',
  providers: [VendorService, FilterConnectorService, FormUtilityService]
})
export class CompanyCreateModalPage implements OnInit, OnDestroy {

  @Input() form: FormGroup;

  private _subsink = new SubSink();

  constructor(public _modalController: ModalController,
    private _vendorService: VendorService,
    private _formUtilityService: FormUtilityService) {
  }

  ngOnInit(): void {

    this._subsink.sink =  this._formUtilityService.OnSuccess
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
      this._formUtilityService.action(this._vendorService.save(this.form.value), 'save');
    }
  }
}
