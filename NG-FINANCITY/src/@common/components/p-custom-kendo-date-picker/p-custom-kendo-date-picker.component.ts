import { Component, forwardRef, Input, OnInit, Injector, AfterContentInit, Optional, SkipSelf, Host, OnDestroy, Output, EventEmitter } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, NgControl, AbstractControl, FormGroup, FormBuilder, ControlContainer, FormControl } from '@angular/forms';
import { IntlService } from '@progress/kendo-angular-intl';
import { SubSink, transformDate } from 'src/@common/service/utility-service';

@Component({
  selector: 'p-custom-kendo-date-picker',
  template: `
          <kendo-datepicker [formControl] ="form?.get('dataValue')"
            (blur)="onTouched()"
            (valueChange)="onValueChange($event)"
            [format]="'dd-MM-yyyy'" 
            [disabled]="disabled" 
            [readonly]="readonly"
            class="form-control">
          </kendo-datepicker>
          `, providers: [{
    provide: NG_VALUE_ACCESSOR, useExisting: forwardRef(() => PCustomKendoDatePickerComponent), multi: true
  }]
})
export class PCustomKendoDatePickerComponent implements OnInit, ControlValueAccessor, AfterContentInit, OnDestroy {

  onChange = (_: any) => { };
  onTouched = () => { };
  @Input() disabled: boolean = false;
  @Input() readonly: boolean = false;
  @Output() blur = new EventEmitter();

  form: FormGroup;
  control: AbstractControl;

  private _subsink = new SubSink();

  constructor(public _intl: IntlService,
    private _injector: Injector,
    private _fb: FormBuilder,
    @Optional() @Host() @SkipSelf() private _controlContainer: ControlContainer) { }

  ngOnInit(): void {

    this.form = this._fb.group({
      dataValue: null
    })
  }

  ngAfterContentInit(): void {

    const ngControl: NgControl = this._injector.get(NgControl, null);

    if (this._controlContainer && ngControl) {
      this.control = this._controlContainer.control.get(<string>ngControl.name);
      if (this.control && this.control.validator) {

        this.form.get('dataValue').setValidators(this.control.validator);
        this._subsink.sink = (this.control as FormControl).statusChanges
          .subscribe(item => {
            if (item == 'TOUCHED') {
              this.form.get('dataValue').markAsTouched();
            }
          });
      }
    }
  }

  ngOnDestroy(): void {

    this._subsink.unsubscribe();
  }

  writeValue(value: any): void {

    this.form.get('dataValue').patchValue(transformDate(value))
  }

  registerOnChange(fn: (_: any) => void): void {

    this.onChange = fn;
  }

  onValueChange(value: any) {

    const date = parseInt(this._intl.formatDate(value, 'yyyyMMdd'));
    if (value == null || isNaN(date))
      this.onChange(null)
    else
      this.onChange(date);
  }

  registerOnTouched(fn: any): void {

    this.onTouched = fn;
    this.blur.emit();
  }

  setDisabledState?(isDisabled: boolean): void {

    this.disabled = isDisabled;
  }
}



