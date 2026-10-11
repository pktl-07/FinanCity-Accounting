import { Injectable, OnDestroy } from '@angular/core';
import { AbstractControl } from '@angular/forms';
import { SubSink } from 'src/@common/utils';

@Injectable({ providedIn: 'root' })
export class NameConventionService implements OnDestroy {

    private _sourceName: AbstractControl;
    private _firstName: AbstractControl;
    private _lastName: AbstractControl;
    private _isLastNameFirst: AbstractControl;
    private _subsink = new SubSink();

    ngOnDestroy(): void {
        this._subsink.unsubscribe();
    }

    register(param: {
        sourceName: AbstractControl,
        fristName: AbstractControl,
        lastName: AbstractControl,
        isLastNameFirst: AbstractControl
    }) {

        this._sourceName = param.sourceName;
        this._firstName = param.fristName;
        this._lastName = param.lastName;
        this._isLastNameFirst = param.isLastNameFirst;
    }

    public watch() {

        this.intializeSource();

        this._subsink.sink = this._firstName.valueChanges
            .subscribe(() => {
                this.setFullName();
            });

        this._subsink.sink = this._lastName.valueChanges
            .subscribe(() => {
                this.setFullName();
            });

        this._subsink.sink = this._isLastNameFirst.valueChanges
            .subscribe(() => {
                this.setFullName();
            });
    }

    private intializeSource() {

        let s = this._sourceName.value == null ? '' : this._sourceName.value;
        let sourceName = s.trim().split(/\s/g);

        if (this._isLastNameFirst.value == true) {

            this._lastName.patchValue(sourceName[0], { emitEvent: false });

            let firstName = "";

            for (let i = 1; i < sourceName.length; i++) {
                firstName += ` ${sourceName[i]}`;
            }

            this._firstName.patchValue(firstName, { emitEvent: false });

        } else {

            this._firstName.patchValue(sourceName[0], { emitEvent: false });

            let lastName = "";

            for (let i = 1; i < sourceName.length; i++) {

                lastName += ` ${sourceName[i]}`;
            }

            this._lastName.patchValue(lastName, { emitEvent: false });
        }
    }

    private setFullName() {

        let firstName = this._firstName.value != null ? this._firstName.value : "";
        let lastName = this._lastName.value != null ? this._lastName.value : "";

        if (this._isLastNameFirst.value == true)
            this._sourceName.patchValue(`${lastName} ${firstName}`, { emitEvent: false });
        else
            this._sourceName.patchValue(`${firstName} ${lastName}`, { emitEvent: false });
    }
}