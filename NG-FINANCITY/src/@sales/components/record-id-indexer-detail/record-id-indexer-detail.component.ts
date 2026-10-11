import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { IDReferenceIndexService } from 'src/@sales/services/Id-reference-Index-service';
import { PModalFormComponent } from 'src/@common/components/p-modal-form/p-modal-form.component';
import { catchError, finalize } from 'rxjs/operators';
import { indexingFormat, makeFormTouched, createJsonFormGroup, SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-record-id-indexer-detail',
  templateUrl: './record-id-indexer-detail.component.html',
  providers: [IDReferenceIndexService]
})
export class RecordIdIndexerDetailComponent extends DetailFormBase implements OnInit, OnDestroy {

  @ViewChild(PModalFormComponent, { static: true }) public _modalForm: PModalFormComponent;

  indexingFormat = [];
  private _subsink = new SubSink();

  constructor(fb: FormBuilder,
    public _idexerService: IDReferenceIndexService) {

    super(fb);
  }

  ngOnInit() {

    this.indexingFormat = indexingFormat();

    this._modalForm
      .controls
      .Ready(this.formReady)
      .Success(this.OnSuccess)
      .StateKey(['Indexer', 'RecordStatus'])
      .UpdateButton({
        action: () => this._idexerService.update(this.form.value.Indexer),
        validate: () => { makeFormTouched(this.form); return this.form.valid; }
      });
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  baseDate = [];
  transTypeDescription: string;

  loadData(param: { SysKey: string }): Promise<FormGroup> {

    return new Promise<FormGroup>((resolve, reject) => {

      let subscribe = param.SysKey ? this._idexerService.findByKey(param.SysKey) : this._idexerService.default();

      this._subsink.sink = subscribe.pipe(
        catchError(error => {

          reject(error);
          return error;
        }),
        finalize(() => resolve(this.form)))
        .subscribe(data => {

          this.baseDate = data.BaseDateCollection;
          this.transTypeDescription = data.TransTypeDescription;

          const c: any = createJsonFormGroup(data);

          this.form = this.fb.group(c);
          this.form.get('Indexer').get('Prefix').setValidators(Validators.required);
          this.form.get('Indexer').get('StartValue').setValidators(Validators.required);
          this.form.get('Indexer').get('EndValue').setValidators(Validators.required);
        })
    });
  }
}
