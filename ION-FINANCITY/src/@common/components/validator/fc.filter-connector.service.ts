import { Injectable, QueryList } from "@angular/core";
import { BehaviorSubject } from 'rxjs';
import { kendoFilter } from 'src/@common/utils';
import { FCFilterItemDirective } from '../filter/fc-filter-item.directive';

@Injectable()
export class FilterConnectorService extends BehaviorSubject<any> {

    constructor() {
        super([])
    }

    public filterValue: string;
    public countingSubject = new BehaviorSubject<number>(0);

    toKendoFilter(components: QueryList<FCFilterItemDirective>): any[] {

        let count = 0;
        let filterResult = [];

        components.filter(f => (f.type == 'text' || f.type == 'hidden') && f.value_1 && f.value_1 != null)
            .forEach((c, i) => {

                if (c.type != 'hidden')
                    count++;

                filterResult.push(...kendoFilter([
                    { field: c.field, value: c.value_1, operator: 'contains', logic: c.logic }
                ]));
            });

        components.filter(f => f.type == 'dropdown' && (f.value_1 && f.value_1 != null) && f.value_1.length > 0)
            .forEach((c, i) => {

                let source = c.value_1 as Array<any>;
                if (source.length == 1) {

                    count++;
                    filterResult.push(...kendoFilter([
                        { field: c.field, value: source[0], operator: 'eq', logic: 'and' },
                    ]));
                } else {
                    source.map((m, i) => {
                        if (i == 0) {

                            count++;
                            filterResult.push(...kendoFilter([
                                { field: c.field, value: `<# ( #> ${m}`, operator: 'eq', logic: c.logic },
                            ]));
                        } else if (i == source.length - 1) {

                            count++;
                            filterResult.push(...kendoFilter([
                                { field: c.field, value: `${m} <# ) #>`, operator: 'eq', logic: c.logic },
                            ]));
                        } else {

                            count++;
                            filterResult.push(...kendoFilter([
                                { field: c.field, value: m, operator: 'eq', logic: c.logic },
                            ]));
                        }
                    });
                }
            });

        components.filter(f => (f.type == 'date' || f.type == 'currency' || f.type == 'number'))
            .forEach((c, i) => {

                if (c.value_1 && c.value_1 != null && c.value_2 && c.value_2 != null) {

                    count++;
                    filterResult.push(...kendoFilter([
                        { field: c.field, value: c.value_1, operator: 'gte', logic: 'and' },
                    ]));

                    count++;
                    filterResult.push(...kendoFilter([
                        { field: c.field, value: c.value_2, operator: 'lte', logic: 'and' },
                    ]));
                }
                else if (c.value_1 && c.value_1 != null) {
                    if (c.value_1 && c.value_1 != null) {

                        count++;
                        filterResult.push(...kendoFilter([
                            { field: c.field, value: c.value_1, operator: 'eq', logic: 'and' },
                        ]));
                    }
                }
            });

        this.countingSubject.next(count);
        return filterResult;
    }
}