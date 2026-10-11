import { Component, OnInit, Input, OnDestroy, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { PGridViewComponent } from 'src/@common/components/p-grid-view/p-grid-view.component';
import { WindowState } from '@progress/kendo-angular-dialog';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { CommissionReceivedAllocationDetailDetailComponent } from '../commission-received-allocation-detail-detail/commission-received-allocation-detail-detail.component';
import { CommissionReceivedAllocationService } from 'src/@sales/services/commission-received-allocation.service';
import { SalespersonManagementService } from 'src/@sales/services/salesperson-management.service';
import { SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-commission-received-allocation-detail-list',
  templateUrl: './commission-received-allocation-detail-list.component.html',
  providers: [
    SalespersonManagementService,
    CommissionReceivedAllocationService,
    PWindowComponentService
  ]
})
export class CommissionReceiveAllocationDetailListComponent implements OnDestroy, OnInit {

  private _subsink = new SubSink();

  @Input() formGroup: FormGroup;
  @ViewChild('commissionReceivedAllocationGrid', { static: true }) _gridView: PGridViewComponent;

  constructor(
    public _windowService: PWindowComponentService,
    public _salespersonManagementService: SalespersonManagementService,
    public _commissionAllocationService: CommissionReceivedAllocationService) {
  }

  ngOnInit(): void {

    this._gridView.pageable = false;
    this._gridView.config = {
      key: 'SysKey',
      methodName: 'asyncFinds',
      filterable: 'none',
      fetch: this._commissionAllocationService,
      insertable: () => this.createNew(null, { PercentComplete: 0 }),
      columns: [
        { dataType: 'button', field: 'SysKey', filter: false, title: '', viewDetail: (a: string, item: any) => this.createNew(a, item) },
        {
          dataType: 'hidden', field: 'HeaderSK', filter: true, title: '', align: 'center', width: 120, filterOptions: {
            defaultValue1: this.formGroup.get('InvoiceSK').value
          }
        },
        { dataType: 'string', field: 'R1', filter: true, title: 'Allocation No.', align: 'center', width: 120 },
        { dataType: 'string', field: 'SPSK', filter: true, title: 'Salesperson Name' },
        { dataType: 'date', field: 'AllocationDate', filter: true, title: 'Post Date', align: 'center', width: 120 },
        { dataType: 'string', field: 'PercentComplete', filter: true, align: 'center', width: 120, title: 'Share (%)' },
        { dataType: 'currency', field: 'AmountBeforeGST', filter: true, width: 120, title: 'Gross Comm.' },
        { dataType: 'number', field: 'GSTPercent', filter: true, width: 120, title: 'GST (%)' },
        { dataType: 'currency', field: 'GSTValue', filter: true, width: 120, title: 'GST ($)' },
        { dataType: 'currency', field: 'AllocateAmount', filter: true, width: 120, title: 'Amt. Allocated' },
        { dataType: 'string', field: 'Remarks', filter: true, title: 'Remarks' },
        { dataType: 'record_status', field: 'RecordStatus', filter: false, title: 'Status' },
        {
          dataType: 'hidden', field: 'TransType', filter: true, width: 120, title: 'Amt. Allocated', filterOptions: {
            defaultValue1: 60
          }
        },
      ],
      rowDataBound: d => {

        if (d.data['RecordStatus'] == 8) {
          this._subsink.sink = this._commissionAllocationService.SalespersonTotalReceivable(d.data['SPSK'], d.data['HeaderSK'])
            .subscribe((c: number) => {
              d.data['PercentComplete'] = (d.data['AllocateAmount'] / this.formGroup.get('TotalReceived').value) * 100;
              //  d.data['PercentComplete'] = (this.formGroup.get('TotalReceived').value / c) * 100;
            })
        } else {
          d.data['PercentComplete'] = '--';
        }

        this._subsink.sink = this._salespersonManagementService.findByKey(d.data['SPSK'])
          .subscribe(person => {
            d.data['SPSK'] = person['Person']['Name'];
          });
      }
    }
  }

  createNew(key = null, item: any = null): Promise<any> {

    return new Promise<any>((resolve) => {

      const state: WindowState = 'default';
      const window = this._windowService.open({ title: 'Commission Allocation', componentType: CommissionReceivedAllocationDetailDetailComponent, state: state });
      const windowComponent = window.content.instance as CommissionReceivedAllocationDetailDetailComponent;
      window.window.instance.width = 800;

      windowComponent.invoiceSK = this.formGroup.get('InvoiceSK').value;
      windowComponent.commissionBalance = this.formGroup.get('TotalReceived').value;
      windowComponent.percentComplete = item['PercentComplete'];

      windowComponent.loadData({ SysKey: key });

      this._subsink.sink = windowComponent.OnSuccess
        .subscribe(() => {
          this._gridView.refresh();
        });
      resolve(null);
    })
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }
}
