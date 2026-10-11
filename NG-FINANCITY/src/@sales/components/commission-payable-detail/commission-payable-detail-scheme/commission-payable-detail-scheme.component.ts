import { Component, OnInit, ViewChild, OnDestroy, Input } from '@angular/core';
import { PGridViewComponent } from 'src/@common/components/p-grid-view/p-grid-view.component';
import { PayableSchemeService } from 'src/@sales/services/payable-scheme.service';
import { PropertyTransactionDetailService } from 'src/@sales/services/property-transaction-detial.service';
import { getComissionContext, kendoFilter, SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-commission-payable-detail-scheme',
  templateUrl: './commission-payable-detail-scheme.component.html',
  providers: [
    PayableSchemeService,
    PropertyTransactionDetailService
  ]
})
export class CommissionPayableDetailSchemeComponent implements OnInit, OnDestroy {

  @ViewChild('commissionScheme', { static: true }) _gridView: PGridViewComponent;

  @Input('InvoiceSK') public InvoiceSK: string;
  @Input('SPSK') public SPSK: string;
  @Input('ALOSK') public ALOSK: string;

  private _subsink = new SubSink();

  constructor(
    public _propertyDetailService: PropertyTransactionDetailService,
    public _payableSchemeService: PayableSchemeService) {
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  ngOnInit() {

    this._gridView.pageable = false;
    this._gridView.config = {
      key: 'SysKey',
      filterable: 'none',
      fetch: this._payableSchemeService,
      columns: [
        { dataType: 'currency', field: 'GrossComm', filter: true, title: 'SP Gross Comm.', width: 120 },
        { dataType: 'string', field: 'AgencyPercent', filter: true, title: 'Company (%)', align: 'center', width: 120 },
        { dataType: 'currency', field: 'AgencyComm', filter: true, title: 'Company Fee', width: 120 },
        { dataType: 'currency', field: 'AssociateComm', filter: true, title: 'SP Comm. Payable' },
      ], rowDataBound: d => {

        this._subsink.sink = this._propertyDetailService.finds({
          filter: {
            filters: this._getPropertyDetailCriteria(),
            logic: 'and'
          }
        }).subscribe(detail => {

          if (detail[0]['TransType'] == 20) {

            let taxDetail = getComissionContext({
              tax: detail[0]['TaxPercent'], amount: d.data['AssociateComm'], call: 'total'
            });

            d.data['GrossComm'] = taxDetail.gross;
            d.data['AssociateComm'] = taxDetail.gross;
          }
        });
      }
    }

    this._payableSchemeService.asyncFinds({
      filter: {
        filters: this._getParameter(),
        logic: 'and'
      }
    });
  }

  private _getPropertyDetailCriteria() {

    return kendoFilter([
      { field: 'PTOSK', value: this.InvoiceSK, operator: 'eq', logic: 'and' },
      { field: 'SPSK', value: this.SPSK, operator: 'eq', logic: 'and' }
    ])
  }

  private _getParameter() {

    return kendoFilter([
      { field: 'ALOSK', value: this.ALOSK, operator: 'eq', logic: 'and' },
    ])
  }
}
