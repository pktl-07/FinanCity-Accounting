import { Component, OnInit, ViewChild, OnDestroy, Inject } from '@angular/core';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BehaviorSubject } from 'rxjs';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { createJsonFormGroup, makeFormTouched, salespersonStatus, activeViewType, rankType, SubSink } from 'src/@common/service/utility-service';
import { ChangeOfStatusService } from 'src/@sales/services/change-of-status.service';
import { SalespersonManagementService } from 'src/@sales/services/salesperson-management.service';
import { map, mergeMap, finalize } from 'rxjs/operators';
import { ActivatedRoute, Router } from '@angular/router';
import { CommissionStructureDetailService } from 'src/@sales/services/commission-structure-detail.service';

@Component({
  selector: 'app-change-of-status-detail',
  templateUrl: './change-of-status-detail.component.html',
  providers: [
    ChangeOfStatusService,
    SalespersonManagementService,
    CommissionStructureDetailService,
    { provide: 'salesperson-1', useClass: SalespersonManagementService },
    { provide: 'salesperson-2', useClass: SalespersonManagementService }
  ]
})
export class ChangeOfStatusDetailComponent extends DetailFormBase implements OnInit, OnDestroy {

  @ViewChild(PToolbarComponent, { static: true }) private _toolbar: PToolbarComponent;

  _reasonToTerminate = new BehaviorSubject([]);
  _isActive = new BehaviorSubject(activeViewType());
  _rank = new BehaviorSubject(rankType());

  private _subsink = new SubSink();

  constructor(
    fb: FormBuilder,
    public _router: Router,
    private _activatedRoute: ActivatedRoute,
    private _changeOfStatusService: ChangeOfStatusService,
    private _commissionStructure: CommissionStructureDetailService,
    @Inject('salesperson-1') public _salespersonManagementService1: SalespersonManagementService,
    @Inject('salesperson-2') public _salespersonManagementService2: SalespersonManagementService,
  ) {

    super(fb);
  }

  ngOnInit() {

    this._toolbar
      .controls
      .Success(this.OnSuccess)
      .Ready(this.formReady)
      .StateKey(['RecordStatus'])
      .NavigationButton({ action: () => this._router.navigateByUrl('/Management/SalespersonManagement/ChangeOfStatus') })
      .NewButton({
        action: () =>
          this._changeOfStatusService.default()
            .pipe(
              finalize(() => {
                this.form.get('NRIC').patchValue(null);
                this.form.get('Designation').patchValue(null);
              })
            )
      })
      .SaveButton({
        action: () =>
          this._changeOfStatusService.save(this.form.value),
        validate: () => {
          makeFormTouched(this.form);
          return this.form.valid
        }
      });

    this._subsink.sink = this._activatedRoute.data
      .subscribe((data) => {
        this.loadData(data['data']);
      });

    this._salespersonManagementService1.asyncFinds();
    this._salespersonManagementService2.asyncFinds();
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }


  private fetchReasonToTerminateSrouce(value: boolean) {

    let status = [];
    if (value)
      status = salespersonStatus().filter(x => x.value < 10);
    else
      status = salespersonStatus().filter(x => x.value > 5);

    return status;
  }

  loadData(data: any): Promise<FormGroup> {

    return new Promise<FormGroup>((resolve, reject) => {
      try {

        const c: any = createJsonFormGroup(data);
        this.form = this.fb.group(c);
        this._reasonToTerminate.next(this.fetchReasonToTerminateSrouce(this.form.get('IsActive').value));

        this._subsink.sink = this.form.get('SPSK').valueChanges
          .subscribe(value => {

            if (value != null) {

              this._subsink.sink = this._salespersonManagementService1.findByKey(value)
                .pipe(
                  map(response => {

                    response['NRIC'] = response['Person']['NRIC'];
                    response['Rank'] = response['Employment']['Rank'];
                    response['T1'] = response['Employment']['L1'];

                    return response;
                  }),
                  mergeMap(response => {
                    return this._commissionStructure.findByKey(response['Employment']['SchemeDetailSysKey'])
                      .pipe(map(data => {

                        response['SchemeDetailSK'] = data['SysKey'];
                        response['Designation'] = data['Designation'];

                        return response;
                      }))
                  })
                ).subscribe((data: any) => {

                  this.form.get('Rank').patchValue(data['Rank']);
                  this.form.get('SchemeDetailSK').patchValue(data['SchemeDetailSK']);
                  this.form.get('NRIC').patchValue(data['NRIC']);
                  this.form.get('Designation').patchValue(data['Designation']);
                  this.form.get('T1').patchValue(data['T1']);
                })
            }
          });

        this._subsink.sink = this.form.get('IsActive').valueChanges
          .subscribe(value => {

            let status = this.fetchReasonToTerminateSrouce(value);

            this._reasonToTerminate.next(status);
            this.form.get('ReasonToTerminate').patchValue(status[0].value);
          });

        this.form.get('SPSK').setValidators([Validators.required]);
        this.form.get('CreateDate').setValidators([Validators.required]);
        this.form.get('ApprovedBy').setValidators([Validators.required]);
        this.form.get('EffectiveDate').setValidators([Validators.required]);

        resolve(null);

      } catch (error) {
        reject(error)
      }
    })
  }

  public itemDisabled(itemArgs: { dataItem: any, index: number }) {

    if (typeof itemArgs.dataItem.IsActive != 'undefined')
      return !itemArgs.dataItem.IsActive;

    return false;
  }
}
