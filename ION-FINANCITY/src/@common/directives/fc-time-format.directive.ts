import { Directive, AfterContentInit, OnInit, OnDestroy } from '@angular/core';
import { IonDatetime } from '@ionic/angular';
import { NgControl } from '@angular/forms';
import { SubSink } from '../utils';

@Directive({
    selector: '[fcTime]',
})
export class FCTimeFormatDirective implements OnInit, AfterContentInit, OnDestroy {

    private _subsink = new SubSink();

    constructor(private _ngControl: NgControl,
        private _ionDate: IonDatetime) {
    }

    ngOnInit(): void {

        if (this._ngControl.control.value && this._ngControl.control.value != null) {

            const d = this.splitTime(this._ngControl.control.value);
            this._ionDate.value = `${d.hour}:${d.min}`
        }

        this._subsink.sink = this._ngControl.control.valueChanges.subscribe((value: string) => {

            try {
                if (value == null) {
                    this._ngControl.control.patchValue(null,
                        { emitModelToViewChange: false, emitViewToModelChange: true, emitEvent: false });
                    this._ionDate.value = null;
                }
                else {
                    this._ngControl.control.patchValue(value.replace(':', ''),
                        { emitModelToViewChange: false, emitViewToModelChange: true, emitEvent: false });

                    const d = this.splitTime(this._ngControl.control.value);
                    this._ionDate.value = `${d.hour}:${d.min}`
                }
            } catch {
                console.error('Error occurs while transforming ISO date into custom format.')
            }
        });
    }

    ngOnDestroy(): void {

        this._subsink.unsubscribe();
    }

    ngAfterContentInit(): void {

        this._ionDate.displayFormat = "hh:mm a";
    }

    private splitTime(time: any): { hour: string, min: string } {

        const hour = time.toString().substr(0, 2);
        const min = time.toString().substr(2, 2);

        return { hour: hour, min: min };
    }
}
