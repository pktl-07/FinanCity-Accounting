import { Component, forwardRef, Input, OnInit, Injector, AfterContentInit, Optional, SkipSelf, Host, OnDestroy, Output, EventEmitter } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, NgControl, AbstractControl, FormGroup, FormBuilder, ControlContainer, FormControl } from '@angular/forms';
import { SubSink } from 'src/@common/service/utility-service';


@Component({
    selector: 'p-custom-kendo-time-picker',
    template: `
          <kendo-timepicker [formControl] ="form?.get('timeValue')"
            (blur)="onTouched()"
            (valueChange)="onValueChange($event)"
            [format]="'hh:mm a'" 
            [disabled]="disabled" 
            [readonly]="readonly"
            class="form-control">
          </kendo-timepicker>
          `, providers: [{
        provide: NG_VALUE_ACCESSOR, useExisting: forwardRef(() => PCustomKendoTimePickerComponent), multi: true
    }]
})
export class PCustomKendoTimePickerComponent implements OnInit, ControlValueAccessor, AfterContentInit, OnDestroy {

    onChange = (_: any) => { };
    onTouched = () => { };
    @Input() disabled: boolean = false;
    @Input() readonly: boolean = false;
    @Output() blur = new EventEmitter();

    form: FormGroup;
    control: AbstractControl;

    private _subsink = new SubSink();

    constructor(private _injector: Injector,
        private _fb: FormBuilder,
        @Optional() @Host() @SkipSelf() private _controlContainer: ControlContainer) { }

    ngOnInit(): void {

        this.form = this._fb.group({
            timeValue: null
        })
    }

    ngAfterContentInit(): void {

        const ngControl: NgControl = this._injector.get(NgControl, null);

        if (this._controlContainer && ngControl) {
            this.control = this._controlContainer.control.get(<string>ngControl.name);
            if (this.control && this.control.validator) {

                this.form.get('timeValue').setValidators(this.control.validator);
                this._subsink.sink = (this.control as FormControl).statusChanges
                    .subscribe(item => {
                        if (item == 'TOUCHED') {
                            this.form.get('timeValue').markAsTouched();
                        }
                    });
            }
        }
    }

    ngOnDestroy(): void {

        this._subsink.unsubscribe();
    }

    writeValue(value: any): void {

        this.form.get('timeValue').patchValue(this.transformTime(value))
    }

    registerOnChange(fn: (_: any) => void): void {

        this.onChange = fn;
    }

    onValueChange(value: any) {

        if (value == null) {
            this.onChange(null)
        }
        else if (value && !(value instanceof Date)) {
            this.onChange(null)
        }
        else {
            this.onChange(`${this.prefixZero(value?.getHours(), 2)}${this.prefixZero(value?.getMinutes(), 2)}`);
        }
    }

    registerOnTouched(fn: any): void {

        this.onTouched = fn;
        this.blur.emit();
    }

    setDisabledState?(isDisabled: boolean): void {

        this.disabled = isDisabled;
    }

    private transformTime(time: string): Date {

        if (time == null)
            return null;

        const h = +time.substr(0, 2);
        const m = +time.substr(2, 2);
        const d = new Date()

        return new Date(d.getFullYear(), d.getMonth() + 1, d.getDate(), h, m, 0);
    }

    private prefixZero(source: any, digit: number) {

        return ("0" + (parseInt(source))).slice(-1 * digit);
    }
}



