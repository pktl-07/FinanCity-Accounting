import { Component, OnInit, Input, Inject, OnDestroy } from '@angular/core';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { IndustryExamService } from 'src/@sales/services/industryexam.service';
import { QualificationService } from 'src/@sales/services/qualification.service';
import { DropDownFilterSettings } from '@progress/kendo-angular-dropdowns';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { WindowState } from '@progress/kendo-angular-dialog';
import { IndustryExamDetailComponent } from '../../industryexam-detail/industryexam-detail.component';
import { QualificationDetailComponent } from '../../qualification-detail/qualification-detail.component';
import { SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-salesperson-employment',
  templateUrl: './salesperson-employment.component.html',
  providers: [
    IndustryExamService,
    QualificationService,
    PWindowComponentService]
})
export class SalespersonEmploymentComponent implements OnInit, OnDestroy {

  @Input() formGroup: FormGroup;
  @Input() industry: FormControl;
  @Input() qualification: FormControl;

  _industryList = [];
  _qulificationList = [];

  _upTree = [];

  private _subsink = new SubSink();

  constructor(public fb: FormBuilder,
    public _windowService: PWindowComponentService,
    private _industryService: IndustryExamService,
    private _qualificationService: QualificationService) {
  }

  public filterSettings: DropDownFilterSettings = {
    caseSensitive: false,
    operator: 'contains'
  }

  ngOnInit(): void {

    this._subsink.sink = this._industryService.finds().subscribe(data => {
      this._industryList = data;
    });

    this._subsink.sink = this._qualificationService.finds().subscribe(data => {
      this._qulificationList = data;
    })

    this._industryService.asyncFinds();
    this._qualificationService.asyncFinds();

    this.formGroup.get('YearsOfExperience').setValidators([Validators.required]);
    this.formGroup.get('DateJoin').setValidators([Validators.required]);
  }

  private _filterText: string;
  handleFilterChange(value: string) {

    this._filterText = value;
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  public itemDisabled(itemArgs: { dataItem: any, index: number }) {
    return !itemArgs.dataItem.IsActive; //disable the 3rd item
  }

  get readonly() {
    return this.formGroup.get('RecordStatus').value > 1;
  }

  addNewIndustry() {

    let state: WindowState = 'default';
    let window = this._windowService.open({ title: 'Industry Setup', componentType: IndustryExamDetailComponent, state: state });
    let windowComponent = window.content.instance as IndustryExamDetailComponent;

    windowComponent.loadData({ Description: this._filterText });
    this._subsink.sink = windowComponent.OnSuccess
      .subscribe(data => {

        this._industryList.push(data.data);
        (this.industry.value as any[]).push(data.data);
        this._windowService.close();
      });
  }

  addNewEducation() {

    let state: WindowState = 'default';
    let window = this._windowService.open({ title: 'Qualification Setup', componentType: QualificationDetailComponent, state: state });
    let windowComponent = window.content.instance as QualificationDetailComponent;

    windowComponent.loadData({ Description: this._filterText });
    this._subsink.sink = windowComponent.OnSuccess
      .subscribe(data => {

        this._qulificationList.push(data.data);
        (this.qualification.value as any[]).push(data.data);
        this._windowService.close();
      });
  }
}
