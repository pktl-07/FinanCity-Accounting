import { FormControl } from '@angular/forms';
import { BehaviorSubject } from 'rxjs';
import { PGridInCellDropDownItemTemplateField } from 'src/@common/Infrastructure/common-interface';

export interface filterItem {
    title?: string,
    field?: string,
    value_1?: FormControl,
    value_2?: FormControl,
    operator?: any,
    operators?: any[],
    control: 'dropdown' | 'multi-select' | 'checkbox' | 'textbox' | 'multi-control' | 'record_status' | 'date-period' | 'hidden'
    | 'currency' | 'number' | 'date' ,
    valueChange?: (value: any) => any,
    multiControl?: {
        source?: FormControl | BehaviorSubject<any>,
        valueField?: string,
        textField?: string,
        filterChange?: (text: any) => any,
        itemTemplateField?: PGridInCellDropDownItemTemplateField,
    }
}

export const stringFilter = [
    { value: 'eq', text: 'Equals', symbol: '=' },
    { value: 'isempty', text: 'Is Empty', symbol: '....' },
    { value: 'isnotempty', text: 'Is Not Empty', symbol: '!...' },
    { value: 'doesnotcontain', text: 'Not Contain', symbol: '!(...)' },
    { value: 'contains', text: 'Contains', symbol: '(....)' },
    { value: 'startswith', text: 'Start With', symbol: 'sw' },
    { value: 'endswith', text: 'End With', symbol: 'ew' },
];

export const numberFilter = [
    { value: 'bwt', text: 'Between', symbol: '><' },
    { value: 'neq', text: 'Not Equals', symbol: '<>' },
    { value: 'eq', text: 'Equals', symbol: '=' },
    { value: 'gt', text: 'Greater Than', symbol: '>' },
    { value: 'gte', text: 'Greater Than Equals', symbol: '>=' },
    { value: 'lt', text: 'Less Than', symbol: '<' },
    { value: 'lte', text: 'Less Than Equals', symbol: '<=' },
    { value: 'isnull', text: 'Is Null', symbol: 'null' },
    { value: 'isnotnull', text: 'Is Not Null', symbol: 'not null' },
];

export function traverseFilter(items: filterItem[]): any[] {

    let filterInfo = [];
    let result = {};

    items.map((o, i) => {

        if (o.value_1.value && o.value_1.value != null && o.value_1.value != '') {

            result['field'] = o.field;
            if (o.operator.text = 'Between') {

                result['value'] = o.value_1.value;
                result['operator'] = 'gte';
                result['logic'] = 'and'

                filterInfo.push(result);

                if (o.value_2.value && o.value_2.value != null && o.value_2.value != '') {

                    result['value'] = o.value_2.value;
                    result['operator'] = 'lte';
                    result['logic'] = 'and'

                    filterInfo.push(result);
                }
            } else {

                switch (o.control) {
                    case 'textbox':

                        result['value'] = o.value_1.value
                        result['operator'] = o.operator.value;
                        result['logic'] = 'and';

                        filterInfo.push(result);
                        break;

                    case 'multi-control':

                        (o.value_1.value as []).map((o, i) => {

                            result['value'] = o['text'];
                            result['operator'] = 'eq';
                            result['logic'] = 'or';

                            filterInfo.push(result);
                        })
                        break;
                }
            }
        }
    });

    return filterInfo;
}