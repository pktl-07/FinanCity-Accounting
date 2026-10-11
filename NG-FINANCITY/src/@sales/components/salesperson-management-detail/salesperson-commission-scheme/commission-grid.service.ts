import { Injectable, OnDestroy } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { CommissionStructureDetailService } from 'src/@sales/services/commission-structure-detail.service';
import { kendoFilter, SubSink } from 'src/@common/service/utility-service';

@Injectable()
export class CommissionGridService implements OnDestroy {

    public loading: boolean = false;
    public formArray: FormArray;
    private _fromGroup: FormGroup;
    private _service: CommissionStructureDetailService;
    private _subsink = new SubSink();

    constructor(private _fb: FormBuilder) {

        this.formArray = _fb.array([]);
    }

    ngOnDestroy(): void {
        this._subsink.unsubscribe();
    }

    read(parentsk: string,
        formGroup: FormGroup,
        service: CommissionStructureDetailService) {

        this._service = service;
        this.formArray = this._fb.array([]);
        this._fromGroup = formGroup;

        return new Promise((resolve, reject) => {
            this.loading = true;
            this._subsink.sink = this._service.finds({
                filter: {
                    filters: kendoFilter([{
                        field: 'ParentSK', logic: 'and', operator: 'eq', value: parentsk
                    }]), logic: 'and'
                }
            }).subscribe((response: []) => {

                this.mapFormArray(response);
                this.loading = false;
                resolve(null);
            }, (error) => reject(error));
        })
    }

    private mapFormArray(data: []) {

        //clear form array
        this.formArray.controls = [];
        data.map((o, i) => {
            const control = {
                IsCurrent: new FormControl(false),
                SysKey: new FormControl(o['SysKey']),
                ParentSK: new FormControl(o['ParentSK']),
                Designation: new FormControl(o['Designation']),
                MinCom: new FormControl(o['MinCom']),
                MaxCom: new FormControl(o['MaxCom']),
                AssociatedPercent: new FormControl(o['AssociatedPercent']),
                AgencyPercent: new FormControl(o['AgencyPercent']),
                L1: new FormControl(o['L1']),
                L2: new FormControl(o['L2']),
                L3: new FormControl(o['L3']),
                L4: new FormControl(o['L4']),
                L5: new FormControl(o['L5']),
                Type: new FormControl(o['Type']),
                Disabled: new FormControl(!o['IsActive']),
            };
            //push into formarray
            this.formArray.push(new FormControl(control));
        })
    }

    setDisabled() {

        this.formArray.controls.map((o, i) => {
            //clear all check marks
            (o['value']['Disabled'] as FormControl).setValue(true);
        });
    }

    setEnabled() {

        this.formArray.controls.map((o, i) => {
            //clear all check marks
            (o['value']['Disabled'] as FormControl).setValue(false);
        });
    }

    setDesignation(param: { key?: string, explicitSetOpeningSales?: boolean } =
        { key: null, explicitSetOpeningSales: true }) {

        this.clearCheckMark();

        if (this.data[0].Type == 4) {

            if (param.key == null) {

                for (let i = 0; i < this.data.length; i++) {

                    if (this.data[i].MaxCom == this.total) {

                        const g = this.formArray.controls[i + 1];

                        g['value']['IsCurrent'].setValue(true); // set check state to true
                        break;
                    }
                    if (this.total >= this.data[i].MinCom && this.total <= this.data[i].MaxCom) {

                        const g = this.formArray.controls[i];

                        g['value']['IsCurrent'].setValue(true); // set check state to true
                        break;
                    }
                    if (this.total >= this.data[this.data.length - 1].MaxCom) {

                        const g = this.formArray.controls[this.data.length - 1];

                        g['value']['IsCurrent'].setValue(true); // set check state to true
                        break;
                    }
                }

                if (this.isLast())
                    this._fromGroup.get('GrossToPromoteNext').setValue(0)
                else {
                    let b = this.data[this.currentIndex].MaxCom;
                    this._fromGroup.get('GrossToPromoteNext').setValue(b - this.total);
                }
            }
            else {

                //CloseSalesCommissionToDate
                const g = this.formArray.controls.filter(x => x['value']['SysKey']['value'] == param.key)[0];
                g['value']['IsCurrent'].setValue(true);

                if (this.isFirst()) {

                    if (param.explicitSetOpeningSales == true) {

                        if (this._fromGroup.get('OpeningAccSales').value >= this.data[this.currentIndex].MaxCom)
                            this._fromGroup.get('OpeningAccSales').setValue(0);
                    }

                    this._fromGroup.get('GrossToPromoteNext').setValue(this.data[this.currentIndex].MaxCom - this._fromGroup.get('OpeningAccSales').value);
                }
                else if (this.isLast()) {

                    const a = this.data[this.currentIndex - 1].MaxCom;
                    const b = this.data[this.currentIndex].MaxCom;

                    if (param.explicitSetOpeningSales == true) {
                        this._fromGroup.get('OpeningAccSales').setValue(a);
                    }

                    this._fromGroup.get('GrossToPromoteNext').setValue(0);
                } else {

                    const a = this.data[this.currentIndex - 1].MaxCom;
                    const b = this.data[this.currentIndex].MaxCom;

                    if (param.explicitSetOpeningSales == true) {
                        this._fromGroup.get('OpeningAccSales').setValue(a);
                    }
                    this._fromGroup.get('GrossToPromoteNext').setValue((b + this.total) - a);
                }
            }
            this._fromGroup.get('SchemeDetailSysKey').setValue(this.data[this.currentIndex].SysKey);
        } else {
            if (param.key == null) {

                const key = this.data.filter(x => x.IsActive == true)[0]['SysKey'];
                const g = this.formArray.controls.filter(x => x['value']['SysKey']['value'] == key)[0];

                g['value']['IsCurrent'].setValue(true);
                this._fromGroup.get('SchemeDetailSysKey').setValue(key);
            }
            else {
                const g = this.formArray.controls.filter(x => x['value']['SysKey']['value'] == param.key)[0];
                g['value']['IsCurrent'].setValue(true);
                this._fromGroup.get('SchemeDetailSysKey').setValue(param.key);
            }
        }
    }

    get data() {

        let a = new Array<{ SysKey: string, Designation: string, MinCom: number, MaxCom: number, Type: number, Disabled: boolean, IsActive: boolean }>();
        this.formArray.controls
            .map((o, i) => {
                a.push({
                    SysKey: o.value.SysKey.value,
                    Designation: o.value.Designation.value,
                    MinCom: o.value.MinCom.value,
                    MaxCom: o.value.MaxCom.value,
                    Type: o.value.Type.value,
                    Disabled: o.value.Disabled.value,
                    IsActive: !o.value.Disabled.value
                });
            })

        return a;
    }

    clear() {

        this.formArray = this._fb.array([]);
    }

    clearCheckMark() {

        this.formArray.controls.map((o, i) => {
            //clear all check marks
            (o['value']['IsCurrent'] as FormControl).setValue(false);
        });
    }

    isLast() {
        return this.data[this.data.length - 1].SysKey == this.current.value['SysKey']['value'];
    }

    isFirst() {
        return this.data[0].SysKey == this.current.value['SysKey']['value'];
    }

    get total() {

        const o = this._fromGroup.get('OpeningAccSales').value;
        const c = this._fromGroup.get('CloseSalesCommissionToDate').value;
        const y = this._fromGroup.get('CloseSalesCommissionToYear').value;

        if (this._fromGroup.get('ResetCommissionEffectiveStartDate') != null) {

            return (o + y);
        }
        return (o + c);
    }

    get currentIndex() {
        let idx = 0;
        this.data.map((o, i) => {
            if (o.SysKey == this.current.value['SysKey']['value']) {
                idx = i;
            }
        })
        return idx;
    }

    get current() {

        const g = this.formArray.controls.filter(x => x['value']['IsCurrent']['value']);
        return g[0];
    }
}