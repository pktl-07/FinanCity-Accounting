import { Component, OnInit, Input, ViewChild, OnDestroy } from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { PGridIncellEditingComponent } from 'src/@common/components/p-grid-incell-editing/p-grid-incell-editing.component';
import { SalespersonManagementService } from 'src/@sales/services/salesperson-management.service';
import { SalespersonManagementDetailComponent } from '../../salesperson-management-detail/salesperson-management-detail.component';
import { WindowState } from '@progress/kendo-angular-dialog';
import { mergeMap, map } from 'rxjs/operators';
import { getComissionContext, multiCurrency, cloneData, SubSink } from 'src/@common/service/utility-service';
import { PageUtilityService } from '../page.utility.service';
import { BehaviorSubject } from 'rxjs';

@Component({
  selector: 'app-property-transaction-internals',
  templateUrl: './property-transaction-internals.component.html',
  providers: [
    PWindowComponentService,
    SalespersonManagementService,
  ]
})
export class PropertyTransactionInternalComponent implements OnDestroy, OnInit {

  @ViewChild('internalGrid', { read: PGridIncellEditingComponent, static: true }) public dataGrid: PGridIncellEditingComponent;
  @Input() formGroup: FormGroup;

  private _subsink = new SubSink()
  private _commissionTypeSubject = new BehaviorSubject([]);
  private _commissionTypes = [];

  constructor(public _windowService: PWindowComponentService,
    private _pageUtilityService: PageUtilityService,
    public _internalSalespersonService: SalespersonManagementService,) {
  }

  ngOnInit(): void {

    this._subsink.sink = this._pageUtilityService.commissionSource.subscribe((data) => {

      if (data.length > 0) {
        this._commissionTypes = cloneData(data);
        this._commissionTypeSubject.next(this._commissionTypes);
      }
    })

    this._initInternalGrid();

    this._subsink.sink = this._pageUtilityService.taxSubject
      .subscribe(tax => {

        this._pageUtilityService.internalSubject.value.forEach((item: any) => {

          item['TaxPercent'] = tax.TaxPercent;
          item['TaxCode'] = tax.Value;
          item['TaxSK'] = tax.Key;

          let context = getComissionContext({ tax: tax.TaxPercent, amount: item['GrossComm'], call: 'gross' })

          item['GSTValue'] = context.taxAmount;
          item['TotalPayable'] = context.total;
        })
      });
  }

  ngOnDestroy(): void {

    this._subsink.unsubscribe();
  }

  public _initInternalGrid() {

    this.dataGrid.config = {
      fetch: this._pageUtilityService.internalSubject,
      key: 'SysKey',
      columns: [
        { dataType: 'hidden', field: 'SysKey', defaultValue: null },
        { dataType: 'hidden', field: 'PTOSK', defaultValue: this.formGroup.get('SysKey').value },
        { dataType: 'hidden', field: 'TransType', defaultValue: 25 },
        { dataType: 'hidden', field: 'CurrentCommission', defaultValue: 0 },
        { dataType: 'hidden', field: 'TaxSK', defaultValue: () => this.formGroup.get('TaxSK').value, width: 120 },
        {
          dataType: 'cascade_checkbox', field: 'IsMain', title: '', defaultValue: false, width: 50, cellValueChange: (c) => {
            if (c.event == true) {
              this.dataGrid.service.data.map((o, i) => {
                o['IsMain'] = false;
              })
              c.fg.get('IsMain').patchValue(true);
            }
          }
        },
        {
          dataType: 'dropdown', field: 'SPSK', title: 'Salesperson Name', validators: [Validators.required], dropdown: {
            fetch: this._internalSalespersonService, textField: 'Name', valueField: 'SysKey', itemEmpty: (i) => this.addNewSalesperson(i)
          }, cellValueChange: (c) => {

            c.fg.get('SPSK').patchValue(c.event);

            this._subsink.sink = this._internalSalespersonService.isTierCommission(c.event)
              .pipe(mergeMap(response => {
                return this._internalSalespersonService.findByKey(c.event)
                  .pipe(map(data => {
                    c.fg.get('NRIC').patchValue(data['Person']['NRIC']);
                    return response;
                  }))
              }))
              .subscribe(tier => {
                if (tier) {

                  this._subsink.sink = this._internalSalespersonService.getCurrentCommSchemeByKey(c.fg.get('SPSK').value)
                    .subscribe(data => {

                      let comm = this._commissionTypes.find(f => f.SysKey == data['SysKey']);
                      c.fg.get('SchemeDetailSK').patchValue(data['SysKey']);
                      this._commissionTypeSubject.next([comm]);
                    })
                } else {

                  this._subsink.sink = this._internalSalespersonService.getCurrentCommSchemeByKey(c.event)
                    .subscribe((current: any) => {
                      c.fg.get('SchemeDetailSK').patchValue(current['SysKey']);
                      this._commissionTypeSubject.next(this._commissionTypes.filter(f => f.ParentSK == current['ParentSK']));
                    })
                }
              })
          },
        },
        { dataType: 'display', field: 'NRIC', title: 'NRIC No.', width: 120, align: 'center' },
        {
          dataType: 'dropdown', field: 'SchemeDetailSK', title: 'Comm. Type', width: 200,
          dropdown: {
            fetch: this._commissionTypeSubject,
            customFetching: async (c) => {
              if (c.data.SchemeDetailSK != null) {

                const current = this._commissionTypes.find(f => f.SysKey == c.data.SchemeDetailSK);
                if (current['Type'] == 4) {

                  this._subsink.sink = this._internalSalespersonService.getCurrentCommSchemeByKey(c.data.SPSK)
                    .subscribe(data => {
                      let com = this._commissionTypes.find(f => f.SysKey == data['SysKey']);
                      this._commissionTypeSubject.next([com]);
                    })
                }
                else {

                  let source = this._commissionTypes.filter(f => f.ParentSK == current['ParentSK']);
                  this._commissionTypeSubject.next(source);
                }
              }
            },
            textField: 'Designation',
            valueField: 'SysKey'
          }
        },
        {
          dataType: 'number', field: 'Share', title: 'Share (%)', defaultValue: 0, width: 120, cellValueChange: (c) => {
            let rate = c.event;
            let totalGross = this.formGroup.get('GrossCommission').value;
            let gross = (totalGross / 100) * rate;
            let tax = c.fg.get('TaxPercent').value;
            let context = getComissionContext({ tax: tax, amount: gross, call: 'gross' })

            c.fg.get('GrossComm').patchValue(context.gross);
            c.fg.get('GSTValue').patchValue(context.taxAmount);
            c.fg.get('TotalPayable').patchValue(context.total);
          }
        },
        {
          dataType: 'currency', field: 'GrossComm', title: 'SP Gross Commission', defaultValue: 0, width: 120, cellValueChange: (c) => {

            let gross = this.formGroup.get('GrossCommission').value;
            let tax = c.fg.get('TaxPercent').value;
            let context = getComissionContext({ tax: tax, amount: c.event, call: 'gross' })

            c.fg.get('Share').patchValue((c.event / gross) * 100);
            c.fg.get('GSTValue').patchValue(context.taxAmount);
            c.fg.get('TotalPayable').patchValue(context.total);
          }
        },//GSTPercent
        {
          dataType: 'hidden', field: 'TaxCode', title: 'GST',
          defaultValue: () => this._pageUtilityService.taxSubject.value.Value, width: 120, align: 'center'
        },
        { dataType: 'hidden', field: 'TaxPercent', align: 'center', defaultValue: () => this._pageUtilityService.taxSubject.value.TaxPercent, title: 'GST (%)', width: 120 },
        { dataType: 'hidden', field: 'GSTValue', title: 'GST ($)', defaultValue: 0, width: 120 },
        {
          dataType: 'hidden', field: 'TotalPayable', title: 'Total', defaultValue: 0, width: 120, cellValueChange: (c) => {

            let total = c.event;
            let gross = this.formGroup.get('GrossCommission').value;
            let tax = c.fg.get('TaxPercent').value;

            let context = getComissionContext({ tax: tax, amount: total, call: 'total' })

            c.fg.get('Share').patchValue((this.formGroup.get('GrossCommission').value / gross) * 100);
            c.fg.get('GSTValue').patchValue(context.taxAmount);
            c.fg.get('GrossComm').patchValue(context.gross);
          }
        },
      ],
      rowDataBound: async (b) => {

        let gross = this.formGroup.get('GrossCommission').value;

        //GSTValue
        //TotalPayable
        b.data['GSTValue'] = multiCurrency({
          op: b.data['CURROP'],
          rate: b.data['CURRATE'],
          value: b.data['GSTValue']
        })

        b.data['GrossComm'] = multiCurrency({
          op: b.data['CURROP'],
          rate: b.data['CURRATE'],
          value: b.data['GrossComm']
        })

        b.data['TotalPayable'] = multiCurrency({
          op: b.data['CURROP'],
          rate: b.data['CURRATE'],
          value: b.data['TotalPayable']
        })

        let rate = (b.data['GrossComm'] / gross) * 100;

        b.data['Share'] = rate;

        let d = await this._internalSalespersonService.findByKey(b.data['SPSK'])
          .toPromise();

        if (d != null) {
          b.data['NRIC'] = d['Person']['NRIC'];
          b.data['CEA'] = d['Person']['CEANumber'];
        }
      }
    };
  }

  addNewSalesperson(i?: (j: any) => any) {

    let state: WindowState = 'maximized';
    let window = this._windowService.open({ title: 'Salesperson Management', componentType: SalespersonManagementDetailComponent, state: state });
    let windowComponent = window.content.instance as SalespersonManagementDetailComponent;
    windowComponent.loadData({ Name: this.dataGrid.dropDownFilterText });

    this._subsink.sink = windowComponent.OnSuccess
      .subscribe(item => {
        if (i) {
          this._windowService.close();
          i(item.data.Person.SysKey);
        }
        window.close();
      })
  }

  lock() {
    return this.formGroup.get('RecordStatus').value == 8 || this.formGroup.get('RecordStatus').value == 6;
  }
}
