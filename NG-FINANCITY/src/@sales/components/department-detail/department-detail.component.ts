import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { PModalFormComponent } from 'src/@common/components/p-modal-form/p-modal-form.component';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { FormBuilder, Validators } from '@angular/forms';
import { DepartmentService } from 'src/@accounts/services/department.service';
import { makeFormTouched, createJsonFormGroup, rebindJsonFormGroup, SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-department-detail',
  templateUrl: './department-detail.component.html',
  providers: [DepartmentService]
})
export class DepartmentDetailComponent extends DetailFormBase implements OnInit, OnDestroy {

  @ViewChild(PModalFormComponent, { static: true }) public _modalForm: PModalFormComponent;

  private _subsink = new SubSink();

  constructor(fb: FormBuilder,
    public _departmentService: DepartmentService) {

    super(fb);

    this._subsink.sink = this.OnSuccess
      .subscribe(item => {

        const status: any = item.data.RecordStatus;
        rebindJsonFormGroup(this.form, item.data, status == 0 ? 'null custom form data' : 'none');
        this._modalForm.changeState(status);
      });
  }

  ngOnInit() {

    this._modalForm
      .controls
      .Success(this.OnSuccess)
      .NewButton({ action: () => this._departmentService.default() })
      .SaveButton({
        action: () => this._departmentService.save(this.form.value),
        validate: () => { makeFormTouched(this.form); return this.form.valid; }
      })
      .UpdateButton({
        action: () => this._departmentService.update(this.form.value),
        validate: () => { makeFormTouched(this.form); return this.form.valid; }
      })
      .DeleteButton({
        action: () => this._departmentService.delete(this.form.value.SysKey),
      })
  }

  ngOnDestroy(): void {

    this._subsink.unsubscribe();
  }

  loadData(param: { SysKey?: string, Description?: string }): Promise<any> {

    return new Promise<any>(async (resolve) => {

      let data: any;

      if (param.SysKey)
        data = await this._departmentService.findByKey(param.SysKey).toPromise();
      else
        data = await this._departmentService.default().toPromise();

      this._modalForm.changeState(data.RecordStatus); //set change state

      const c: any = createJsonFormGroup(data);

      this.form = this.fb.group(c);

      if (param.Description)
        this.form.get('Description').patchValue(param.Description);

      this.form.get('R1').setValidators(Validators.required);
      this.form.get('Description').setValidators(Validators.required);

      resolve(null);
    });
  }
}
