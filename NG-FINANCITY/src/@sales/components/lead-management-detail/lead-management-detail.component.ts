import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { PModalFormComponent } from 'src/@common/components/p-modal-form/p-modal-form.component';
import { ButtonControlBuilder } from 'src/@common/components/p-toolbar/button-control.builder';
import { catchError, finalize } from 'rxjs/operators';
import { InternationalNameSpliterService } from 'src/@common/service/internation.name.splitter.service';
import { salutation, martialStatus, gender, makeFormTouched, createJsonFormGroup, informationSource, saleActivity, SubSink, NotifyMeBefore, LeadCloseRate } from 'src/@common/service/utility-service';
import { LeadService } from 'src/@sales/services/lead.service';
import { TNC_PropertyTypeService } from 'src/@sales/services/tnc_propertyType.service';
import { BehaviorSubject } from 'rxjs';
import { LeadManagementDetailShareComponent } from './lead-management-share/lead-management-detail-share.component';
import { PImageUploaderComponent } from 'src/@common/components/p-image-uploader/p-image-uploader.component';

@Component({
  selector: 'app-lead-management-detail',
  templateUrl: './lead-management-detail.component.html',
  providers: [LeadService,
    InternationalNameSpliterService,
    TNC_PropertyTypeService]
})
export class LeadManagementDetailComponent extends DetailFormBase implements OnInit, OnDestroy {

  @ViewChild(LeadManagementDetailShareComponent, { static: true }) public _contentShareComponent: LeadManagementDetailShareComponent;
  @ViewChild(PModalFormComponent, { static: true }) public _modalForm: PModalFormComponent;
  @ViewChild(PImageUploaderComponent) _imageUploader: PImageUploaderComponent;

  _salutation = [];
  _martialStatus = [];
  _gender = [];
  _source = [];
  _salesActivity = [];
  _notifymeBefore = [];
  _leadCloseRate = [];

  private _subsink = new SubSink();

  shareInsertedSubject = new BehaviorSubject([]);
  shareDeletedSubject = new BehaviorSubject([]);
  insertedRemarkSubject = new BehaviorSubject([]);
  deletedRemarkSubject = new BehaviorSubject([]);

  shareContentOriginKeySelection = [];
  shareContentKeySelection = [];

  constructor(fb: FormBuilder,
    public _nameConventionService: InternationalNameSpliterService,
    public _buttonControlBuilder: ButtonControlBuilder,
    public _propertyTypeService: TNC_PropertyTypeService,
    public _leadService: LeadService) {

    super(fb);

    this._subsink.sink = this.OnSuccess
      .subscribe((response) => {

        this.shareContentKeySelection = [];
        this.shareContentOriginKeySelection = [];
        this.shareContentKeySelection.push(...response.data['ShareContacts']);
        this.shareContentOriginKeySelection.push(...response.data['ShareContacts']);

        this.insertedRemarkSubject.next(response.data['Remarks']);
      })
  }

  ngOnInit() {

    this._salutation = salutation();
    this._martialStatus = martialStatus();
    this._gender = gender();
    this._source = informationSource();
    this._salesActivity = saleActivity();
    this._propertyTypeService.asyncFinds();
    this._notifymeBefore = NotifyMeBefore;
    this._leadCloseRate = LeadCloseRate;

    this._modalForm
      .controls
      .Ready(this.formReady)
      .Success(this.OnSuccess)
      .StateKey(['Person', 'RecordStatus'])
      .NewButton({
        action: () => this._leadService.default().pipe(finalize(() => {
          this._imageUploader.clear();
        }))
      })
      .SaveButton({
        action: () => this._leadService.save({
          Lead: this.form.get('Lead').value,
          Person: this.form.get('Person').value,
          Contact: this.form.get('Contact').value,
          FollowUp: this.form.get('FollowUp').value,
          InsertedRemarkItems: this.insertedRemarkSubject.value,
          InsertedSharedContacts: this.shareInsertedSubject.value,
          DeletedSharedContacts: this.shareDeletedSubject.value,
        }),
        validate: () => { makeFormTouched(this.form); return this.form.valid; }
      })
      .UpdateButton({
        action: () => this._leadService.update({
          Lead: this.form.get('Lead').value,
          Person: this.form.get('Person').value,
          Contact: this.form.get('Contact').value,
          FollowUp: this.form.get('FollowUp').value,
          InsertedSharedContacts: this.shareInsertedSubject.value,
          DeletedSharedContacts: this.shareDeletedSubject.value,
        }),
        validate: () => { makeFormTouched(this.form); return this.form.valid; }
      })
      .DeleteButton({
        action: () => this._leadService.delete(this.form.value.Person.SysKey)
      })
  }

  ngOnDestroy(): void {

    this._subsink.unsubscribe();
  }

  loadData(param: { SysKey?: string, ClientName?: string }): Promise<FormGroup> {

    return new Promise<FormGroup>((resolve, reject) => {

      let subscribe = param.SysKey ? this._leadService.findByKey(param.SysKey) : this._leadService.default();
      subscribe.pipe(
        catchError(error => {

          reject(error);
          return error;
        }),
        finalize(() => resolve(this.form)))
        .subscribe(data => {

          this.shareContentKeySelection = [];
          this.shareContentOriginKeySelection = [];
          this.shareContentKeySelection.push(...data['ShareContacts']);
          this.shareContentOriginKeySelection.push(...data['ShareContacts']);

          const c: any = createJsonFormGroup(data);
          this.form = this.fb.group(c);

          this.insertedRemarkSubject.next(data['Remarks']);

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
          this.form.get('Lead').get('PropertyTypeSK').setValidators([Validators.required]);
          this.form.get('Lead').get('Source').setValidators([Validators.required]);
          this.form.get('Lead').get('SalesActivity').setValidators([Validators.required]);
          this.form.get('FollowUp').get('StartDate').setValidators([Validators.required]);
        });
    });
  }
}
