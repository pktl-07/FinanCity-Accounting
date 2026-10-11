import { Component, OnInit, OnDestroy, ViewChild, } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { ActivatedRoute } from '@angular/router';
import { ActionSheetController, } from '@ionic/angular';
import { ClientService } from 'src/app/services/client-service';
import { FormBuilder, FormGroup } from '@angular/forms';
import { FormUtilityService } from 'src/app/services/form-utility.service';
import { BroadcasterService } from 'src/app/services/broadcaster.service';
import { ClientTransactionHistoryPage } from './client-contact-transaction-history.page';
import { PropertyTransactionService } from 'src/app/services/property-transaction.service';
import { SubSink } from 'src/@common/utils';

@Component({
  selector: 'fc-client-contact-detail',
  templateUrl: './client-contact-detail.page.html',
  providers: [ClientService,
    FormUtilityService,
    PropertyTransactionService]
})
export class ClientContactDetailPage implements OnInit, OnDestroy {

  public form: FormGroup;
  private _subsink = new SubSink();
  @ViewChild(ClientTransactionHistoryPage) _history: ClientTransactionHistoryPage;

  constructor(private _activeRoute: ActivatedRoute,
    private _clientService: ClientService,
    private _broadcaster: BroadcasterService,
    private _formUtilityService: FormUtilityService,
    private _formBuilder: FormBuilder,
    private _actionSheetController: ActionSheetController) {
  }

  ngOnInit() {

   this._subsink.sink = this._formUtilityService.OnSuccess
      .subscribe((data: any) => {
        if (data['Person']['RecordStatus'] != 0)
          this._broadcaster.broadcast('client-reload');
        this._formUtilityService.rebind(this.form, data);
      })

    this._subsink.sink = this._activeRoute.data
      .subscribe((data) => {
        let c = this._formUtilityService.formGroupJson(data['client']);
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
      newHandler: this._clientService.default().pipe(finalize(() => {
        this._history.reset();
      })
      ),
      saveHandler: this._clientService.save(this.form.value),
      updateHandler: this._clientService.update(this.form.value),
      deleteHandler: this._clientService.delete(this.form.value.Person.SysKey)
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
