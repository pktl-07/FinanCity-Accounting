import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { PModalFormComponent } from 'src/@common/components/p-modal-form/p-modal-form.component';
import { CommissionStructureService } from 'src/@sales/services/commission-structure.service';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { PGridIncellEditingComponent } from 'src/@common/components/p-grid-incell-editing/p-grid-incell-editing.component';
import { Subject, BehaviorSubject } from 'rxjs';
import { takeUntil, catchError, finalize } from 'rxjs/operators';
import { makeFormTouched, createJsonFormGroup, SubSink } from 'src/@common/service/utility-service';
import { CommissionStructureBaseService } from 'src/@sales/services/commission-sturcutre-base.service';

@Component({
  selector: 'app-commission-scheme-detail',
  templateUrl: './commission-scheme-detail.component.html',
  providers: [
    CommissionStructureService,
    CommissionStructureBaseService,
  ],
  styles: [`
    .pointer-none {
      pointer-events: none;
    }
  `]
})
export class CommissionSchemeDetailComponent extends DetailFormBase implements OnInit, OnDestroy {

  //should static is turn off when it is inside the nested *ngIf.
  @ViewChild('commissionGrid', { read: PGridIncellEditingComponent }) public _dataGrid: PGridIncellEditingComponent;
  @ViewChild(PModalFormComponent, { static: true }) public _modalForm: PModalFormComponent;

  _fatch: BehaviorSubject<Array<any>> = new BehaviorSubject<Array<any>>([]);

  public _activeViewType = [{ text: 'Y', value: true }, { text: 'N', value: false }];
  public _overridingLevles = [0, 1, 2, 3, 4];
  private _subsink = new SubSink();

  constructor(fb: FormBuilder,
    public _commissionStructureBaseService: CommissionStructureBaseService,
    public _commissionSchemeService: CommissionStructureService) {

    super(fb);

    this._subsink.sink = this.OnSuccess
      .subscribe(item => {

        this._fatch.next(item.data.CommissionSchemeDetail);
      });
  }

  //As the grid is inside of the *ngIf and it could not be found or undefined result produce inside of the ngOnInit
  readonly gridConfig = {
    key: 'SysKey',
    fetch: this._fatch,
    columns: [
      { dataType: 'hidden', field: 'SysKey', defaultValue: null },
      { dataType: 'hidden', field: 'ParentSK', defaultValue: null },
      { dataType: 'hidden', field: 'Type', defaultValue: 4 },
      { dataType: 'string', field: 'Designation', title: 'Designation', defaultValue: null, width: 200, validators: Validators.required },
      { dataType: 'string', field: 'BrifeDesignationName', title: 'Abbreviation', defaultValue: null, width: 100, validators: Validators.required },
      { dataType: 'currency', field: 'MinCom', title: 'Base ($)', defaultValue: 0, width: 120, validators: Validators.required },
      { dataType: 'currency', field: 'MaxCom', title: 'Cap ($)', defaultValue: 0, width: 120, validators: Validators.required },
      { dataType: 'number', field: 'AssociatedPercent', title: 'SP (%)', defaultValue: 0, width: 80, validators: Validators.required },
      { dataType: 'number', field: 'AgencyPercent', title: 'Company (%)', defaultValue: 0, width: 80, validators: Validators.required },
      { dataType: 'number', field: 'L1', title: 'L1 (%)', defaultValue: 0, width: 80, validators: Validators.required },
      { dataType: 'number', field: 'L2', title: 'L2 (%)', defaultValue: 0, width: 80, validators: Validators.required },
      { dataType: 'number', field: 'L3', title: 'L3 (%)', defaultValue: 0, width: 80, validators: Validators.required },
      { dataType: 'number', field: 'L4', title: 'L4 (%)', defaultValue: 0, width: 80, validators: Validators.required },
      { dataType: 'chekbox', field: 'IsActive', title: 'Active', defaultValue: true, width: 80 },
    ]
  };

  ngOnInit() {

    this._commissionStructureBaseService.asyncFinds();
    this._modalForm
      .controls
      .Success(this.OnSuccess)
      .Ready(this.formReady)
      .StateKey(['CommissionScheme', 'RecordStatus'])
      .NewButton({ action: () => this._commissionSchemeService.default() })
      .SaveButton({
        action: () => this._commissionSchemeService.save(this.bindFormValue()),
        validate: () => { makeFormTouched(this.form); return this.form.valid; }
      })
      .UpdateButton({
        action: () => this._commissionSchemeService.update(this.bindFormValue()),
        validate: () => { makeFormTouched(this.form); return this.form.valid; }
      })
      .DeleteButton({
        action: () => this._commissionSchemeService.delete(this.form.get('CommissionScheme').get('SysKey').value)
      })
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  loadData(param: { SysKey?: string, SchemeName?: string }): Promise<FormGroup> {

    return new Promise<any>((resolve, reject) => {

      let subscribe = param.SysKey ? this._commissionSchemeService.findByKey(param.SysKey) : this._commissionSchemeService.default();
      this._subsink.sink = subscribe.pipe(
        catchError((e) => {
          reject(e);
          return e;
        }),
        finalize(() => resolve(this.form))
      ).subscribe(data => {

        const c: any = createJsonFormGroup(data);
        this.form = this.fb.group(c);

        if (param.SchemeName)
          this.form.get('CommissionScheme').get('SchemeName').patchValue(param.SchemeName)

        this.form.get('CommissionScheme').get('SchemeName').setValidators([Validators.required]);
        this._fatch.next(this.form.get('CommissionSchemeDetail').value);

        this._subsink.sink = this.form.get('CommissionScheme').get('BaseHeaderSK').valueChanges
          .subscribe(item => {
            if (item == "01") {

              this._subsink.sink = this._commissionSchemeService.default()
                .subscribe(data => {

                  if (this.form.get('CommissionScheme').get('RecordStatus').value == 0) {
                    this._fatch.next(data['CommissionSchemeDetail']);
                  }

                  this._dataGrid.disabledAddNewButton = true;
                  this._dataGrid.disableDeleteButton = item => {
                    if (item.Type == 1 || item.Type == 2 || item.Type == 3) {
                      return true;
                    }
                  }

                  this.gridConfig.columns.filter(x => x.field == 'MinCom')[0].width = 0;
                  this.gridConfig.columns.filter(x => x.field == 'MaxCom')[0].width = 0;
                })
            }
            else if (item == "02") {

              if (this.form.get('CommissionScheme').get('RecordStatus').value == 0) {
                this._fatch.next([]);
              }

              this._dataGrid.disabledAddNewButton = false;
              this._dataGrid.disableDeleteButton = () => {
                return false;
              }

              this.gridConfig.columns.filter(x => x.field == 'MinCom')[0].width = 120;
              this.gridConfig.columns.filter(x => x.field == 'MaxCom')[0].width = 120;
            }
          })

        this.form.get('CommissionScheme').get('BaseHeaderSK').setValue(this.form.get('CommissionScheme').get('BaseHeaderSK').value);
      })
    })
  }

  public bindFormValue(): any {

    return {
      CommissionScheme: this.form.get('CommissionScheme').value,
      Inserted: this._dataGrid.service.data.filter(x => x['SysKey'] == null),
      Updated: this._dataGrid.service.data.filter(x => x['SysKey'] != null),
      Deleted: this._dataGrid.service.deletedItems,
    };
  }
}
