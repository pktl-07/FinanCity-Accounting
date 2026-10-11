import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { PropertyTransactionService } from 'src/@sales/services/property-transaction.service';
import { mergeMap, tap, map } from 'rxjs/operators';
import { SalesInvoiceService } from 'src/@sales/services/sales-invoice.service';
import { CommissionReceivedService } from 'src/@sales/services/commission-received.service';
import { kendoFilter, SubSink } from 'src/@common/service/utility-service';
import { CommissionPayableService } from 'src/@sales/services/commission-payable.service';
import { CommissionReceivedAllocationService } from 'src/@sales/services/commission-received-allocation.service';
import { SalespersonManagementService } from 'src/@sales/services/salesperson-management.service';

export interface ITransactionActivity {
  TransactionDescription: string,
  Date: string,
  TransactionRef: string,
  Amount: number,
  RecordStatus: number
};

@Component({
  selector: 'app-property-transaction-activity',
  templateUrl: './property-transaction-activity.component.html',
  providers: [
    PropertyTransactionService,
    SalesInvoiceService,
    CommissionReceivedService,
    CommissionPayableService,
    SalespersonManagementService,
  ]
})
export class PropertyTransactionActivityComponent implements OnInit, OnDestroy {

  @Input() PropertySK: string;
  private _subsink = new SubSink();

  gridResult = new BehaviorSubject<any[]>([]);
  receivableStatus = new BehaviorSubject(1);
  payableStatus = new BehaviorSubject(1);

  constructor(
    private _propertyTransactionService: PropertyTransactionService,
    private _salesInvoiceService: SalesInvoiceService,
    private _commissionReceivedService: CommissionReceivedService,
    private _commissionAllocationService: CommissionReceivedAllocationService,
    private _commissionPayableService: CommissionPayableService,
    private _salespersonService: SalespersonManagementService,
  ) { }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  readonly gridConfig = {
    fetch: this.gridResult,
    filterable: 'none',
    columns: [
      { dataType: 'string', field: 'TransactionDescription', title: 'Transaction Description' },
      { dataType: 'date', field: 'Date', width: 120, title: 'Date' },
      { dataType: 'string', field: 'TransactionRef', width: 120, title: 'Transaction Reference Number' },
      { dataType: 'currency', field: 'Amount', title: 'Amount' },
      {
        dataType: 'record_status', field: 'RecordStatus', width: 120, title: 'Status', filterOptions: {
          defaultValue1: [-1, 1, 2, 3, 6, 8],
        },
      },
    ]
  }

  ngOnInit(): void {

    this.refresh();
  }

  refresh(key?: string) {

    const _key = key ? key : this.PropertySK;

    let result = new Array<ITransactionActivity>();

    this._subsink.sink = this._propertyTransactionService.findByKey(_key)
      .pipe(tap((data) => {
        result.push({
          TransactionDescription: 'PROPERTY TRANSACTION ORDER',
          Date: data['PropertyTransaction']['TransactionDate'],
          TransactionRef: data['PropertyTransaction']['R1'],
          Amount: data['PropertyTransaction']['TotalAmount'],
          RecordStatus: data['PropertyTransaction']['RecordStatus'],
        })
      }),
        mergeMap(() => {
          return this._salesInvoiceService.findByKey(_key)
            .pipe(tap(data => {

              if (data['SalesInvoice'] != null) {

                this.receivableStatus.next(data['SalesInvoice']['ReceivableStatus']);
                this.payableStatus.next(data['SalesInvoice']['PayableStatus']);

                result.push({
                  TransactionDescription: 'SALES INVOICE',
                  Date: data['SalesInvoice']['InvoiceDate'],
                  TransactionRef: data['SalesInvoice']['R1'],
                  Amount: data['SalesInvoice']['ActualAmt'],
                  RecordStatus: data['SalesInvoice']['RecordStatus'],
                })
              }
            }))
        }),
        mergeMap(() => {
          return this._commissionReceivedService.finds({
            filter: {
              logic: 'and',
              filters: kendoFilter([{ field: 'InvoiceSK', value: _key, logic: 'and', operator: 'eq' }])
            }
          }).pipe(tap((data: any[]) => {
            data.forEach(rec => {
              result.push({
                TransactionDescription: 'RECEIVED',
                Date: rec['ReceiptDate'],
                TransactionRef: rec['R1'],
                Amount: rec['TotalAmt'],
                RecordStatus: rec['RecordStatus']
              })
            })
          }))
        }),
        mergeMap(() => {
          return this._commissionAllocationService.finds({
            filter: {
              logic: 'and',
              filters: kendoFilter([
                { field: 'HeaderSK', value: _key, logic: 'and', operator: 'eq' },
              ])
            }
          }).pipe(tap((data: any[]) => {
            data.forEach((alo: any) => {

              this._subsink.sink = this._commissionPayableService.finds({
                filter: {
                  logic: 'and',
                  filters: kendoFilter([
                    { field: 'ALOSK', value: alo['SysKey'], logic: 'and', operator: 'eq' },
                  ])
                }
              })
                .pipe(map((value: any[]) => {
                  value.map(i => {
                    i['SPSK'] = alo['SPSK'];
                  })
                  return value;
                }),
                  tap((data: any[]) => {
                    data.forEach(p => {
                      this._subsink.sink = this._salespersonService.everyBody({
                        filter: {
                          logic: 'and',
                          filters: kendoFilter([
                            { field: 'SysKey', value: p['SPSK'], logic: 'and', operator: 'eq' }
                          ])
                        }
                      })
                        .pipe(tap((sp: any[]) => {
                          result.push({
                            TransactionDescription: `COMMISSION PAID TO (${sp[0]['Name']})`,
                            Date: p['DatePaid'],
                            TransactionRef: p['R1'],
                            Amount: p['NetPaid'],
                            RecordStatus: p['RecordStatus']
                          })
                        })).subscribe()
                    })
                  })).subscribe()
            })
          }))
        })
      ).subscribe(() => this.gridResult.next(result));
  }
}
