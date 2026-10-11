import { Directive, AfterContentInit, OnInit, OnDestroy, Input } from '@angular/core';
import { IonInput } from '@ionic/angular';
import { NgControl, NgModel } from '@angular/forms';
import { SubSink } from '../utils';

@Directive({
    host: {
        '(ionBlur)': 'handleBlur($event)',
        '(ionFocus)': 'handleFocus($event)',
        '(ionInput)': 'handleInput($event)',
    },
    selector: '[fcCurrency]',
    providers: [],
})
export class FCCurrencyFormatDirective implements OnInit, AfterContentInit, OnDestroy {

    @Input('nullable') public isNullable: boolean = false;
    private _subsink = new SubSink();

    constructor(private _ngControl: NgControl,
        private _ionInput: IonInput) {

        if (_ngControl instanceof NgModel)
            _ngControl.options.updateOn = 'blur';
    }

    ngOnInit(): void {

        this.tranfrom();

        this._subsink.sink = this._ngControl.valueChanges
            .subscribe(() => {

                try {
                    this._ngControl.control.patchValue(this.money,
                        { emitModelToViewChange: false, emitViewToModelChange: true, emitEvent: false });

                    this.tranfrom();
                } catch (error) {
                    console.error('Error occurs while transforming currency into custom format.')
                }
            });
    }

    ngOnDestroy(): void {

        this._subsink.unsubscribe();
    }

    ngAfterContentInit(): void {

        this._ionInput.inputmode = "decimal";
    }

    handleFocus(e: any) {

        e.target.value = this.money;
        e.target.type = "number";
    }

    handleBlur(e: any) {

        if (this.isNullable == false && (e.target.value == null || e.target.value == ''))
            this._ngControl.control.patchValue(0);

        e.target.type = "text";

        this.tranfrom();
    }

    handleInput(e: any) {

        e.target.value = Number(e.target.value.replace(/[^0-9.-]+/g, ""));
    }

    private tranfrom() {

        const formatter = new Intl.NumberFormat('en-us', {
            style: 'currency',
            currency: 'USD',
            minimumFractionDigits: 2,
        })

        if (this.money != null) {

            this._ionInput.type = 'text';
            this._ionInput.value = formatter.format(this.money);
        }
    }

    private get money() {

        let value: any;
        if (this._ionInput.value === undefined || this._ionInput.value == null || this._ionInput.value == '')
            if (this.isNullable == false)
                value = 0;
            else
                value = null;
        else
            value = this._ionInput.value;

        if (value != null) {
            const f: string = value.toString().replace(/[^0-9.-]+/g, "");
            value = Math.round(+f * 100) / 100;
        }

        return value;
    }
}
