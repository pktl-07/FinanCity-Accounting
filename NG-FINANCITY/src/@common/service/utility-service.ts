import { ICommissionContext } from '../Infrastructure/common-interface';
import { FormGroup, FormArray, FormControl, AbstractControl } from '@angular/forms';
import { EventEmitter } from '@angular/core';
import { HttpResponse } from '@angular/common/http';

/**
 * To clone object of array data.
 * @param data Array
 */
export const cloneData = (data: any[]) => data.map(item => Object.assign({}, item));

export const verd_ed = {
    ['approve']: 'Approved',
    ['reject']: 'Rejected',
    ['save']: 'Saved',
    ['update']: 'Updated',
    ['delete']: 'Deleted',
    ['void']: 'Voided',
    ['post']: 'Posted',
    ['reverse']: 'Reversed'
}

export function isPromise(promise: any) {
    return promise && typeof promise.then === 'function'
}

export function getBlobFileName(response: HttpResponse<Blob>) {

    let fileName: string;
    try {
        const contentDisposition: string = response.headers.get('Content-Disposition');
        const r = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
        fileName = r.exec(contentDisposition)[1];
    } catch (e) {

        return 'myfile.txt';
    }
    return fileName;
}

export const recordStatusColor = {

    [-1]: { color: 'pupple', text: 'Submit'.toUpperCase() },
    [0]: { color: 'gray', text: 'New'.toUpperCase() },
    [1]: { color: 'gray', text: 'New'.toUpperCase() },
    [2]: { color: 'orange', text: 'Enter'.toUpperCase() },
    [3]: { color: 'orange', text: 'Enter'.toUpperCase() },
    [6]: { color: 'danger', text: 'Void'.toUpperCase() },
    [8]: { color: 'success', text: 'Post'.toUpperCase() },
    [125]: { color: 'danger', text: 'Reversal'.toUpperCase() },
    [126]: { color: 'danger', text: 'Reversed'.toUpperCase() },
};

export const salesStatusColor = {
    [1]: { color: 'orange', text: 'Open' },
    [2]: { color: 'success', text: 'Close' },
}

export const NotifyMeBefore: Array<{ value: number, text: string }> = [
    { value: 0, text: 'At time of event' },
    { value: 1, text: '1 Day Before' },
    { value: 2, text: '2 Day Before' },
    { value: 3, text: '3 Day Before' },
    { value: 5, text: '5 Day Before' },
    { value: 7, text: '7 Day Before' },
    { value: 14, text: '14 Day Before' },
]


export function transformDate(value: any): any {

    if (value === undefined || value === null)
        return null;

    else if (typeof (value) === 'number')
        value = value.toString();

    let d = new Date();

    d.setFullYear(parseInt(value.substring(0, 4)));
    d.setMonth(parseInt(value.substring(4, 6)) - 1);
    d.setDate(parseInt(value.substring(6, 8)));

    return d;
}

export const LeadCloseRate: Array<{ value: number, text: string }> = [
    { value: 1, text: 'Hot' },
    { value: 2, text: 'Medium' },
    { value: 3, text: 'Cold' }
]

export function getComissionContext(e: { tax: number, amount: number, call: 'gross' | 'total' }): ICommissionContext {

    let result: ICommissionContext = { gross: 0, total: 0, taxAmount: 0, taxPercent: 0 };
    if (e.tax == 0)
        return result = {
            taxPercent: 0,
            taxAmount: 0,
            gross: e.amount,
            total: e.amount,
        }
    switch (e.call) {
        case 'gross':

            let t = (e.amount / 100) * e.tax;
            return result = {
                taxPercent: e.tax,
                taxAmount: t,
                gross: e.amount,
                total: e.amount + t,
            }
        case 'total':

            let gross = (e.amount / ((100 + e.tax) / 100));
            let taxAmount = e.amount - gross;

            return result = {
                taxPercent: e.tax,
                taxAmount: taxAmount,
                gross: gross,
                total: e.amount,
            }
    }
}

export function taxType(): Array<{ value: number, text: string }> {

    return [{ value: 1, text: 'Common Module Base' },
    { value: 2, text: 'Supply' },
    { value: 3, text: 'Purchase' }];
}

export function taxCategory(): Array<{ value: number, text: string }> {

    return [{ value: 1, text: 'Standard' },
    { value: 2, text: 'Zero' },
    { value: 3, text: 'Exempted' },
    { value: 4, text: 'NA' },
    { value: 5, text: 'Not Clamiable' }];
}

export function activeViewType(): Array<{ value: boolean, text: string }> {

    return [{ value: true, text: 'ACTIVE' },
    { value: false, text: 'IN ACTIVE' }]
}

export function accountType(): Array<{ value: number, text: string }> {

    return [{ value: 1, text: 'Posting' },
    { value: 2, text: 'Header' }];
}

export function martialStatus(): Array<{ value: number, text: string }> {

    return [{ value: 0, text: 'SINGLE' },
    { value: 1, text: 'MARRIED' }];
}

export function gender(): Array<{ value: number, text: string }> {

    return [{ value: 0, text: 'MALE' },
    { value: 1, text: 'FEMALE' }];
}

export function informationSource(): Array<{ value: string, text: string }> {

    return [{ value: 'COMPANY', text: 'COMPANY' },
    { value: 'PERSONAL', text: 'PERSONAL' },
    { value: 'FACEBOOK', text: 'FACEBOOK' },
    { value: 'GOOGLE', text: 'GOOGLE' },
    { value: 'REFERRAL', text: 'REFERRAL' },
    { value: 'OTHERS', text: 'OTHERS' }];
}

export function saleActivity(): Array<{ value: string, text: string }> {

    return [{ value: 'PROSPECTING', text: 'PROSPECTING' },
    { value: 'PRESENTATION', text: 'PRESENTATION' },
    { value: 'SALES FOLLOW-UP', text: 'SALES FOLLOW-UP' },
    { value: 'SALES CLOSURE', text: 'SALES CLOSURE' }];
}

export function salutation(): Array<{ value: string, text: string }> {

    return [{ value: 'MR', text: 'MR' },
    { value: 'MRS', text: 'MRS' },
    { value: 'MS', text: 'MS' },
    { value: 'MDM', text: 'MDM' },
    { value: 'DR', text: 'DR' }];
}

export function indexingFormat(): Array<{ value: number, text: string }> {

    return [
        { value: 15, text: 'MANUAL' },
        { value: 10, text: 'BUILD IN' },
        { value: 20, text: 'yy:mm:####' },
    ]
}

export function vendorType(): Array<{ value: number, text: string }> {

    return [{ value: 15, text: 'REA' },
    { value: 16, text: 'PROPERTY DEVELOPER' },
    { value: 17, text: 'LAW FIRM' },
    { value: 18, text: 'BANK' },
    { value: 100, text: 'REFERRAL' },
    { value: 19, text: 'OTHERS' }];
}

export function cashBookType(): Array<{ value: number, text: string }> {

    return [{ value: 1, text: 'CHEQUE' },
    { value: 2, text: 'BANK' }];
}

export function commissoinType(): Array<{ value: number, text: string, IsActive: boolean }> {

    return [{ value: 1, text: 'COMPANY LEADS', IsActive: true },
    { value: 2, text: 'PERSONAL LEADS', IsActive: true },
    { value: 3, text: 'REFERRAL LEADS', IsActive: true },
    { value: 4, text: 'TIER', IsActive: true }];
}

export function rankType(): Array<{ value: number, text: string }> {

    return [{ value: 1, text: 'SALESPERSON' },
    { value: 5, text: 'TEAM MANAGER' },
    { value: 10, text: 'DIVISION DIRECTOR' }];
}

export function paymentMode(): Array<{ value: number, text: string }> {

    return [{ value: 1, text: 'BANK TRANSFER' },
    { value: 5, text: 'CASH' },
    { value: 10, text: 'CHEQUE' }];
}

export function paymentType(): Array<{ value: number, text: string }> {
    return [{ value: 1, text: 'PARTIAL' },
    { value: 2, text: 'FULL' }];
}

export interface IKendoFilter {
    'field': string,
    'value': any,
    operator: 'eq' | 'isempty' | 'isnotempty' | 'doesnotcontain' | 'contains' | 'startswith' | 'endswith' | 'bwt' | 'neq' | 'gte' | 'lte',
    logic: 'and' | 'or'
}


export function kendoFilter(criteria: IKendoFilter[]) {

    const filterInfo = [];
    criteria.map(c => {
        filterInfo.push({
            logic: c.logic,
            filters: [{ field: c.field, value: c.value, operator: c.operator }]
        })
    })

    return filterInfo;
}

export function recordStatusOptions() {

    return [{ text: 'Submit'.toUpperCase(), value: -1 },
    { text: 'New'.toUpperCase(), value: 1 },
    { text: 'Enter'.toUpperCase(), value: 2 },
    //{ text: 'Enter Update'.toUpperCase(), value: 3 },
    { text: 'Void'.toUpperCase(), value: 6 },
    { text: 'Post'.toUpperCase(), value: 8 },
    { text: 'Reversal'.toUpperCase(), value: 125 },
    { text: 'Reversed'.toUpperCase(), value: 126 }]
}

export function accountingModules() {

    return [
        {
            value: 'GLC', text: 'General Ledger'
        }, {
            value: 'CBP', text: 'Payments'
        }, {
            value: 'CBR', text: 'Receipts'
        }, {
            value: 'SOP', text: 'Sales (AR)'
        }, {
            value: 'POP', text: 'Purchase (AP)'
        }
    ];
}

export function salespersonStatus() {

    return [
        {
            value: 1, text: 'LIVE'
        }, {
            value: 5, text: 'DUMMY'
        }, {
            value: 10, text: 'PENDING'
        }, {
            value: 15, text: 'IPA'
        }, {
            value: 20, text: 'RESIGNED'
        }, {
            value: 25, text: 'LAPSED'
        }, {
            value: 30, text: 'WITHDRAWN'
        }, {
            value: 35, text: 'REJECTED'
        }, {
            value: 40, text: 'TERMINATED'
        }
    ];
}

export function makeFormTouched(group: FormGroup | FormArray) {

    Object.keys(group.controls).forEach((key: string) => {
        const abstractControl = group.controls[key];
        if (abstractControl instanceof FormGroup || abstractControl instanceof FormArray) {
            makeFormTouched(abstractControl)
        } else {

            let formControl = abstractControl as FormControl;
            formControl.markAsTouched();
            (formControl.statusChanges as EventEmitter<any>).emit('TOUCHED');
        }
    })
}

export function findObject(json: any, keys: string[]) {

    let result: any;
    if (typeof keys != 'undefined') {
        for (let i = 0; i < keys?.length; i++) {
            let j = Object.getOwnPropertyNames(json);
            for (let y = 0; y < j.length; y++) {
                if (j[y] == keys[i] && typeof json[j[y]] == 'object') {
                    json = json[j[y]];
                    continue;
                }
                else if (j[y] == keys[i]) {
                    result = json[j[y]];
                    continue;
                }
            }
        }
    }
    return result;
}

export function createJsonFormGroup(input: any, g?: FormGroup): {} {
    //{key.[key], pipe}
    let d = {};//result json.
    Object.getOwnPropertyNames(input).forEach(f => {

        if (Array.isArray(input[f]) == false && typeof input[f] == "object" && input[f] != null) { //check parent node.

            let n = new FormGroup({}); //create form group.
            d[f] = n;
            createJsonFormGroup(input[f], n); //recursively add child json and carring parent form group object.
        } else {

            if (g != undefined) {

                let form = new FormControl(input[f]);
                g.addControl(f, form); //add to the group

            } else {

                let form = new FormControl(input[f]);
                d[f] = form; //add to traget json as form control.
            }
        }
    });
    return d;
}

export function getAbstractControlName(control: AbstractControl): string | null {

    let group = <FormGroup>control.parent;
    if (!group) {
        return null;
    }

    let name: string;
    Object.keys(group.controls).forEach(key => {
        let childControl = group.get(key);

        if (childControl !== control) {
            return;
        }

        name = key;
    });
    return name;
}

export function today() {
    let d = new Date();
    let year = d.getFullYear();
    let month = zeroPrepending(d.getMonth() + 1, 2);
    let day = zeroPrepending(d.getDate(), 2);

    return `${year}${month}${day}`;
}

/**
 * 
 * @param source source
 * @param digit number of length to prepend zero.
 *  eg. when source value is 1 and you want to change 01 then you should give 2 digit prepending.
 */
export function zeroPrepending(source: any, digit: number) {

    return ("0" + (parseInt(source))).slice(-1 * digit);
}

export function addressFormat(param: {
    building: string,
    address1: string,
    address2: string,
    floorUnit: string,
    postalCode: string
}) {

    if (param.building != null && param.building != '')
        param.building += `${param.building}`;
    else
        param.building = '';

    if (param.address1 != null && param.address1 != '')
        param.address1 += `${param.address1},`;
    else
        param.address1 = '';

    if (param.address2 != null && param.address2 != '')
        param.address2 += `${param.address2},`;
    else
        param.address2 = '';

    if (param.floorUnit != null && param.floorUnit != '')
        param.floorUnit += `#${param.floorUnit},`;
    else
        param.floorUnit = '';

    if (param.postalCode != null && param.postalCode != '')
        param.postalCode += `${param.postalCode}`;
    else
        param.postalCode = '';

    return `${param.building} ${param.address1} ${param.address2} ${param.floorUnit} ${param.postalCode}`;
}

export function rebindJsonFormGroup(group: FormGroup | FormArray,
    data: any,
    type: 'null custom form data' | 'none') {

    Object.keys(group.controls).forEach((key: string) => {

        if (typeof data[key] != 'undefined') {

            const abstractControl = group.controls[key];

            if (abstractControl instanceof FormGroup || abstractControl instanceof FormArray)
                rebindJsonFormGroup(abstractControl, data[key], type);
            else {

                let formControl = abstractControl as FormControl;
                // let controlName = getAbstractControlName(formControl);
                if (Object.keys(data).filter(f => f == key).length > 0) {
                    formControl.patchValue(data[key], { emitEvent: false })
                }
                else {
                    if (type == 'null custom form data') {
                        formControl.patchValue(null);
                    }
                }
            }
        }
    });
}

export function multiCurrency(param: { value: number, rate: number, op: '*' | '/' }) {

    if (param.op == '*')
        return Number((param.value / param.rate).toFixed(2));

    return Number((param.value * param.rate).toFixed(2));
}

export function fcDate() {

    const date = new Date();

    const day = zeroPrepending(date.getDate(), 2);
    const month = zeroPrepending(date.getUTCMonth() + 1, 2);
    const year = date.getFullYear();

    return `${year}${month}${day}`;
}

const isFunction = (fn: any) => typeof fn === 'function';

export interface SubscriptionLike {
    unsubscribe(): void;
}

export class SubSink {

    protected _subs: SubscriptionLike[] = [];

    constructor() { }

    add(...subscriptions: SubscriptionLike[]) {
        this._subs = this._subs.concat(subscriptions);
    }

    set sink(subscription: SubscriptionLike) {
        this._subs.push(subscription);
    }

    unsubscribe() {
        this._subs.forEach(sub => sub && isFunction(sub.unsubscribe) && sub.unsubscribe());
        this._subs = [];
    }
}



