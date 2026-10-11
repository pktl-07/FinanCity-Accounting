import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { PModalFormComponent } from 'src/@common/components/p-modal-form/p-modal-form.component';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { catchError, finalize } from 'rxjs/operators';
import { makeFormTouched, createJsonFormGroup, SubSink } from 'src/@common/service/utility-service';
import { JobService } from 'src/@accounts/services/job.service';

@Component({
  selector: 'app-job-detail',
  templateUrl: './job-detail.component.html',
  providers: [
    JobService
  ]
})
export class JobDetailComponent extends DetailFormBase implements OnInit, OnDestroy {

  @ViewChild(PModalFormComponent, { static: true }) public _modalForm: PModalFormComponent;

  private _subsink = new SubSink();

  constructor(fb: FormBuilder,
    public _jobService: JobService) {

    super(fb);
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  ngOnInit() {

    this._modalForm
      .controls
      .Success(this.OnSuccess)
      .Ready(this.formReady)
      .StateKey(['RecordStatus'])
      .NewButton({
        action: () => this._jobService.default()
      })
      .SaveButton({
        action: () => this._jobService.save(this.form.value),
        validate: () => {
          makeFormTouched(this.form);
          return this.form.valid;
        }
      })
      .UpdateButton({
        action: () => this._jobService.update(this.form.value),
        validate: () => {
          makeFormTouched(this.form);
          return this.form.valid;
        }
      })
      .DeleteButton({
        action: () => this._jobService.delete(this.form.value.SysKey)
      })
  }

  loadData(param: { SysKey?: string, Description?: string }): Promise<FormGroup> {

    return new Promise<FormGroup>((resolve, reject) => {

      const subscribe = param.SysKey ? this._jobService.findByKey(param.SysKey) : this._jobService.default();
      this._subsink.sink = subscribe.pipe(catchError(error => {
        reject(error);
        return error;
      }),
        finalize(() => resolve(this.form)))
        .subscribe(data => {

          const c: any = createJsonFormGroup(data);
          this.form = this.fb.group(c);

          if (param.Description)
            this.form.get('Description').patchValue(param.Description);

          this.form.get('Description').setValidators(Validators.required);
        })
    })
  }
}
