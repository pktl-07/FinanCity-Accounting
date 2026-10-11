import { Directive, AfterContentInit, OnInit, OnDestroy, Input } from '@angular/core';
import { IonInput } from '@ionic/angular';
import { NgControl, NgModel } from '@angular/forms';
import { CurrencyPipe } from '@angular/common';
import { SubSink } from '../utils';

@Directive({
    host: {
        '(ionBlur)': 'handleBlur($event)',
        '(ionFocus)': 'handleFocus($event)',
        '(ionInput)': 'handleInput($event)',
    },
    selector: '[fcDecimal]',
    providers: [CurrencyPipe],
})
export class FCDecimalFormatDirective implements OnInit, AfterContentInit, OnDestroy {

    @Input('nullable') public isNullable: boolean = false;
    @Input('fraction') fraction: number = 2;

    private _subsink = new SubSink();

    constructor(
        private _ionInput: IonInput,
        private _ngControl: NgControl) {

        if (_ngControl instanceof NgModel) {
            _ngControl.options.updateOn = 'blur';
        }
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
                    console.error('Error occurs while transforming decimal into custom format.')
                }
            });
    }

    ngOnDestroy(): void {

        this._subsink.unsubscribe();
    }

    ngAfterContentInit(): void {

        if (this.fraction > 0)
            this._ionInput.inputmode = "decimal";
    }

    /**
     * To transform currency digit number eg. $123.00 to 123.00  
     * @param e IoincInputEventArgs
     */
    handleFocus(e: any) {

        e.target.value = this.money;
        e.target.type = "number";
    }

    /**
     * To transform currency format eg. 123.00 to $123.00
     * @param e IoincInputEventArgs
     */
    handleBlur(e: any) {

        if (this.isNullable == false && (e.target.value == null || e.target.value == ''))
            this._ngControl.control.patchValue(0);

        e.target.type = "text";
        this.tranfrom();
    }

    /**
     * To elimate first digit number zero eg. 0123 to 123
     * @param e IoincInputEventArgs
     */
    handleInput(e: any) {

        e.target.value = Number(e.target.value.replace(/[^0-9.-]+/g, ""));
    }

    /**
     * To transform currency format eg. 1,234.44
     */
    private tranfrom() {

        const formatter = new Intl.NumberFormat('en-us', {
            style: 'decimal',
            currency: 'USD',
            minimumFractionDigits: this.fraction
        })

        if (this.money != null) {

            this._ionInput.type = 'text';
            this._ionInput.value = formatter.format(this.money);
        }
    }

    /**
     * To get currency digit number eg. as 1,234.125 to 1234.13
     */
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
            let f = +(value.toString().replace(/[^0-9.-]+/g, ""));
            if (value != null) {

                switch (this.fraction) {
                    case 1:
                        value = Math.round(f * 10) / 10;
                        break;
                    case 2:
                        value = Math.round(f * 100) / 100;
                        break;
                    case 3:
                        value = Math.round(f * 1000) / 1000;
                        break;
                    case 4:
                        value = Math.round(f * 10000) / 10000;
                        break;
                }
            }
        }
        return value;
    }
}
