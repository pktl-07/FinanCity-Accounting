import { Component, OnInit, OnDestroy, Inject } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { map } from 'rxjs/operators';
import { FormUtilityService } from 'src/app/services/form-utility.service';
import { SalespersonManagementService } from 'src/app/services/salesperson-management.service';
import { ActionSheetController } from '@ionic/angular';
import { IFCConfig, SubSink } from 'src/@common/utils';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'fc-my-profile',
  templateUrl: './my-profile.page.html',
  providers: [FormUtilityService,
    SalespersonManagementService]
})
export class MyProfilePage implements OnInit, OnDestroy {

  public form: FormGroup;
  private _subsink = new SubSink();

  constructor(private _activeRoute: ActivatedRoute,
    private _salespersonManagementService: SalespersonManagementService,
    private _formBuilder: FormBuilder,
    private _actionSheetController: ActionSheetController,
    private _formUtilityService: FormUtilityService) {
  }

  ngOnInit() {

    this._subsink.sink = this._formUtilityService.OnSuccess
      .subscribe((data: any) => {
        this._formUtilityService.rebind(this.form, data);
      })

    this._subsink.sink = this._activeRoute.data.subscribe((data) => {
      let c = this._formUtilityService.formGroupJson(data['profile'],
        null,
        fg => { },
        [
          'CloseSalesCommissionToYear',
          'CloseSalesCommissionToDate',
          'CloseSalesValueToYear',
          'CloseSalesValueToDate',
        ]);
      this.form = this._formBuilder.group(c);
    })
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  action() {
    const config = this._formUtilityService.build({
      form: this.form,
      status: 10,
      updateHandler: this._salespersonManagementService.update(this.form.value)
        .pipe(map((response) => {
          environment.auth.profileName.next(`${(<string>this.form.get('Person').get('Name').value).toUpperCase()}`);
          return response;
        })
        ),
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
