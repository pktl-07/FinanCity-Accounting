import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { FormBuilder } from '@angular/forms';
import { SalespersonManagementService } from 'src/@sales/services/salesperson-management.service';
import { Observable, BehaviorSubject } from 'rxjs';
import { finalize, catchError, map } from 'rxjs/operators';
import { InternationalNameSpliterService } from 'src/@common/service/internation.name.splitter.service';
import { makeFormTouched, createJsonFormGroup, SubSink } from 'src/@common/service/utility-service';
import { ActivatedRoute, Router } from '@angular/router';
import { PModalFormComponent } from 'src/@common/components/p-modal-form/p-modal-form.component';
import { TabStripComponent } from '@progress/kendo-angular-layout';

@Component({
  selector: 'app-salesperson-management-detail',
  templateUrl: './salesperson-management-detail.component.html',
  providers: [
    SalespersonManagementService,
    InternationalNameSpliterService
  ]
})
export class SalespersonManagementDetailComponent extends DetailFormBase implements OnInit, OnDestroy {

  //should static is turn off when it is inside the nested *ngIf.
  @ViewChild('tabstrip', { read: TabStripComponent }) private _tabstrip: TabStripComponent;
  @ViewChild(PModalFormComponent, { static: true }) public _modalForm: PModalFormComponent;
  private _subsink = new SubSink()

  clearImageSubject = new BehaviorSubject(false);

  constructor(fb: FormBuilder,
    public _router: Router,
    public _activeRoute: ActivatedRoute,
    public _nameConventionService: InternationalNameSpliterService,
    public _salespersonManagementService: SalespersonManagementService) {

    super(fb);
  }

  ngOnInit() {

    this._modalForm
      .controls
      .Ready(this.formReady)
      .Success(this.OnSuccess)
      .StateKey(['Person', 'RecordStatus'])
      .NewButton({
        action: () => this._salespersonManagementService.default().pipe(
          map((response) => {
            response['Employment']['SalespersonCommScheme'] = null;
            return response;
          }), finalize(() => {
            this.clearImageSubject.next(true);
            this._tabstrip.selectTab(0);
          })
        )
      })
      .SaveButton({
        action: () => this._salespersonManagementService.save(this.form.value),
        validate: () => {
          makeFormTouched(this.form);
          return this.form.valid;
        }
      })
      .UpdateButton({
        action: () => this._salespersonManagementService.update(this.form.value),
        validate: () => {
          makeFormTouched(this.form);
          return this.form.valid;
        }
      })
      .DeleteButton({
        action: () => this._salespersonManagementService.delete(this.form.value.Person.SysKey).pipe(
          finalize(() => {

            this.clearImageSubject.next(true);
          })
        )
      })
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  loadData(param: { SysKey?: string, Name?: string }): Promise<any> {

    return new Promise<any>((resolve, reject) => {

      let subscribe: Observable<any>;

      if (param.SysKey)
        subscribe = this._salespersonManagementService.findByKey(param.SysKey);
      else
        subscribe = this._salespersonManagementService.default();

      this._subsink.sink = subscribe.pipe(
        finalize(() => { resolve(this.form) }),
        catchError((error) => {
          reject(error);
          return error
        }))
        .subscribe((data: any) => {

          const c = createJsonFormGroup(data);
          this.form = this.fb.group(c);

          if (param.Name)
            this.form.get('Person').get('Name').patchValue(param.Name)

          this._nameConventionService.register({
            sourceName: this.form.get('Person').get('Name'),
            fristName: this.form.get('Person').get('FirstName'),
            lastName: this.form.get('Person').get('LastName'),
            isLastNameFirst: this.form.get('Person').get('IsLastNameFirst'),
          });

          this._nameConventionService.watch();
        })
    });
  }
}
