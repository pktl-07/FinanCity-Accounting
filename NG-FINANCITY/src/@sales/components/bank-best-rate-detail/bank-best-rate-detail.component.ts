import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { PModalFormComponent } from 'src/@common/components/p-modal-form/p-modal-form.component';
import { catchError, finalize } from 'rxjs/operators';
import { makeFormTouched, createJsonFormGroup, SubSink } from 'src/@common/service/utility-service';
import { PDatePeriodComponent } from 'src/@common/components/p-date-period/p-date-period.component';
import { BankBestRateService } from 'src/@sales/services/bank-best-rate.service';
import { PFileUploaderComponent } from 'src/@common/components/p-file-uploader/p-file-uploader.component';
import { PImageUploaderComponent } from 'src/@common/components/p-image-uploader/p-image-uploader.component';

@Component({
  selector: 'app-bank-best-rate-detail',
  templateUrl: './bank-best-rate-detail.component.html',
  providers: [BankBestRateService]
})
export class BankBestRateDetailComponent extends DetailFormBase implements OnInit, OnDestroy {

  @ViewChild(PModalFormComponent, { static: true }) public _modalForm: PModalFormComponent;
  @ViewChild(PDatePeriodComponent, { static: true }) public _datePeriod: PDatePeriodComponent;
  @ViewChild(PImageUploaderComponent) _imageUploader: PImageUploaderComponent;
  @ViewChild(PFileUploaderComponent) public _fileUploader: PFileUploaderComponent;

  private _subsink = new SubSink();

  constructor(fb: FormBuilder,
    public _bankBestRateService: BankBestRateService) {

    super(fb);
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  uploadSaveUrl = "";
  uploadRemoveUrl = "";

  ngOnInit() {
    this._modalForm
      .controls
      .Ready(this.formReady)
      .Success(this.OnSuccess)
      .StateKey(['RecordStatus'])
      .NewButton({
        action: () => this._bankBestRateService.default().pipe(
          finalize(() => {
            this._fileUploader.clear();
            this._imageUploader.clear();
          })
        )
      })
      .SaveButton({
        action: () => this._bankBestRateService.save(this.form.value),
        validate: () => { makeFormTouched(this.form); return this.form.valid; }
      })
      .UpdateButton({
        action: () => this._bankBestRateService.update(this.form.value),
        validate: () => { makeFormTouched(this.form); return this.form.valid; }
      })
      .DeleteButton({
        action: () => this._bankBestRateService.delete(this.form.value.SysKey)
      })
  }

  loadData(param: { SysKey?: string, Description?: string }): Promise<FormGroup> {

    return new Promise<FormGroup>((resolve, reject) => {

      let subscribe = param.SysKey ? this._bankBestRateService.findByKey(param.SysKey) : this._bankBestRateService.default();

      this._subsink.sink = subscribe.pipe(
        catchError(error => {
          reject(error);
          return error;
        }),
        finalize(() => resolve(this.form)))
        .subscribe(data => {

          const c: any = createJsonFormGroup(data);
          this.form = this.fb.group(c);

          if (param.Description)
            this.form.get('Description').patchValue(param.Description);

          this.form.get('Rate').setValidators([Validators.required, Validators.min(0.0001)]);
          this.form.get('Description').setValidators([Validators.required]);
          this.form.get('EffectiveDate').setValidators([Validators.required])
        })
    });
  }
}
