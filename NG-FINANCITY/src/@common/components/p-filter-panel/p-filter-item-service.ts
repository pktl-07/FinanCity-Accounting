import { Injectable } from '@angular/core';
import { PGridViewConfig, PGridViewColumn } from 'src/@common/Infrastructure/common-interface';
import { FormGroup, FormBuilder, FormArray, FormControl } from '@angular/forms';
import { filterItem, stringFilter, numberFilter } from './p-filter-panel-model';
import { BaseApiService } from 'src/@common/Infrastructure/base.api.service';
import { recordStatusOptions } from 'src/@common/service/utility-service';

@Injectable({ providedIn: 'root' })
export class FilterItemService {

    public form: FormGroup;
    private currentItem: filterItem;

    constructor(private fb: FormBuilder) {
    }

    public translateForm(config: PGridViewConfig) {

        this.form = this.fb.group({
            formItems: this.fb.array([])
        });

        config.columns.map((o, i) => {

            if (o.filter == true) {
                this.currentItem = {
                    title: null,
                    field: null,
                    control: o.dataType == 'hidden' ? 'hidden' : 'textbox',
                    operator: stringFilter.find(x => x.value == 'eq'),
                    operators: [],
                    value_1: new FormControl(null),
                    value_2: new FormControl(null),
                    valueChange: null,
                    multiControl: {
                        source: new FormControl(null),
                        itemTemplateField: o.itemTemplateField,
                    }
                };
                this.visitTitle(o);
            }
        })
    }

    private visitTitle(col: PGridViewColumn) {

        if (col.title)
            this.currentItem.title = col.title;

        this.visitField(col);
    }

    private visitField(col: PGridViewColumn) {

        if (col.field)
            this.currentItem.field = col.field;

        this.visitControl(col);
    }

    private visitControl(col: PGridViewColumn) {

        if (col.filterOptions && col.filterOptions.control) {
            this.currentItem.control = col.filterOptions.control;
        } else {
            switch (col.dataType) {
                case 'record_status':
                    this.currentItem.control = 'record_status';
                    break;
                case 'hidden':
                    this.currentItem.control = 'hidden';
                    break;
                case 'dropdown':
                    this.currentItem.control = 'dropdown';
                    break;
                case 'string':
                    this.currentItem.control = 'textbox';
                    break;
                case 'currency':
                    this.currentItem.control = 'currency';
                    break;
                case 'number':
                    this.currentItem.control = 'number';
                    break;
                case 'date':
                    this.currentItem.control = 'date';
                    break;
                case 'date-period':
                    this.currentItem.control = 'date-period';
                    break;
                default:
                    this.currentItem.control = 'multi-select';
                    break;
            }
        }

        this.visitOperators(col);
    }

    private visitOperators(col: PGridViewColumn) {

        switch (this.currentItem.control) {
            case 'dropdown':
                this.currentItem.operators = [{ value: 'eq', text: 'Equals', symbol: '=' }];
                break;
            case 'textbox':
            case 'hidden':
                this.currentItem.operators = stringFilter;
                break;
            case 'currency':
            case 'number':
            case 'date':
                this.currentItem.operators = numberFilter;
                break;
            default:
                this.currentItem.operators = null;
                break;
        }

        this.visitOperator(col);
    }

    private visitOperator(col: PGridViewColumn) {

        if (!col.filterOptions) {

            switch (col.dataType) {
                case 'string':
                case 'hidden':
                case 'dropdown':
                    this.currentItem.operator = stringFilter.find(x => x.value == 'contains');
                    break;
                case 'currency':
                case 'number':
                case 'date':
                    this.currentItem.operator = numberFilter.find(x => x.value == 'bwt');
                    break;
                case 'dropdown':
                    this.currentItem.operator = numberFilter.find(x => x.value == 'eq');
                    break;
                default:
                    this.currentItem.operator = numberFilter.find(x => x.value == 'eq');
                    break;
            }
        } else {

            if (col.filterOptions.operator) {
                this.currentItem.operator = [...stringFilter, ...numberFilter]
                    .filter(x => x['value'] == col.filterOptions.operator)[0];
            } else {
                this.currentItem.operator = numberFilter.find(x => x.value == 'eq');
            }
        }

        this.visitSetDefault(col);
    }

    private visitSetDefault(col: PGridViewColumn) {

        if (col.filterOptions) {
            if (col.filterOptions.defaultValue1)
                this.currentItem.value_1.setValue(col.filterOptions.defaultValue1);
            if (col.filterOptions.defaultValue2)
                this.currentItem.value_2.setValue(col.filterOptions.defaultValue2);

        } else {

            this.currentItem.value_1.setValue(null);
            this.currentItem.value_2.setValue(null);
        }

        this.visitTextField(col);
    }

    private visitTextField(col: PGridViewColumn) {

        if (col.filterOptions && col.filterOptions.source && col.filterOptions.textField) {
            this.currentItem.multiControl.textField = col.filterOptions.textField;
        }

        this.visitValueField(col);
    }

    private visitValueField(col: PGridViewColumn) {

        this.currentItem.valueChange = col.valueChange;
        if (col.filterOptions && col.filterOptions.source && col.filterOptions.valueField) {
            this.currentItem.multiControl.valueField = col.filterOptions.valueField;
            this.currentItem.multiControl.filterChange = col.filterOptions.filterChange;
        }

        this.visitValidator(col);
    }

    private visitValidator(col: PGridViewColumn) {

        this.currentItem.value_1.setValidators(col.validators);
        this.visitSource(col);
    }

    private visitSource(col: PGridViewColumn) {

        switch (col.dataType) {
            case 'checkbox':
                this.currentItem.multiControl.source = new FormControl([{ text: 'Yes', value: true }, { text: 'No', value: false }]);
                break;
            case 'record_status':
                this.currentItem.multiControl.source = col.filterOptions.source ? new FormControl(col.filterOptions.source)
                    : new FormControl(recordStatusOptions());
                break;
            case 'sales-status':

                this.currentItem.multiControl.source = new FormControl([
                    { text: 'Open', value: 1 },
                    { text: 'Closed', value: 2 }]);
                break;
            default:
                if (col.filterOptions && col.filterOptions.source) {
                    if (col.filterOptions.source instanceof BaseApiService) {
                        this.currentItem.multiControl.source = col.filterOptions.source;
                    } else {
                        this.currentItem.multiControl.source = new FormControl(col.filterOptions.source);
                    }
                }
                break;
        }

        this.addItem();
    }

    private addItem(): void {

        (this.form.get('formItems') as FormArray).push(this.fb.control(this.currentItem));
    }
}