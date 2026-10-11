import { FormBuilder, FormGroup } from '@angular/forms';
import { Subject, BehaviorSubject } from 'rxjs';
import { DropDownFilterSettings } from '@progress/kendo-angular-dropdowns';
import { IDataComplete, IDataReady } from './common-interface';

export class DetailFormBase {

    public fb: FormBuilder;
    private _form: FormGroup;

    public get form(): FormGroup {
        return this._form;
    }

    public set form(value: FormGroup) {

        if (value && value != null)
            this.formReady.next({ ready: true, form: value });

        this._form = value;
    }

    public formReady = new BehaviorSubject<IDataReady>({ ready: false, form: null });
    public OnSuccess: Subject<IDataComplete> = new Subject<IDataComplete>();
    public Start: Subject<boolean> = new Subject();

    constructor(fb: FormBuilder) {
        this.fb = fb;
    }

    public filterSettings: DropDownFilterSettings = {
        caseSensitive: false,
        operator: 'contains'
    };

    public itemDisabled(itemArgs: { dataItem: any, index: number }) {

        if (typeof itemArgs.dataItem.IsActive != 'undefined')
            return !itemArgs.dataItem.IsActive;

        return false;
    }
}
