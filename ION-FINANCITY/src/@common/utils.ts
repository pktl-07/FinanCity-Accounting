import { Observable, BehaviorSubject } from 'rxjs';
import { State } from '@progress/kendo-data-query';
import { CanActivate } from '@angular/router';
import { FormGroup, FormControl } from '@angular/forms';
import { HttpResponse } from '@angular/common/http';

export interface IFCConfig {
    auth: IAuthConfig,
    dbReady: BehaviorSubject<boolean>,
    general: IGeneralSetting,
    defaultSalesTax: IDefaultTax,
    defaultPurchaseTax: IDefaultTax,
    defaultPurchaseZeroTax: IDefaultTax,
    production : boolean
}

export const logoPicture = {
    ["money"]: "assets/icon/money.png",
    ["building"]: "assets/icon/building.png",
    ["external-salesperson"]: "assets/icon/external-salesperson.png",
    ["client"]: "assets/icon/client.png",
    ["internal-salesperson"]: "assets/icon/internal-salesperson.png",
    ["chart"]: "assets/icon/chart.png",
    ["profile"]: "assets/icon/profile.png",
    ["tool"]: "assets/icon/tool.png",
    ["key"]: "assets/icon/key.jpg",
    ["home"]: "assets/icon/home.png",
    ["pin"]: "assets/icon/pin.png",
    ["lead"]: "assets/icon/lead.png",
    ["thumb-up"]: "assets/icon/thumb-up.png",
    ["log-out"]: "assets/icon/log-out.png",
    ["envelop"]: "assets/icon/envelop.png",
};

export interface IAuthConfig {

    email?: string;
    api: string,
    licenseKey?: string;
    profileName?: BehaviorSubject<string>;
    isAuthenticate?: BehaviorSubject<boolean>;
    SPSK?: string;
    loginType: 'password' | 'google' | 'facebook' | 'ios';
}

export interface IGeneralSetting {
    currSK: string,
    currCode: string,
    profilePhoto: BehaviorSubject<string>,
    chattingCounter: BehaviorSubject<number>,
    mortageSalesCounter: BehaviorSubject<number>,
    leadFollowUpCounter: BehaviorSubject<number>,
    referralSalesCounter: BehaviorSubject<number>,
    version: BehaviorSubject<number>
}

export interface IDefaultTax {

    taxSK: string,
    taxCode: string,
    taxPercent: number
}

export interface IWriteable {

    save(data: {}): Observable<any>;
    update(data: {}): Observable<any>;
    delete(data: {}): Observable<any>;
}

export interface ISearchable {

    finds(state: State): Observable<any[]>;
    findByKey(key: string): Observable<any>;
}

export interface ICommissionContext {
    taxPercent: number;
    taxAmount: number;
    gross: number;
    total: number;
}

export interface INavigateMenuItem {
    resolve?: any,
    pathMatch?: string,
    redirectTo?: string,
    path?: string,
    icon?: string,
    title?: string,
    children?: Array<INavigateMenuItem>
    loadChildren?: string,
    display?: 'none' | 'dispaly',
    canActivate?: Array<CanActivate>,
    color?: string,
}

export function fcDate() {

    const date = new Date();

    const day = zeroPrepending(date.getDate(), 2);
    const month = zeroPrepending(date.getUTCMonth() + 1, 2);
    const year = date.getFullYear();

    return `${year}${month}${day}`;
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

/**
 * 
 * @param source source
 * @param digit number of length to prepend zero.
 *  eg. when source value is 1 and you want to change 01 then you should give 2 digit prepending.
 */
export function zeroPrepending(source: any, digit: number) {

    return ("0" + (parseInt(source))).slice(-1 * digit);
}

export interface FCFileInfo {
    uploadId: string,
    name: string,
    size: string,
    fileType: string,
    description: string,
    contentType: string,
    url: string
}

export function getHeaderScheme(expression?: {}) {

    let headerJson = {
        'authorize': 'authorize'
    };

    if (typeof expression != "undefined" || expression != null) {
        Object.getOwnPropertyNames(expression).forEach(f => {
            if (expression[f] !== undefined) {

                headerJson[f.toString()] = expression[f].toString();
            }
        });
    }
    return headerJson;
}

export function commissionStructureType(): Array<{ value: number, text: string }> {

    return [
        { value: 1, text: 'COMPANY LEADS' },
        { value: 2, text: 'PERSONAL LEADS' },
        { value: 3, text: 'REFERRAL LEADS' },
        { value: 4, text: 'TIER' },
    ]
}

export const notifyMeBefore: Array<{ value: number, text: string }> = [
    { value: 0, text: 'Now' },
    { value: 1, text: '1 Day Before' },
    { value: 2, text: '2 Day Before' },
    { value: 3, text: '3 Day Before' },
    { value: 4, text: '4 Day Before' },
    { value: 5, text: '5 Day Before' },
    { value: 6, text: '6 Day Before' },
    { value: 7, text: '7 Day Before' },
]

export const LeadCloseRate: Array<{ value: number, text: string }> = [
    { value: 1, text: 'Hot' },
    { value: 2, text: 'Medium' },
    { value: 3, text: 'Cold' }
]

export function kendoFilter(criteria: Array<{
    field: string,
    value: any,
    operator: 'eq' | 'isempty' | 'isnotempty' | 'doesnotcontain' | 'contains' | 'startswith' | 'endswith' | 'bwt' | 'neq' | 'gte' | 'lte',
    logic: 'and' | 'or'
}>) {

    let filterInfo = [];
    criteria.forEach(c => {
        filterInfo.push({
            logic: c.logic,
            filters: [{ field: c.field, value: c.value, operator: c.operator }]
        })
    });

    return filterInfo;
}

export function jsonFormGroup(input: any, g?: FormGroup, appendInGroup?: (s: { form: FormGroup, name: string }) => any): {} {

    let d = {};
    Object.getOwnPropertyNames(input).forEach(f => {
        if (Array.isArray(input[f]) == false && typeof input[f] == "object" && input[f] != null) {

            let n = new FormGroup({});
            d[f] = n;
            if (appendInGroup) {
                appendInGroup({ form: n, name: f });
            }

            this.getFormGroupJson(input[f], n);
        } else {

            if (g != undefined) {

                let form = new FormControl(input[f]);
                g.addControl(f, form);
                this._formControl.push(form);

            } else {
                let form = new FormControl(input[f]);
                d[f] = form;
                this._formControl.push(form);
            }
        }
    });
    return d;
}

export function formatCurrency(value: any): number {

    let f = parseFloat(value.toString().replace('$', '').replace(',', ''));
    return value = Math.round(f * 100) / 100;
}

export function getComissionContext(e: { tax: number, amount: number, call: 'gross' | 'total' }): ICommissionContext {

    let result: ICommissionContext = { gross: 0, total: 0, taxAmount: 0, taxPercent: 0 };
    if (e.tax == 0) {
        return result = {
            taxPercent: 0,
            taxAmount: 0,
            gross: formatCurrency(e.amount),
            total: formatCurrency(e.amount),
        }
    }

    switch (e.call) {
        case 'gross':

            let t = formatCurrency((formatCurrency(e.amount) / 100) * e.tax);

            return result = {
                taxPercent: e.tax,
                taxAmount: t,
                gross: formatCurrency(e.amount),
                total: formatCurrency(e.amount) + formatCurrency(t),
            }
        case 'total':

            let gross = (formatCurrency(e.amount) / ((100 + e.tax) / 100));
            let taxamount = formatCurrency(e.amount) - formatCurrency(gross);

            return result = {
                taxPercent: e.tax,
                taxAmount: taxamount,
                gross: gross,
                total: formatCurrency(e.amount),
            }
    }
}

export function nullDefault(value: any) {

    if (value == null)
        return "";
    else
        return value;
}

export function getDeletedItems(origin: any[], items: any[], key: string) {

    let a = [];
    items.forEach(x => {

        if (x[key] && x[key] != null)
            a.push(x[key]);
    });

    return origin.filter(x => x['SysKey'] != a);
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









