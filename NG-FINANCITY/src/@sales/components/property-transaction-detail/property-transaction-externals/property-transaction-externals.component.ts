import { Component, OnInit, Input, ViewChild, Inject, OnDestroy } from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { PGridIncellEditingComponent } from 'src/@common/components/p-grid-incell-editing/p-grid-incell-editing.component';
import { ExternalSalespersonService } from 'src/@sales/services/external-salesperson.service';
import { WindowState } from '@progress/kendo-angular-dialog';
import { VendorService } from 'src/@sales/services/vendor.service';
import { TaxService } from 'src/@accounts/services/tax.service';
import { getComissionContext, SubSink } from 'src/@common/service/utility-service';
import { PageUtilityService } from '../page.utility.service';
import { ExternalSalespersonDetailComponent } from '../../external-salesperson-detail/external-salesperson-detail.component';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-property-transaction-externals',
  templateUrl: './property-transaction-externals.component.html',
  providers:
    [
      TaxService,
      ExternalSalespersonService,
      VendorService,
      PWindowComponentService
    ]
})
export class PropertyTransactionExternalComponent implements OnDestroy, OnInit {

  @ViewChild('externalGrid', { read: PGridIncellEditingComponent, static: true }) public dataGrid: PGridIncellEditingComponent;
  @Input() formGroup: FormGroup;

  private _subsink = new SubSink();

  constructor(public _windowService: PWindowComponentService,
    private _pageUtilityService: PageUtilityService,
    public _taxService: TaxService,
    public _vendorService: VendorService,
    public _externalSalespersonService: ExternalSalespersonService) {
  }

  ngOnInit(): void {
    this._initExternalGrid();
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  public _initExternalGrid() {

    this.dataGrid.config = {
      fetch: this._pageUtilityService.externalSubject,
      key: 'SysKey',
      columns: [
        { dataType: 'hidden', field: 'SysKey', defaultValue: null },
        { dataType: 'hidden', field: 'PTOSK', defaultValue: this.formGroup.get('SysKey').value },
        { dataType: 'hidden', field: 'TransType', defaultValue: 20 },
        { dataType: 'hidden', field: 'TaxSK', defaultValue: environment.defaultPurchaseTax.taxSK },
        {
          dataType: 'dropdown', field: 'SPSK', title: 'Salesperson Name', validators: Validators.required, dropdown: {
            fetch: this._externalSalespersonService, textField: 'Name', valueField: 'SysKey', itemEmpty: (i) => this.addNewExternal(i)
          }, cellValueChange: (c) => {

            this._subsink.sink = this._externalSalespersonService.findByKey(c.event)
              .subscribe((data) => {

                c.fg.get('NRIC').setValue(data['Person']['NRIC']);
                this._vendorService.findByKey(data['Person']['VendorSK']).subscribe((vendor: any) => {
                  c.fg.get('CompanyName').setValue(vendor['Vendor']['CompanyName']);

                  if (vendor['Vendor']['TaxSK'] != null) {
                    c.fg.get('TaxSK').setValue(vendor['Vendor']['TaxSK']);

                    this._subsink.sink = this._taxService.findByKey(vendor['Vendor']['TaxSK'])
                      .subscribe((tax: any) => {

                        c.fg.get('HasGST').setValue(tax.TaxPercent > 0);
                        c.fg.get('TaxPercent').setValue(tax.TaxPercent);

                        let gross = c.fg.get('GrossComm').value;
                        let context = getComissionContext({ tax: tax.TaxPercent, amount: gross, call: 'gross' })

                        c.fg.get('GSTValue').setValue(context.taxAmount);
                        c.fg.get('TotalPayable').setValue(context.total);
                      });
                  } else {

                    c.fg.get('HasGST').setValue(false);
                    c.fg.get('TaxSK').setValue(null);
                    c.fg.get('TaxPercent').setValue(0);

                    let gross = c.fg.get('GrossComm').value;
                    let context = getComissionContext({ tax: 0, amount: gross, call: 'gross' })

                    c.fg.get('GSTValue').setValue(context.taxAmount);
                    c.fg.get('TotalPayable').setValue(context.total);
                  }
                });
              })
          },
        },
        { dataType: 'display', field: 'NRIC', title: 'NRIC No.', width: 120, align: 'center' },
        { dataType: 'display', field: 'CompanyName', title: 'Company Name', width: 120 },
        {
          dataType: 'number', field: 'Share', title: 'Share (%)', defaultValue: 0, width: 120, cellValueChange: (c) => {

            let rate = c.event;
            let totalGross = this.formGroup.get('GrossCommission').value;
            let gross = (totalGross / 100) * rate;
            let tax = c.fg.get('TaxPercent').value;
            let context = getComissionContext({ tax: tax, amount: gross, call: 'gross' })

            c.fg.get('GrossComm').setValue(context.gross);
            c.fg.get('GSTValue').setValue(context.taxAmount);
            c.fg.get('TotalPayable').setValue(context.total);
          }
        },
        {
          dataType: 'dispaly/checkbox', field: 'HasGST', title: 'GST', defaultValue: true, width: 120
        },
        { dataType: 'hidden', field: 'TaxPercent', align: 'center', defaultValue: environment.defaultPurchaseTax.taxPercent, title: 'GST (%)', width: 120 },
        { dataType: 'hidden', field: 'GSTValue', title: 'GST ($)', defaultValue: 0, width: 120 },
        {
          dataType: 'currency', field: 'GrossComm', title: 'SP Gross Commission', defaultValue: 0, width: 120, cellValueChange: (c) => {

            let gross = c.event;
            let totalGross = this.formGroup.get('GrossCommission').value;
            let tax = c.fg.get('TaxPercent').value;

            let context = getComissionContext({ tax: tax, amount: gross, call: 'gross' })

            c.fg.get('Share').setValue((gross / totalGross) * 100);
            c.fg.get('GSTValue').setValue(context.taxAmount);
            c.fg.get('TotalPayable').setValue(context.total);
          }
        },
        {
          dataType: 'hidden', field: 'TotalPayable', title: 'Total', defaultValue: 0, width: 120, cellValueChange: (c) => {

            let total = c.event;
            let totalGross = this.formGroup.get('GrossCommission').value;
            let tax = c.fg.get('TaxPercent').value;
            let context = getComissionContext({ tax: tax, amount: total, call: 'total' })

            c.fg.get('Share').setValue((context.gross / totalGross) * 100);
            c.fg.get('GSTValue').setValue(context.taxAmount);
            c.fg.get('GrossComm').setValue(context.gross);
          }
        },
      ],
      rowDataBound: (b) => {

        this._subsink.sink = this._externalSalespersonService.findByKey(b.data['SPSK'])
          .subscribe(d => {
            b.data['NRIC'] = d['Person']['NRIC'];
          })

        let gross = this.formGroup.get('GrossCommission').value;
        let rate = (b.data['GrossComm'] / gross) * 100;
        let tax = b.data['TaxSK'];

        this._subsink.sink = this._taxService.findByKey(tax)
          .subscribe(t => {
            if (t.TaxPercent)
              b.data['HasGST'] = t.TaxPercent > 0;
            else
              b.data['HasGST'] = false;
          });
        b.data['Share'] = rate;
      }
    };
  }

  addNewExternal(i?: (_: any) => any) {

    let state: WindowState = 'default';
    let window = this._windowService.open({ title: 'External Salesperson Management', componentType: ExternalSalespersonDetailComponent, state: state });
    let windowComponent = window.content.instance as ExternalSalespersonDetailComponent;

    windowComponent.loadData({ Name: this.dataGrid.dropDownFilterText });
    this._subsink.sink = windowComponent.OnSuccess
      .subscribe(item => {
        if (i) {
          i(item.data.Person.SysKey);
          this._windowService.close();
        }
      });
  }

  lock() {
    return this.formGroup.get('RecordStatus').value == 8 || this.formGroup.get('RecordStatus').value == 6;
  }
}
