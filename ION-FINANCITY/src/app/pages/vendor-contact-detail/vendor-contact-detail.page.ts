import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ActionSheetController } from '@ionic/angular';
import { FormBuilder, FormGroup } from '@angular/forms';
import { FormUtilityService } from 'src/app/services/form-utility.service';
import { BroadcasterService } from 'src/app/services/broadcaster.service';
import { VendorService } from 'src/app/services/vendor.service';
import { SubSink } from 'src/@common/utils';

@Component({
  selector: 'fc-vendor-contact-detail',
  templateUrl: './vendor-contact-detail.page.html',
  providers: [FormUtilityService,
    VendorService]
})
export class VendorContactDetailPage implements OnInit, OnDestroy {

  public form: FormGroup;
  private _subsink = new SubSink();

  constructor(private _activeRoute: ActivatedRoute,
    private _vendorService: VendorService,
    private _broadcaster: BroadcasterService,
    private _formUtilityService: FormUtilityService,
    private _formBuilder: FormBuilder,
    private _actionSheetController: ActionSheetController) {
  }

  ngOnInit() {

    this._subsink.sink = this._formUtilityService.OnSuccess
      .subscribe((data: any) => {

        if (data['Vendor']['RecordStatus'] != 0)
          this._broadcaster.broadcast('vendor-list-reload');

        this._formUtilityService.rebind(this.form, data);
      })

    this._subsink.sink = this._activeRoute.data
      .subscribe((data) => {

        let c = this._formUtilityService.formGroupJson(data['vendor']);
        this.form = this._formBuilder.group(c);
      })
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  action() {

    const config = this._formUtilityService.build({
      form: this.form,
      status: +this.form.get('Vendor').get('RecordStatus').value,
      newHandler: this._vendorService.default(),
      saveHandler: this._vendorService.save(this.form.value),
      updateHandler: this._vendorService.update(this.form.value),
      deleteHandler: this._vendorService.delete(this.form.value.Vendor.SysKey)
    });

    this._subsink.sink = config
      .subscribe(async (options) => {
        if (options.valid) {
          const actionSheet = await this._actionSheetController.create(options.config);
          await actionSheet.present();
        }
      })
  }
}
