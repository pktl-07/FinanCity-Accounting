import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { ClientService } from 'src/@sales/services/client.service';
import { PModalFormComponent } from 'src/@common/components/p-modal-form/p-modal-form.component';
import { ButtonControlBuilder } from 'src/@common/components/p-toolbar/button-control.builder';
import { catchError, finalize } from 'rxjs/operators';
import { InternationalNameSpliterService } from 'src/@common/service/internation.name.splitter.service';
import { salutation, martialStatus, gender, makeFormTouched, createJsonFormGroup, SubSink } from 'src/@common/service/utility-service';
import { PImageUploaderComponent } from 'src/@common/components/p-image-uploader/p-image-uploader.component';

@Component({
  selector: 'app-client-management-detail',
  templateUrl: './client-management-detail.component.html',
  providers: [ClientService,
    InternationalNameSpliterService]
})
export class ClientManagementDetailComponent extends DetailFormBase implements OnInit, OnDestroy {

  @ViewChild(PModalFormComponent, { static: true }) public _modalForm: PModalFormComponent;
  @ViewChild(PImageUploaderComponent) _imageUploader: PImageUploaderComponent;

  _salutation = [];
  _martialStatus = [];
  _gender = [];

  private _subsink = new SubSink();

  constructor(fb: FormBuilder,
    public _nameConventionService: InternationalNameSpliterService,
    public _buttonControlBuilder: ButtonControlBuilder,
    public _clientService: ClientService) {

    super(fb);
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  ngOnInit() {

    this._salutation = salutation();
    this._martialStatus = martialStatus();
    this._gender = gender();

    this._modalForm
      .controls
      .Ready(this.formReady)
      .Success(this.OnSuccess)
      .StateKey(['Person', 'RecordStatus'])
      .NewButton({ action: () => this._clientService.default().pipe(finalize(() => this._imageUploader.clear())) })
      .SaveButton({
        action: () => this._clientService.save(this.form.value),
        validate: () => { makeFormTouched(this.form); return this.form.valid; }
      })
      .UpdateButton({
        action: () => this._clientService.update(this.form.value),
        validate: () => { makeFormTouched(this.form); return this.form.valid; }
      })
      .DeleteButton({
        action: () => this._clientService.delete(this.form.value.Person.SysKey)
      })
  }

  loadData(param: { SysKey?: string, ClientName?: string }): Promise<FormGroup> {

    return new Promise<FormGroup>((resolve, reject) => {

      let subscribe = param.SysKey ? this._clientService.findByKey(param.SysKey) : this._clientService.default();
      this._subsink.sink = subscribe.pipe(
        catchError(error => {

          reject(error);
          return error;
        }),
        finalize(() => resolve(this.form)))
        .subscribe(data => {

          const c: any = createJsonFormGroup(data);
          this.form = this.fb.group(c);

          if (param.ClientName)
            this.form.get('Person').get('Name').patchValue(param.ClientName);

          this._nameConventionService.register({

            sourceName: this.form.get('Person').get('Name'),
            fristName: this.form.get('Person').get('FirstName'),
            lastName: this.form.get('Person').get('LastName'),
            isLastNameFirst: this.form.get('Person').get('IsLastNameFirst'),
          });

          this._nameConventionService.watch();

          this.form.get('Person').get('Name').setValidators([Validators.required]);
          this.form.get('Person').get('NRIC').setValidators([Validators.required]);
        });
    });
  }
}
