import { Injectable } from '@angular/core';
import { filterItem } from './p-filter-panel-model';
import { kendoFilter } from 'src/@common/service/utility-service';

@Injectable({ providedIn: 'root' })
export class FilterItemTraversalService {

    constructor() { }

    private _isValid = true;
    public get isValid() {
        return this._isValid;
    }
    public set isValid(value) {
        this._isValid = value;
    }

    markFormTouch(items: filterItem[]) {
        this._isValid = true;
        items.map((o, i) => {
            o.value_1.markAsTouched();
            if (this._isValid) {
                this._isValid = o.value_1.valid;
            }
        });
    }

    traverseFilter(items: filterItem[]): any[] {

        let filterInfo: any[] = [];

        items.map((o, i) => {
            if (o.value_1.valid) {
                if (o.control == "dropdown") {
                    if (o.value_1.value && o.value_1.value != null) {
                        const a = kendoFilter([
                            { field: o.field, value: o.value_1.value, operator: 'eq', logic: 'and' },
                        ]);
                        filterInfo.push({
                            logic: 'and',
                            filters: a[0]['filters']
                        })
                    }
                }

                if (o.control == 'multi-select' || o.control == "checkbox"
                    || o.control == 'record_status') {

                    if (o.value_1.value && o.value_1.value != null) {

                        const data = (o.value_1.value as any[]);
                        const source = data.map(item => Object.assign(item));

                        if (o.control == 'record_status' && source.filter(x => x == 2 || x == 3).length > 0) {
                            source.push(...[2, 3])
                        }

                        source.map((m, i) => {

                            let result = {};
                            let filters = [];
                            let a = {};

                            a['field'] = o.field;

                            if (source.length > 1) {
                                //<# #> is an expression 
                                if (i == 0)
                                    a['value'] = `<# ( #> ${m}`;
                                else if (i == source.length - 1)
                                    a['value'] = `${m} <# ) #>`;
                                else
                                    a['value'] = m;
                            } else {
                                a['value'] = m;
                            }

                            a['operator'] = 'eq';

                            filters.push(a);
                            result['filters'] = filters;

                            if (i < source.length - 1)
                                result['logic'] = 'or';
                            else
                                result['logic'] = 'and';

                            filterInfo.push(result);
                        })
                    }
                }
                else if (o.control == 'date-period') {

                    if (o.value_1.value == null) {
                        const currentYear = new Date().getFullYear();
                        o.value_1.patchValue(`${currentYear}0101-${currentYear}1231`)
                    }

                    if (o.value_1.value && o.value_1.value != null && o.value_1.value != '') {

                        const period = (o.value_1.value as string).split('-');
                        const a = kendoFilter([
                            { field: o.field, value: period[0], operator: 'gte', logic: 'and' },
                            { field: o.field, value: period[1], operator: 'lte', logic: 'and' },
                        ]);

                        filterInfo.push({
                            logic: 'and',
                            filters: kendoFilter([
                                { field: o.field, value: period[0], operator: 'gte', logic: 'and' },
                            ])[0]['filters']
                        });

                        filterInfo.push({
                            logic: 'and',
                            filters: kendoFilter([
                                { field: o.field, value: period[1], operator: 'lte', logic: 'and' },
                            ])[0]['filters']
                        });
                    }
                }
                else if (o.operator.value == 'bwt') {

                    if (o.value_1.value && o.value_1.value != null && o.value_1.value != '') {

                        const a = kendoFilter([
                            { field: o.field, value: o.value_1.value, operator: 'gte', logic: 'and' },
                        ]);

                        filterInfo.push({
                            logic: 'and',
                            filters: a[0]['filters']
                        })
                    }
                    if (o.value_2.value && o.value_2.value != null && o.value_2.value != '') {

                        const a = kendoFilter([
                            { field: o.field, value: o.value_2.value, operator: 'lte', logic: 'and' },
                        ]);

                        filterInfo.push({
                            logic: 'and',
                            filters: a[0]['filters']
                        })
                    }
                }
                else if (o.operator.value == 'isempty' || o.operator.value == 'isnull'
                    || o.operator.value == 'isnotempty' || o.operator.value == 'isnotnull') {

                    const a = kendoFilter([
                        { field: o.field, value: null, operator: o.operator.value, logic: 'and' }
                    ]);

                    filterInfo.push({
                        logic: 'and',
                        filters: a[0]['filters']
                    })
                }
                else if (o.value_1.value && o.value_1.value != null && o.value_1.value != '') {

                    const a = kendoFilter([
                        { field: o.field, value: o.value_1.value, operator: o.operator.value, logic: 'and' }
                    ]);

                    filterInfo.push({
                        logic: 'and',
                        filters: a[0]['filters']
                    })
                }
            }
        })

        return filterInfo;
    }
}