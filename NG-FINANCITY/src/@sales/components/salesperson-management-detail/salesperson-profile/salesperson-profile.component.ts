import { Component, OnInit, Input, OnDestroy, ViewChild } from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { DepartmentService } from 'src/@accounts/services/department.service';
import { DropDownFilterSettings } from '@progress/kendo-angular-dropdowns';
import { WindowState } from '@progress/kendo-angular-dialog';
import { DepartmentDetailComponent } from '../../department-detail/department-detail.component';
import { BehaviorSubject } from 'rxjs';
import { VendorService } from 'src/@sales/services/vendor.service';
import { FranchiseeManagementDetailComponent } from '../../franchisee-management-detail/franchisee-management-detail.component';
import { salutation, martialStatus, gender, kendoFilter, salespersonStatus, rankType, SubSink } from 'src/@common/service/utility-service';
import { PImageUploaderComponent } from 'src/@common/components/p-image-uploader/p-image-uploader.component';

@Component({
  selector: 'app-salesperson-profile',
  templateUrl: './salesperson-profile.component.html',
  providers: [
    VendorService,
    DepartmentService,
    PWindowComponentService
  ]
})
export class SalespersonProfileComponent implements OnInit, OnDestroy {

  @Input() formGroup: FormGroup;
  @Input() rank: number;
  @ViewChild(PImageUploaderComponent, { static: true }) _imageUploader: PImageUploaderComponent;
  @Input() clearImage = new BehaviorSubject<boolean>(false);

  _salutation = [];
  _martialStatus = [];
  _gender = [];
  _reasonForTerminate = new BehaviorSubject([]);

  private _subsink = new SubSink();

  constructor(
    public _vendorService: VendorService,
    public _departmentService: DepartmentService,
    public _windowService: PWindowComponentService) {
  }

  ngOnInit(): void {

    this._subsink.sink = this.clearImage
      .subscribe((value) => {
        if (value)
          this._imageUploader.clear();
      })

    this._reasonForTerminate.next(this.fetchReasonToTerminateSrouce(this.formGroup.get('IsActive').value))

    this._salutation = salutation();
    this._martialStatus = martialStatus();
    this._gender = gender();

    this._departmentService.asyncFinds();
    this.asyncFranchiseeList();

    this._subsink.sink = this.formGroup.get('IsActive').valueChanges
      .subscribe(value => {

        let status = this.fetchReasonToTerminateSrouce(value);
        this._reasonForTerminate.next(status);
        this.formGroup.get('ReasonToTerminate').patchValue(status[0].value);
      });

    this.formGroup.get('Name').setValidators([Validators.required]);
    this.formGroup.get('NRIC').setValidators([Validators.required]);
  }

  fetchReasonToTerminateSrouce(value: boolean) {

    let status = [];
    if (value)
      status = salespersonStatus().filter(x => x.value < 10);
    else
      status = salespersonStatus().filter(x => x.value > 5);

    return status;
  }

  asyncFranchiseeList() {

    this._vendorService.asyncFinds({ // franchisee
      filter: {
        logic: 'and',
        filters: kendoFilter([{ field: 'TransType', value: '6', logic: 'and', operator: 'eq' }])
      }
    });
  }

  rankDescription() {
    return rankType().find(x => x.value == this.rank).text;
  }

  filterSettings: DropDownFilterSettings = {
    caseSensitive: false,
    operator: 'contains'
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  private _franchiseeFilterText: string;
  onHandleFranchiseeFilterChange(value: string) {

    this._franchiseeFilterText = value;
  }

  addNewFranchisee() {

    let state: WindowState = 'default';
    let window = this._windowService.open({ title: 'Franchise Management', componentType: FranchiseeManagementDetailComponent, state: state });
    let windowComponent = window.content.instance as FranchiseeManagementDetailComponent;
    windowComponent.loadData({ FranchiseeName: this._franchiseeFilterText });

    this._subsink.sink = windowComponent.OnSuccess
      .subscribe(item => {

        this.asyncFranchiseeList();
        this.formGroup.get('FranchiseeSK').setValue(item.data.Vendor.SysKey);

        this._windowService.close();
      });
  }

  private _divisionFilterText: string;
  onDivisionFilterChange(value: string) {

    this._divisionFilterText = value;
  }

  addNewDepartment() {

    let state: WindowState = 'default';
    let window = this._windowService.open({ title: 'Division Setup', componentType: DepartmentDetailComponent, state: state });
    let windowComponent = window.content.instance as DepartmentDetailComponent;
    windowComponent.loadData({ Description: this._divisionFilterText });

    this._subsink.sink = windowComponent.OnSuccess
      .subscribe(item => {

        this._departmentService.asyncFinds();
        this.formGroup.get('DepartmentSK').setValue(item.data.SysKey);

        this._windowService.close();
      });
  }
}
