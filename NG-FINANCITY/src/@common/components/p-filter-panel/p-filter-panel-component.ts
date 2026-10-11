import { Component, OnInit, Input, ElementRef, TemplateRef, ViewChild, ViewContainerRef, OnDestroy } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { PopupRef, PopupService } from '@progress/kendo-angular-popup';
import { DropDownFilterSettings } from '@progress/kendo-angular-dropdowns';
import { Subject, BehaviorSubject } from 'rxjs';
import { FilterItemTraversalService } from './p-filter-item-traverse-service';
import { CompositeFilterDescriptor } from '@progress/kendo-data-query';

@Component({
    selector: 'p-filter',
    templateUrl: './p-filter-panel-component.html',
    styles: [`
        #div-popup a:hover{
            background-color: #ededed;
        }

        #div-popup a:active {
            background-color: var(--primary);
            color: white;
        }

        #div-popup a{
            cursor:pointer;
        }

        .operator-select {
            background-color: #ededed;
            font-weight: bold;
        }

        #div-popup a hover {
            cursor: pointer;
        }

        #div-popup a:hover {
            background-color: var(--primary);
        }
    `],
    providers: [
        PopupService,
        FilterItemTraversalService
    ]
})
export class FilterComponent implements OnInit, OnDestroy {


    @ViewChild('container', { read: ViewContainerRef, static: true }) public container: ViewContainerRef;
    @Input() form: FormGroup;
    @Input() onFilter: (data: CompositeFilterDescriptor[]) => any[];
    @Input() type: 1 | 2 = 1;

    _unsubscribe: Subject<any> = new Subject<any>();

    constructor(public popupService: PopupService,
        public traversalService: FilterItemTraversalService) {
    }

    ngOnInit(): void {

        document.onclick = (e) => {
            if (this.popupRef != null) {
                this.popupRef.close();
                this.popupRef == null;
            }
        };
    }

    ngOnDestroy(): void {

        this._unsubscribe.next();
        this._unsubscribe.complete();
        clearTimeout(this.toggleTimeout)
    }

    public filterSettings: DropDownFilterSettings = {
        caseSensitive: false,
        operator: 'contains'
    }

    private popupRef: PopupRef;
    private toggleTimeout: any;
    public togglePopup(anchor: ElementRef, template: TemplateRef<any>) {

        this.toggleTimeout = setTimeout(() => {
            this.popupRef = this.popupService.open({
                anchor: anchor,
                appendTo: this.container,
                content: template
            });
        }, 1);
    }

    public onClearClick(item: any) {

        (item.value.value_1 as FormControl).setValue(null);
        (item.value.value_2 as FormControl).setValue(null);

        if (item.value.operator.value == 'isempty' || item.value.operator.value == 'isnotempty') {

            let i = (item.value.operators as []).find(x => x['value'] == 'contains');
            item.value.operator = i;
        }
        if (item.value.operator.value == 'isnull' || item.value.operator.value == 'isnotnull') {

            let i = (item.value.operators as []).find(x => x['value'] == 'bwt');
            item.value.operator = i;
        }
    }

    public onPopupItemClick(data: { bindItem: FormControl, selectedItem: any }) {

        this.popupRef.close();
        this.popupRef = null;
        data.bindItem.value.operator = data.selectedItem;
    }

    public isBaseApi(value: any) {

        return value instanceof BehaviorSubject;
    }

    fetchSubject: Subject<any> = new Subject();
    public handleClose() {

        let state = this.traversalService.traverseFilter(this.form.get('formItems').value)
        if (this.onFilter) {
            state = this.onFilter(state);
        }
        this.fetchSubject.next(state);
    }

    public bindValueField(data: { bindItem: FormControl }) {

        if (data.bindItem.value.multiControl && data.bindItem.value.multiControl.source
            && data.bindItem.value.multiControl.valueField) {

            return data.bindItem.value.multiControl.valueField;
        } else {

            return data.bindItem.value.field;
        }
    }

    public bindTextField(data: { bindItem: FormControl }) {

        if (data.bindItem.value.multiControl && data.bindItem.value.multiControl.source
            && data.bindItem.value.multiControl.textField) {

            return data.bindItem.value.multiControl.textField;
        } else {
            return data.bindItem.value.field;
        }
    }

    public handleFilterChange(param: { source?: any, method: any, text: string }) {

        if (param.text.length > 2 && param.method) {
            param.method(param.text);
        }
        if (param.text.length == 0) {
            param.method("undefined undefined undefined.")
        }
    }

    public handleValueChange(param: { method: any, value: any }) {

        if (param.method)
            param.method(param.value)
    }

    public dropDownDefaultItem(form: FormControl) {

        let d = {};
        d[form.value.multiControl.valueField] = null;
        d[form.value.multiControl.textField] = 'All'
        return d;
    }
}
