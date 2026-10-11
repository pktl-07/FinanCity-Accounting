import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ActionSheetController } from '@ionic/angular';
import { FormBuilder, FormGroup } from '@angular/forms';
import { FormUtilityService } from 'src/app/services/form-utility.service';
import { BroadcasterService } from 'src/app/services/broadcaster.service';
import { ExternalSalespersonService } from 'src/app/services/external-salesperson.service';
import { SubSink } from 'src/@common/utils';

@Component({
  selector: 'fc-cobroker-contact-detail',
  templateUrl: './cobroker-contact-detail.page.html',
  providers: [ExternalSalespersonService,
    FormUtilityService]
})
export class CoBrokerContactDetailPage implements OnInit, OnDestroy {

  public form: FormGroup;
  private _subsink = new SubSink();

  constructor(private _activeRoute: ActivatedRoute,
    private _externalSalespersonService: ExternalSalespersonService,
    private _broadcaster: BroadcasterService,
    private _formUtilityService: FormUtilityService,
    private _formBuilder: FormBuilder,
    private _actionSheetController: ActionSheetController) {
  }

  ngOnInit() {

    this._subsink.sink = this._formUtilityService.OnSuccess
      .subscribe((data: any) => {
        if (data['Person']['RecordStatus'] != 0)
          this._broadcaster.broadcast('external-salesperson-reload');
        this._formUtilityService.rebind(this.form, data);
      })

    this._subsink.sink = this._activeRoute.data
      .subscribe((data) => {
        let c = this._formUtilityService.formGroupJson(data['externalSalsperson']);
        this.form = this._formBuilder.group(c);
      })
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  action() {

    const config = this._formUtilityService.build({
      form: this.form,
      status: +this.form.get('Person').get('RecordStatus').value,
      newHandler: this._externalSalespersonService.default(),
      saveHandler: this._externalSalespersonService.save(this.form.value),
      updateHandler: this._externalSalespersonService.update(this.form.value),
      deleteHandler: this._externalSalespersonService.delete(this.form.value.Person.SysKey)
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
