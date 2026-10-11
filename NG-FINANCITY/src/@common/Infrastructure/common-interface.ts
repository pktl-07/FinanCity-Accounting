import { CompositeFilterDescriptor, State } from '@progress/kendo-data-query';
import { Observable, BehaviorSubject } from 'rxjs';
import { Subject } from 'rxjs';
import { BaseApiService } from './base.api.service';
import { FormGroup } from '@angular/forms';
import { GridComponent } from '@progress/kendo-angular-grid';

export interface ICommandToolBarConfig {
    type?: number,
    title?: string,
    navigatButton?: IButtonConfig,
    actionButtons?: IButtonConfig[]
}

export interface IButtonConfig {
    text?: string,
    disabled?: boolean,
    addSeperator?: boolean,
    icon?: any,
    action?: () => any,
}

export interface LoaderState {
    show: boolean;
}

export interface IPageable {
    getPager(state: State): void;
}

export interface ISearchable {

    finds(state: State): Observable<any[]>;
    findByKey(key: string): Observable<any>;
}

export interface IWriteable {

    save(data: {}): Observable<any>;
    update(data: {}): Observable<any>;
    delete(data: {}): Observable<any>;
}

export interface IPageableConfig {

    buttonCount: number,
    info: boolean,
    type: string,
    previousNext: boolean
}

export interface IGridConfig {
    pageSize: number,
    skip: number,
    scrollable: string,
    filter?: CompositeFilterDescriptor,
    filterable: boolean
}

export interface IDataToolbarConfig {
    start?: Subject<boolean>,
    success?: Subject<IDataComplete>,
    fromReady?: BehaviorSubject<IDataReady>,
    statekey?: string[],
    buttons: ButtonControlConfig[],
}

export interface ButtonControlConfig {
    label?: string;
    action: IActionAnimationConfig | any;
    orderyBy?: number;
    disabled?: boolean;
    hide?: boolean,
    icon?: any;
}

export interface IActionAnimationConfig {
    action?: () => Observable<any> | Promise<any> | Function, // attach form task
    validate?: () => boolean, // check that condition before submitting form
    disabled?: boolean,
    requireConfirmation?: boolean, // confirmation dialog
    requireNotification?: boolean, // show notification pop up on top of the form
    requireLoading?: boolean
}

export interface PGridIncellEditingConfig {
    columns: Array<PGridInCellColumn>,
    key: string,
    fetch: BaseApiService | BehaviorSubject<any>,
    rowDataBound?: (e: { data: any, index?: number }) => void,
    saveCompleted?: (e: { sender: GridComponent, formGroup: FormGroup, rowIndex: number, isNew: boolean }) => void,
    updateCompleted?: (e: { formGroup: FormGroup, rowIndex: number }) => void,
    deleteCompleted?: (e: { sender: GridComponent, formGroup: FormGroup, rowIndex: number }) => void,
    canEditRow?: (data: { rowIndex: number, columnIndex: number, dataItem: any, isEdited: boolean }) => boolean,
    canDeleteRow?: (data: { rowIndex: number, dataItem: any }) => boolean,
    height?: number,
}

export interface PGridInCellColumn {
    field: string,
    title?: string,
    width?: number,
    dataType: 'hidden' | 'display' | 'dispaly/currency' | 'dispaly/checkbox' | 'string' | 'number' | 'currency' | 'dropdown' | 'chekbox' | 'cascade_checkbox',
    dropdown?: PGridInCellDropDownColumn,
    defaultValue?: any,
    validators?: any,
    align?: 'center' | 'right' | 'left',
    clickable?: boolean,
    dropdownOpen?: (data: { dataItem: any, fg: FormGroup, column: PGridInCellColumn, event: any, isNew: boolean, rowIndex: number }) => any,
    cellValueChange?: (data: { dataItem: any, fg: FormGroup, column: PGridInCellColumn, event: any, isNew: boolean, rowIndex: number }) => any,
    disableDeleteButton?: (data: { dataItem: any }) => boolean,
}

export interface PGridInCellDropDownItemTemplateField {
    field1: string,
    field2: string,
}

export interface PGridInCellDropDownColumn {
    fetch: BaseApiService | any[] | BehaviorSubject<any[]>,
    textField: string,
    valueField: string,
    customFetching?: (e: { data: any }) => any,
    itemEmpty?: (d: (item: any) => any) => any,
    itemTemplateField?: PGridInCellDropDownItemTemplateField,
    disabledItem?: (item: any) => boolean,
    customFilter?: (e: { dropDown: PGridInCellDropDownColumn, value: string }) => any
}

export interface PGridViewConfig {
    fetch?: BaseApiService | BehaviorSubject<any>,
    methodName?: string,
    columns?: PGridViewColumn[],
    key?: string,
    filterable?: 'menu' | 'none' | 'window',
    detailTemplate?: any,
    rowDataBound?: (e: { data: any, index?: number }) => void,
    insertable?: () => any,
    rowFilter?: (data: CompositeFilterDescriptor[]) => any[],
}

export interface PFilterOptions {
    control?: 'dropdown' | 'textbox' | 'multi-select' | 'date' | 'number' | 'currency' | 'date-period',
    source?: { text: any, value: any }[] | BehaviorSubject<any>,
    filterChange?: (text: string) => any,
    valueField?: string,
    textField?: string,
    defaultValue1?: any | { text: any, value: any }[],
    defaultValue2?: any,
    operator?: 'eq' | 'isempty' | 'isnotempty' | 'doesnotcontain' | 'contains' | 'startswith' | 'endswith' | 'bwt' | 'neq'
    | 'gt' | 'gte' | 'lt' | 'lte' | 'isnull' | 'isnotnull'
}

export interface IDataComplete {
    data: any,
    action: string,
}

export interface IDataReady {
    form: FormGroup
    ready: boolean,
}

export interface PGridViewColumn {
    field: any,
    title: string,
    width?: number,
    viewDetail?: (key: any, dataItem?: any) => void,
    valueChange?: (item: any) => any,
    filter: boolean,
    dataType: 'string' | 'number' | 'dropdown' | 'date' | 'date-period' | 'currency' | 'button' | 'checkbox' | 'currcode' | 'record_status' | 'hidden' | 'sales-status',
    align?: 'center' | 'right' | 'left',
    filterOptions?: PFilterOptions,
    validators?: any,
    itemTemplateField?: PGridInCellDropDownItemTemplateField,
}

export interface IAuthConfig {
    email?: string;
    api: string,
    licenseKey?: string;
    profileName?: string;
    isAuthenticate?: boolean;
}

export interface IGeneralSetting {
    currSK: string,
    currCode: string,
}

export interface FinanCityConfig {
    production: boolean,
    auth: IAuthConfig,
    general: IGeneralSetting,
    defaultSalesTax: IDefaultTax,
    defaultPurchaseTax: IDefaultTax,
    defaultPurchaseZeroTax: IDefaultTax,
}

export interface IDefaultTax {
    taxSK: string,
    taxCode: string,
    taxPercent: number
}

export interface ICommissionContext {
    taxPercent: number;
    taxAmount: number;
    gross: number;
    total: number;
}

export interface DetailView {
    dataItem: any,
}

export function getHeaderScheme(expression?: {}) {
    let headerJson = {
        'authorize': 'authorize'
    };
    if (typeof expression != "undefined" || expression != null) {
        Object.getOwnPropertyNames(expression).forEach(f => {
            if (expression[f] !== undefined) {
                headerJson[f.toString()] = expression[f];
            }
        });
    }
    return headerJson;
}









