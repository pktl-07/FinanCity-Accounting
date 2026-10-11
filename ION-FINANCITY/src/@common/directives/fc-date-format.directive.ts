import { Directive, AfterContentInit, OnInit, OnDestroy } from '@angular/core';
import { IonDatetime } from '@ionic/angular';
import { NgControl } from '@angular/forms';
import { SubSink } from '../utils';

@Directive({
    selector: '[fcDate]',
})
export class FCDateFormatDirective implements OnInit, AfterContentInit, OnDestroy {

    private _subsink = new SubSink();

    constructor(private _ngControl: NgControl,
        private _ionDate: IonDatetime) {
    }

    ngOnInit(): void {

        this._subsink.sink = this._ngControl.valueChanges
            .subscribe((value) => {
                if (value) {

                    try {
                        const date = this.splitDate(value)
                        this._ngControl.control.setValue(`${date.year}${date.month}${date.day}`,
                            { emitModelToViewChange: false, emitViewToModelChange: true, emitEvent: false });

                        this._ionDate.value = `${date.year}-${date.month}-${date.day}`;

                    } catch (error) {
                        this._ngControl.control.setErrors({ 'Error occurs while transforming ISO date into custom format.': true });
                    }
                }
            })
    }

    ngAfterContentInit(): void {

        this._ionDate.displayFormat = "DD/MM/YYYY";
        this._ionDate.max = "2030-12-31";
    }

    ngOnDestroy(): void {
        this._subsink.unsubscribe();
    }

    private splitDate(date: any): { year: string, month: string, day: string } {

        if (typeof date === 'number') {
            date = date.toString();
        }

        if (date.includes('-')) {

            const s = date.split('-');
            const year = s[0];
            const month = s[1];
            const day = s[2].substr(0, 2);

            return {
                year, month, day
            }
        }
        else {

            const year = date.substring(0, 4);
            const month = date.substring(4, 6);
            const day = date.substring(6, 8);

            return {
                year, month, day
            }
        }
    }
}
