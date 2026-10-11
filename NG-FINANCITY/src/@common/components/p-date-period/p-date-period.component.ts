import { Component, OnInit, OnDestroy, Output, EventEmitter, Input, Injector, Optional, Host, SkipSelf, forwardRef } from '@angular/core';
import { FormGroup, FormBuilder, FormControl, Validators, ControlValueAccessor, ControlContainer, AbstractControl, NgControl, NG_VALUE_ACCESSOR } from '@angular/forms';
import { DatePeriodService } from './date-period.service';
import { makeFormTouched, createJsonFormGroup, SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'p-date-period',
  template: `
  
  <div [formGroup]="form">
  
    <div class="form-inline mb-3" formGroupName="Monthly">
      <div class="form-check mr-0 form-check-inline col-md-3 d-flex justify-content-end">
        <label id="chk_monthly" class="form-check-label">
          Monthly&nbsp;
          <input type="checkbox" [attr.disabled]="disabled ? true : null" id="chk_monthly" formControlName="selected" /> 
        </label>
      </div>
      <kendo-dropdownlist class="col-md-4" [disabled]="disabled" (blur)="onTouched()" [data]="months" [valueField]="'value'" formControlName="item1" [textField]="'text'" [valuePrimitive]="true"></kendo-dropdownlist>
      <kendo-dropdownlist class="col-md-4" [disabled]="disabled" (blur)="onTouched()" [data]="years" [valueField]="'value'" formControlName="item2" [textField]="'text'" [valuePrimitive]="true"></kendo-dropdownlist>
    </div>

    <div class="form-inline mb-3" formGroupName="Quarterly">
      <div class="form-check mr-0 form-check-inline col-md-3 d-flex justify-content-end">
        <label id="chk_quarterly" class="form-check-label">
        Quarterly&nbsp;
          <input type="checkbox" [attr.disabled]="disabled ? true : null" id="chk_quarterly" formControlName="selected" /> 
        </label>
      </div>
      <kendo-dropdownlist class="col-md-4" [disabled]="disabled" (blur)="onTouched()" [data]="quarters" [valueField]="'value'" formControlName="item1" [textField]="'text'" [valuePrimitive]="true"></kendo-dropdownlist>
      <kendo-dropdownlist class="col-md-4" [disabled]="disabled" (blur)="onTouched()" [data]="years" [valueField]="'value'" formControlName="item2" [textField]="'text'" [valuePrimitive]="true"></kendo-dropdownlist>
    </div>

    <div class="form-inline mb-3" formGroupName="HalfYear">
      <div class="form-check mr-0 form-check-inline col-md-3 d-flex justify-content-end">
        <label id="chk_halfyear" class="form-check-label">
        Half Yearly&nbsp;
          <input type="checkbox" [attr.disabled]="disabled ? true : null" id="chk_halfyear" formControlName="selected" /> 
        </label>
      </div>
      <kendo-dropdownlist class="col-md-4" [disabled]="disabled" (blur)="onTouched()" [data]="halfs" [valueField]="'value'" formControlName="item1" [textField]="'text'" [valuePrimitive]="true"></kendo-dropdownlist>
      <kendo-dropdownlist class="col-md-4" [disabled]="disabled" (blur)="onTouched()" [data]="years" [valueField]="'value'" formControlName="item2" [textField]="'text'" [valuePrimitive]="true"></kendo-dropdownlist>
    </div>

    <div class="form-inline mb-3" formGroupName="Yearly">
      <div class="form-check mr-0 form-check-inline col-md-3 d-flex justify-content-end">
        <label id="chk_yearly" class="form-check-label">
        Yearly&nbsp;
          <input type="checkbox" [attr.disabled]="disabled ? true : null" id="chk_yearly" formControlName="selected" /> 
        </label>
      </div>
      <kendo-dropdownlist class="col-md-4" [disabled]="disabled" (blur)="onTouched()" [data]="years" [valueField]="'value'" formControlName="item1" [textField]="'text'" [valuePrimitive]="true"></kendo-dropdownlist>
    </div>

    <div class="form-inline" formGroupName="DateRange">
      <div class="form-check mr-0 form-check-inline col-md-3 d-flex justify-content-end">
        <label id="chk_daterange" class="form-check-label">
        Date Rage&nbsp;
          <input type="checkbox" [attr.disabled]="disabled ? true : null" id="chk_daterange" formControlName="selected" /> 
        </label>
      </div>
      <p-custom-kendo-date-picker class="col-md-4" [disabled]="disabled" (blur)="onTouched()" formControlName="item1"></p-custom-kendo-date-picker>
      <p-custom-kendo-date-picker class="col-md-4" [disabled]="disabled" (blur)="onTouched()" formControlName="item2"></p-custom-kendo-date-picker>
    </div>
   
  </div>
  `,
  styles: [`
   .form-check-label{
      font-weight:unset;
    }
  `],

  providers: [DatePeriodService,
    { provide: NG_VALUE_ACCESSOR, useExisting: forwardRef(() => PDatePeriodComponent), multi: true }
  ]
})
export class PDatePeriodComponent implements ControlValueAccessor, OnInit, OnDestroy {

  private _subsink = new SubSink();

  onChange = (_: any) => { };
  onTouched = () => { };

  @Input() disabled: boolean = false;
  @Input() readonly: boolean = false;

  @Output() valueChange: EventEmitter<any> = new EventEmitter();

  form: FormGroup;
  control: AbstractControl;

  startYear = 0;
  currentYear = 0;
  years = [];
  months = [];
  quarters = [];
  halfs = [];

  private _group = ['Monthly', 'Quarterly', 'HalfYear', 'Yearly', 'DateRange'];

  constructor(public fb: FormBuilder,
    private _dateRangeService: DatePeriodService,
    private _injector: Injector,
    @Optional() @Host() @SkipSelf() private _controlContainer: ControlContainer) {

    this.startYear = _dateRangeService.startYear;
  }

  /**
   * Always retun null unless UI touch.
   * @param value default value.
   */
  writeValue(value: any): void {
    if (value == null) {
      this.form.get('Yearly').get('selected').patchValue(true);
    }
  }

  registerOnChange(fn: any): void {

    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {

    this.onTouched = fn;
  }

  setDisabledState?(isDisabled: boolean): void {

    this.disabled = isDisabled;
  }

  ngOnInit(): void {

    this.currentYear = this._dateRangeService.currentYear;
    this.months = this._dateRangeService.months;
    this.quarters = this._dateRangeService.quarters;
    this.halfs = this._dateRangeService.halfs
    this.years = this._dateRangeService.years();

    this.poppulateFormGroup();

    /**Monthly value change */
    this._subsink.sink = this.form.get('Monthly').get('selected').valueChanges
      .subscribe(value => {
        if (value == false)
          this.setUnChangeState('Monthly')
        else
          this.setDisableState('Monthly');

        this.emitMonthlyValueChange();
      })

    this._subsink.sink = this.form.get('Monthly').get('item1').valueChanges
      .subscribe(() => {
        this.emitMonthlyValueChange();
      })

    this._subsink.sink = this.form.get('Monthly').get('item2').valueChanges
      .subscribe(() => {
        this.emitMonthlyValueChange();
      })

    /**Quarterly value change */
    this._subsink.sink = this.form.get('Quarterly').get('selected').valueChanges
      .subscribe(value => {
        if (value == false)
          this.setUnChangeState('Quarterly')
        else
          this.setDisableState('Quarterly');

        this.emitQuarterlyValueChange();
      })

    this._subsink.sink = this.form.get('Quarterly').get('item1').valueChanges
      .subscribe(() => {
        this.emitQuarterlyValueChange();
      })

    this._subsink.sink = this.form.get('Quarterly').get('item2').valueChanges
      .subscribe(() => {
        this.emitQuarterlyValueChange();
      })

    /**HalfYear value change */
    this._subsink.sink = this.form.get('HalfYear').get('selected').valueChanges
      .subscribe(value => {
        if (value == false)
          this.setUnChangeState('HalfYear')
        else
          this.setDisableState('HalfYear');

        this.emitHalfYearValueChange();
      })

    this._subsink.sink = this.form.get('HalfYear').get('item1').valueChanges
      .subscribe(() => {
        this.emitHalfYearValueChange();
      })

    this._subsink.sink = this.form.get('HalfYear').get('item2').valueChanges
      .subscribe(() => {
        this.emitHalfYearValueChange();
      })

    /**Yearly value change */
    this._subsink.sink = this.form.get('Yearly').get('selected').valueChanges
      .subscribe(value => {

        if (value == false)
          this.setUnChangeState('Yearly')
        else
          this.setDisableState('Yearly');

        this.emitYearlyValueChange();
      })

    this._subsink.sink = this.form.get('Yearly').get('item1').valueChanges
      .subscribe(() => {
        this.emitYearlyValueChange();
      })

    /**DateRange value change */
    this._subsink.sink = this.form.get('DateRange').get('selected').valueChanges
      .subscribe(value => {
        if (value == false)
          this.setUnChangeState('DateRange')
        else
          this.setDisableState('DateRange');

        this.emitDateRangeValueChange();
      })

    this._subsink.sink = this.form.get('DateRange').get('item1').valueChanges
      .subscribe(() => {
        this.emitDateRangeValueChange();
      })

    this._subsink.sink = this.form.get('DateRange').get('item2').valueChanges
      .subscribe(() => {
        this.emitDateRangeValueChange();
      })

    //set default value
    //this.form.get('Yearly').get('selected').patchValue(true);
    this.form.get('DateRange').get('item1').setValidators([Validators.required]);
    this.form.get('DateRange').get('item2').setValidators([Validators.required]);
  }

  ngAfterContentInit(): void {

    const ngControl: NgControl = this._injector.get(NgControl, null);

    if (this._controlContainer && ngControl) {
      this.control = this._controlContainer.control.get(<string>ngControl.name);
      if (this.control) {
        this._subsink.sink = (this.control as FormControl).statusChanges
          .subscribe(item => {
            if (item == 'TOUCHED') {
              makeFormTouched(this.form);
            }
          });
      }
    }
  }

  /**
   * Unregister subscrib value.
   */
  ngOnDestroy(): void {

    this._subsink.unsubscribe();
  }

  /**
   * Emit monthly value change;
   */
  private emitMonthlyValueChange() {

    const value = this._dateRangeService.transalateMonthly(
      this.form.get('Monthly').get('item1').value,
      this.form.get('Monthly').get('item2').value,
    );

    this.valueChange.emit(value);
    this.onChange(value);
  }

  /**
   * Emit quarterly value change.
   */
  private emitQuarterlyValueChange() {

    const value = this._dateRangeService.transalateQuarterly(
      this.form.get('Quarterly').get('item1').value,
      this.form.get('Quarterly').get('item2').value,
    )

    this.valueChange.emit(value);
    this.onChange(value);
  }

  /**
   * Emit half value change.
   */
  private emitHalfYearValueChange() {

    const value = this._dateRangeService.transalteHalfYear(
      this.form.get('HalfYear').get('item1').value,
      this.form.get('HalfYear').get('item2').value,
    )

    this.valueChange.emit(value);
    this.onChange(value);
  }

  /**
   * Emit yearly value change.
   */
  private emitYearlyValueChange() {

    const value = this._dateRangeService.transalateYearly(
      this.form.get('Yearly').get('item1').value);

    this.valueChange.emit(value);
    this.onChange(value);
  }

  /**
   * Emit date range value change.
   */
  private emitDateRangeValueChange() {

    const value = this._dateRangeService.transalateDateRange(
      this.form.get('DateRange').get('item1').value,
      this.form.get('DateRange').get('item2').value,
    );

    this.valueChange.emit(value);
    this.onChange(value);
  }

  /**
   * Prevent uncheck for check value.
   * @param value form group name
   */
  private setUnChangeState(value: 'Monthly' | 'Quarterly' | 'HalfYear' | 'Yearly' | 'DateRange') {

    this.form.get(value).get('selected').patchValue(true, { emitEvent: false });
  }

  /**
   * Toggle disable state and enable state.
   * @param value form group name
   */
  private setDisableState(value: 'Monthly' | 'Quarterly' | 'HalfYear' | 'Yearly' | 'DateRange') {

    this._group.filter(x => x != value).map(m => {

      this.form.get(m).get('item1').disable({ emitEvent: false });

      if (m != value)
        this.form.get(m).get('selected').patchValue(false, { emitEvent: false });

      if (m != 'Yearly')
        this.form.get(m).get('item2').disable({ emitEvent: false });
    })

    this.form.get(value).get('item1').enable({ emitEvent: false });

    if (value != 'Yearly')
      this.form.get(value).get('item2').enable({ emitEvent: false });
  }

  /**
   * Poppulate form controls and set default value.
   */
  private poppulateFormGroup() {

    this.form = this.fb.group(createJsonFormGroup({
      Monthly: { selected: false, item1: 1, item2: this.currentYear },
      Quarterly: { selected: false, item1: 3, item2: this.currentYear },
      HalfYear: { selected: false, item1: 6, item2: this.currentYear },
      Yearly: { selected: false, item1: this.currentYear },
      DateRange: { selected: false, item1: `${this.currentYear}0101`, item2: `${this.currentYear}1231` }
    }));
  }
}



