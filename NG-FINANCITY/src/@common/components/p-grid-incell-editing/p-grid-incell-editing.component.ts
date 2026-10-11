import { Component, OnInit, ViewChild, ViewEncapsulation, Input, OnDestroy, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, FormArray } from '@angular/forms';
import { GridInCellEditService } from './p-grid-incell-editing-service';
import { DropDownFilterSettings } from '@progress/kendo-angular-dropdowns';
import { PGridIncellEditingConfig, PGridInCellColumn, PGridInCellDropDownColumn } from 'src/@common/Infrastructure/common-interface';
import { TooltipDirective } from '@progress/kendo-angular-tooltip';
import { Subject, BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { kendoFilter, SubSink } from 'src/@common/service/utility-service';
import { BaseApiService } from 'src/@common/Infrastructure/base.api.service';
import { GridComponent } from '@progress/kendo-angular-grid';

@Component({
    selector: 'p-grid-incell-editing',
    templateUrl: './p-grid-incell-editing.component.html',
    encapsulation: ViewEncapsulation.None,
    styles: [`

    .lock{
        pointer-events: none;
    }

    .fc-grid-edit > .k-grid td {
        height: 45px;
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
    }

    tr .text-center {
        text-align:center
    }

    tr .text-right {
        text-align:right
    }`],
    providers: [GridInCellEditService]
})
export class PGridIncellEditingComponent implements OnDestroy, OnInit {

    @ViewChild(TooltipDirective) public tooltipDir: TooltipDirective;
    @ViewChild('grid', { read: GridComponent, static: true }) public dataGrid: GridComponent;
    @Input() lock: boolean = false;

    editedRowIndex: number;
    editedRowItem: any;
    formGroup: FormGroup;
    view: any;
    forms: FormArray;

    private _subsink = new SubSink();
    public _config: PGridIncellEditingConfig = { key: '', columns: [], fetch: null, rowDataBound: null };

    @Input() title: string;
    @Input() disabledAddNewButton: boolean;
    @Input() disableDeleteButton: (dataItem: any) => boolean = () => false;

    @Input('config')
    get config(): PGridIncellEditingConfig {
        return this._config;
    }
    set config(value: PGridIncellEditingConfig) {
        this._config = value;
    }

    @Output('ready') ready = new EventEmitter();

    constructor(public formBuilder: FormBuilder,
        public service: GridInCellEditService) {
    }

    ngOnInit(): void {
        this._init();
        this.ready.emit(this.service)
    }

    ngOnDestroy(): void {

        this._subsink.unsubscribe();
        this._timeout.forEach((timeout) => clearTimeout(timeout))
    }

    public _init() {

        this.view = this.service.pipe(map(data => data));
        this.service.read(this.config);
        this.config.columns.filter(x => x.dataType == "dropdown")
            .forEach((column) => {
                if (column.dropdown.fetch instanceof BaseApiService) {
                    this._subsink.sink = column.dropdown.fetch.subscribe();
                }
            })
        this._subsink.sink = this.view
            .subscribe((source: any[]) => {
                source.forEach(data => {
                    this.config.columns.filter(x => x.dataType == "dropdown"
                        && (x.dropdown.fetch instanceof BaseApiService || x.dropdown.fetch instanceof BehaviorSubject))
                        .forEach((column) => {

                            //important condition to save the datagrid view performance
                            //to prevent Api call unless necessary whenever grid-data has changed.
                            if (typeof data[column.field + '_item'] == 'undefined') {
                                //create drop-down item when it is undefined for the first time beging created
                                this.patchDropDownItem({ column: column, data: data });
                            } else {
                                //check whatever associated grid-dropdown change or not
                                //if yes then the patch new value
                                if (data[column.field + '_item'][column.dropdown.valueField] != data[column.field]) {
                                    this.patchDropDownItem({ column: column, data: data });
                                }
                            }
                        })
                })
            })
    }

    public itemDisabled(itemArgs: { dataItem: any, index: number }) {

        if (typeof itemArgs.dataItem.IsActive != 'undefined')
            return !itemArgs.dataItem.IsActive;

        return false;
    }

    cascadeChange(e: { dataItem: any, event: any, column: PGridInCellColumn, isNew: boolean, rowIndex: number }) {

        //should unselected all the cascade-checkbox before it update value.
        this.service.data.map((o, i) => { o[e.column.field] = false })
        this.service.createdItems.map((o, i) => { o[e.column.field] = false })
        this.service.updatedItems.map((o, i) => { o[e.column.field] = false })
        this.service.deletedItems.map((o, i) => { o[e.column.field] = false })

        if (e.column.cellValueChange !== undefined)
            e.column.cellValueChange({ dataItem: e.dataItem, fg: this.formGroup, column: e.column, event: e.event.target.checked, isNew: e.isNew, rowIndex: e.rowIndex });
    }

    dropdownOpen(e: { dataItem: any, event: any, column: PGridInCellColumn, isNew: boolean, rowIndex: number }) {
        if (typeof e.column.dropdownOpen != 'undefined')
            e.column.dropdownOpen({ dataItem: e.dataItem, fg: this.formGroup, column: e.column, event: e.event, isNew: e.isNew, rowIndex: e.rowIndex });
    }

    dropdownClose(e: { dataItem: any, event: any, column: PGridInCellColumn, isNew: boolean, rowIndex: number }) {
        //  const { formGroup, dataItem, rowIndex } = args;
        // if (!e.isNew)
        //     this.cellCloseHandler({ formGroup: this.formGroup, dataItem: e.dataItem, rowIndex: e.rowIndex })
    }

    cellValueChange(e: { dataItem: any, event: any, column: PGridInCellColumn, isNew: boolean, rowIndex: number }) {

        if (e.column.cellValueChange != undefined && e.column.dropdown == undefined)
            e.column.cellValueChange({ dataItem: e.dataItem, fg: this.formGroup, column: e.column, event: e.event, isNew: e.isNew, rowIndex: e.rowIndex })
        else {
            e.column.cellValueChange({ dataItem: e.dataItem, fg: this.formGroup, column: e.column, event: e.event, isNew: e.isNew, rowIndex: e.rowIndex })
        }
    }

    patchDropDownItem(e: { column: PGridInCellColumn, data: any }) {

        if (e.column.dropdown.fetch instanceof BaseApiService
            && typeof e.column.dropdown.customFetching == 'undefined') {

            this._subsink.sink = e.column.dropdown.fetch.finds({
                filter: {
                    logic: 'and',
                    filters: kendoFilter([
                        {
                            field: e.column.dropdown.valueField,
                            operator: 'eq',
                            logic: 'and',
                            value: e.data[e.column.field]
                        }
                    ])
                }
            }).subscribe((s: any[]) => {
                if (s.length > 0) {
                    e.data[e.column.field + '_item'] = s[0];
                }
                else {
                    e.data[e.column.field + '_item'] = {};
                }
            })
        } else if (e.column.dropdown.fetch instanceof BehaviorSubject && e.column.dropdown.customFetching) {

            let s = (e.column.dropdown.fetch.value as any[]);
            if (s.length > 0) {
                let data = s.find(x => x[e.column.dropdown.valueField] == e.data[e.column.field]);
                e.data[e.column.field + '_item'] = data;
            } else {
                e.data[e.column.field + '_item'] = {};
            }
        }
    }

    private _dropDownFilterText: string;
    public get dropDownFilterText(): string {
        return this._dropDownFilterText;
    }
    public set dropDownFilterText(value: string) {
        this._dropDownFilterText = value;
    }

    filterDropDown(e: { dropDown: PGridInCellDropDownColumn, value: string }) {

        this._dropDownFilterText = e.value;

        if (e.value.length >= 3) {

            if (e.dropDown.customFilter) {
                e.dropDown.customFilter(e);
            }
            else if (e.dropDown.fetch instanceof BaseApiService
                && typeof e.dropDown.customFetching == 'undefined') {

                e.dropDown.fetch
                    .asyncFinds({
                        filter: {
                            logic: 'and',
                            filters: kendoFilter([
                                { field: e.dropDown.textField, operator: 'contains', value: e.value, logic: 'and' }
                            ])
                        }
                    })
            }
        }
        if (e.value.length == 0 && e.dropDown.fetch instanceof BaseApiService
            && typeof e.dropDown.customFetching == 'undefined') {

            e.dropDown.fetch.next(null);
        }

        return null;
    }

    showTooltip(e: MouseEvent): void {

        const element = e.target as HTMLElement;
        if ((element.nodeName === 'TD' || element.nodeName === 'TH')
            && element.offsetWidth < element.scrollWidth) {
            this.tooltipDir.toggle(element);
        } else {
            this.tooltipDir.hide();
        }
    }

    filterSettings: DropDownFilterSettings = {
        caseSensitive: false,
        operator: 'contains'
    };

    canEditCell(column: PGridInCellColumn) {

        if (column.clickable == undefined) {
            return false;
        }
        return !column.clickable;
    }

    addDropDownItem(e: { dataItem: any, column: PGridInCellColumn, cell: PGridInCellDropDownColumn, isNew: boolean, rowIndex: number }) {

        e.cell.itemEmpty((item) => {

            if (typeof item === 'object') {
                alert('The return type should not be json type. Please check the return function for empty template item template in the datagrid.');
                return;
            }
            if (e.cell.fetch instanceof BaseApiService && typeof e.cell.customFetching == 'undefined') {
                //refresh dropdown
                e.cell.fetch.asyncFinds({
                    filter: {
                        logic: 'and',
                        filters: kendoFilter([{
                            field: e.column.dropdown.valueField,
                            operator: 'eq',
                            value: item,
                            logic: 'and'
                        }])
                    }
                })
            }
            //set current form-group field
            this.formGroup.get(e.column.field).patchValue(item);
            //notify cell value change
            if (e.column.cellValueChange !== undefined)
                e.column.cellValueChange({ dataItem: e.dataItem, fg: this.formGroup, column: e.column, event: item, isNew: e.isNew, rowIndex: e.rowIndex });
        });
    }

    public cellClickHandler({ sender, rowIndex, columnIndex, dataItem, isEdited }) {

        if (this.config.canEditRow) {
            let editable = this.config.canEditRow({ columnIndex: columnIndex, dataItem: dataItem, isEdited: isEdited, rowIndex: rowIndex });
            if (editable) {
                sender.editCell(rowIndex, columnIndex, this.createFormGroup(dataItem));
            }
        } else {
            if (!isEdited) {
                sender.editCell(rowIndex, columnIndex, this.createFormGroup(dataItem));
            }
        }
    }

    private _timeout = [];
    public cellCloseHandler(args: any) {

        const { formGroup, dataItem, rowIndex } = args;
        if (!formGroup.valid) {
            // prevent closing the edited cell if there are invalid values.
            args.preventDefault();
        } else if (formGroup.dirty) {

            this.config.columns.map(async m => {

                if (m.dataType == 'dropdown'
                    && m.dropdown.fetch instanceof BaseApiService
                    && typeof m.dropdown.customFetching == 'undefined') {

                    this._subsink.sink = m.dropdown.fetch.finds({
                        filter: {
                            filters: kendoFilter([
                                {
                                    field: m.dropdown.valueField,
                                    operator: 'eq',
                                    value: formGroup.get(m.field).value,
                                    logic: 'and'
                                }
                            ]),
                            logic: 'and'
                        }
                    }).subscribe(d => {
                        formGroup.get(`${m.field}_item`).patchValue(d[0]);
                    })
                }
                else if (m.dataType == 'dropdown'
                    && m.dropdown.fetch instanceof BehaviorSubject
                    && typeof m.dropdown.customFetching) {

                    let data = (m.dropdown.fetch.value as any[]).find(x => x[m.dropdown.valueField] == formGroup.get(m.field).value);
                    formGroup.get(`${m.field}_item`).patchValue(data);
                }
            });
            let timeout = setTimeout(() => {

                this.service.assignValues(dataItem, formGroup.value);
                this.service.update(formGroup.value);
                if (this.config.updateCompleted) {
                    this.config.updateCompleted({ formGroup: formGroup, rowIndex: rowIndex });
                }
                this.dataGrid.closeRow(rowIndex);
            }, 100);
            this._timeout.push(timeout)
        }
    }

    closeEditor(grid, rowIndex = this.editedRowIndex) {

        grid.closeRow(rowIndex);
        this.editedRowIndex = undefined;
        this.formGroup = undefined;
    }

    addHandler({ sender }) {
        sender.addRow(this.createFormGroup());
    }

    cancelHandler({ sender, rowIndex }) {

        this.closeEditor(sender, rowIndex);
    }

    editHandler({ sender, rowIndex, dataItem, column }: any): void {

        this.editedRowItem = dataItem;
        this.editedRowIndex = rowIndex;
        this.createFormGroup(dataItem);
        sender.editRow(rowIndex, this.formGroup);
    }

    saveHandler({ sender, formGroup, rowIndex, isNew }) {

        if (formGroup.valid) {

            if (isNew) this.service.create(formGroup.value);
            else this.service.update(formGroup.value);

            sender.closeRow(rowIndex);

            if (isNew)
                if (this.config.saveCompleted)
                    this.config.saveCompleted({ sender: sender, formGroup: formGroup, rowIndex: rowIndex, isNew: isNew });
                else
                    if (this.config.updateCompleted)
                        this.config.updateCompleted({ formGroup: formGroup, rowIndex: rowIndex });
        }
    }

    removeHandler({ sender, formGroup, dataItem, rowIndex }) {

        this.service.remove(dataItem, rowIndex);

        if (this.config.deleteCompleted)
            this.config.deleteCompleted({ sender: sender, formGroup: formGroup, rowIndex: rowIndex });

        sender.cancelCell();
    }

    canDeleteRow({ rowIndex, dataItem }) {

        if (this.config.canDeleteRow) {
            return !this.config.canDeleteRow({ rowIndex: rowIndex, dataItem: dataItem });
        }
        return true;
    }

    cancelChanges(grid: any): void {
        grid.cancelCell();

        this.service.cancelChanges();
    }

    isBaseApiService(item: any) {
        return item instanceof BaseApiService;
    }

    isBehaviourSubject(item: any) {
        return item instanceof BehaviorSubject;
    }

    isCustomFetchingDropDown(column: PGridInCellDropDownColumn) {
        return typeof column.customFetching !== 'undefined';
    }

    getDropDownItem(column: PGridInCellDropDownColumn, item: any) {

        if (column.customFetching && column.fetch instanceof BehaviorSubject) {
            return (column.fetch.value as any[]).find(x => x[column.valueField] == item)[column.textField];
        } else {
            return (column.fetch as any[]).find(x => x[column.valueField] == item)[column.textField];
        }
    }

    createFormGroup(dataItem?: any): FormGroup {

        let d = {};
        if (typeof dataItem === 'undefined') {
            this.config.columns.forEach(f => {

                if (f.defaultValue instanceof Subject) {
                    this._subsink.sink = f.defaultValue.subscribe(data => {
                        d[f.field] = new FormControl(data, f.validators);
                    });
                }
                else if (typeof f.defaultValue === 'function') {
                    d[f.field] = new FormControl(f.defaultValue(), f.validators);
                }
                else {
                    d[f.field] = new FormControl(f.defaultValue, f.validators);
                }
                //want to set true value to all cascade_checkbox
                //when the first item being added
                if (this.service.data.length == 0) {
                    if (f.dataType == 'cascade_checkbox') {
                        (d[f.field] as FormControl).setValue(true);
                    }
                }
            });
        } else {
            this.config.columns.forEach(column => {

                //fetch the dropdown async its save the dropdown performance.
                // when the data retrive is large increasing size then it cas the performace slow
                if (column.dataType == 'dropdown') {

                    let m = column.dropdown.fetch;
                    if (m instanceof BehaviorSubject && column.dropdown.customFetching) {

                        column.dropdown.customFetching({ data: dataItem });
                    }
                    else if (m instanceof BaseApiService && typeof column.dropdown.customFetching === 'undefined') {
                        m.asyncFinds({
                            filter: {
                                logic: 'and',
                                filters: kendoFilter([
                                    {
                                        field: column.dropdown.valueField,
                                        operator: 'eq',
                                        value: dataItem[column.field],
                                        logic: 'and'
                                    }
                                ])
                            }
                        });
                    }

                    //check whatever grid-data dropdown is the first time being created
                    //unless bind it to the formcontrol
                    //additional data will be carried when the grid row has changed parmenant
                    if (dataItem[column.field + '_item'] != undefined) {
                        d[column.field + '_item'] = new FormControl(dataItem[column.field + '_item'], column.validators)
                    }
                }
                //assign new form control into json 
                d[column.field] = new FormControl(dataItem[column.field], column.validators)
            })
        }
        //poppulate current form control
        this.formGroup = this.formBuilder.group(d);
        return this.formGroup;
    }

    toUpperCase(value: string) {

        if (value)
            return value.toUpperCase();

        return value;
    }

    public isundefined(...value: any[]) {

        return value.filter(x => typeof x == 'undefined').length > 0;
    }
}

