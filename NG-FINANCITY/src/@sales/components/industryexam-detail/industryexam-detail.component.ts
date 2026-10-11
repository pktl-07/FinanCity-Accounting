import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { PModalFormComponent } from 'src/@common/components/p-modal-form/p-modal-form.component';
import { catchError, finalize } from 'rxjs/operators';
import { IndustryExamService } from 'src/@sales/services/industryexam.service';
import { makeFormTouched, createJsonFormGroup, SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-industryexam-detail',
  templateUrl: './industryexam-detail.component.html',
  providers: [IndustryExamService]
})
export class IndustryExamDetailComponent extends DetailFormBase implements OnInit, OnDestroy {

  @ViewChild(PModalFormComponent, { static: true }) public _modalForm: PModalFormComponent;
  private _subsink = new SubSink();

  constructor(fb: FormBuilder,
    public _industryExamService: IndustryExamService) {

    super(fb);
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  ngOnInit() {

    this._modalForm
      .controls
      .Ready(this.formReady)
      .Success(this.OnSuccess)
      .StateKey(['RecordStatus'])
      .NewButton({
        action: () => this._industryExamService.default()
      })
      .SaveButton({
        action: () => this._industryExamService.save(this.form.value),
        validate: () => { makeFormTouched(this.form); return this.form.valid; }
      })
      .UpdateButton({
        action: () => this._industryExamService.update(this.form.value),
        validate: () => { makeFormTouched(this.form); return this.form.valid; }
      })
      .DeleteButton({
        action: () => this._industryExamService.delete(this.form.value.SysKey)
      })
  }

  loadData(param: { SysKey?: string, Description?: string }): Promise<FormGroup> {

    return new Promise<FormGroup>((resolve, reject) => {

      let subscribe = param.SysKey ? this._industryExamService.findByKey(param.SysKey) : this._industryExamService.default();
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

          this.form.get('Description').setValidators([Validators.required]);
        })
    })
  }
}
