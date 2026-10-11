import { Resolve } from '@angular/router';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { kendoFilter, getComissionContext } from 'src/@common/service/utility-service';
import { map, mergeMap, takeUntil } from 'rxjs/operators';
import { SalespersonManagementService } from '../salesperson-management.service';
import { CommissionReceivedAllocationService } from '../commission-received-allocation.service';
import { VendorService } from '../vendor.service';
import { SalesInvoiceService } from '../sales-invoice.service';
import { PayableSchemeService } from '../payable-scheme.service';
import { CommissionPayableService } from '../commission-payable.service';
import { PropertyTransactionDetailService } from '../property-transaction-detial.service';
import { CommissionStructureDetailService } from '../commission-structure-detail.service';
import { PayNowService } from '../pay-now.service';

@Injectable({ providedIn: 'root' })
export class CommissionPayableResolverService implements Resolve<any> {

    private _unsubscribe = new Subject();

    constructor(
        private _salesInvoice: SalesInvoiceService,
        private _commissionPayableService: CommissionPayableService,
        private _vendorService: VendorService,
        private _commissionStructureService: CommissionStructureDetailService,
        private _payableSchemeService: PayableSchemeService,
        private _propertyTransactionDetailSerivce: PropertyTransactionDetailService,
        private _salespersonManagementService: SalespersonManagementService,
        private _allocationService: CommissionReceivedAllocationService,
        private _paynowService: PayNowService) {
    }
    //DetailSK
    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<any> | Promise<any> | any {

        return this._salespersonManagementService.everyBody({
            filter: {
                filters: kendoFilter([{ field: 'SysKey', value: route.params.spsk, operator: 'eq', logic: 'and' }]),
                logic: 'and'
            }
        }).pipe(
            takeUntil(this._unsubscribe),
            mergeMap((data: any) => {
                return this._vendorService.findByKey(data[0].VendorSK)
                    .pipe(
                        takeUntil(this._unsubscribe),
                        map(vendor => {
                            return {
                                InvoiceSK: route.params.invoicesk,
                                SPSK: route.params.spsk,
                                ALOSK: route.params.alosk,
                                SalespersonName: data[0]['Name'],
                                BankAccountName: data[0]['BankName'],
                                BankAccountNo: data[0]['BankAccountNo'],
                                NRIC: data[0]['NRIC'],
                                CompanyName: vendor['Vendor']['CompanyName'],
                                SalespersonType: data[0]['TransType'],
                            }
                        }))
            }),
            mergeMap((data: any) => {
                return this._paynowService.finds({
                    filter: {
                        filters: kendoFilter([{ field: 'ParentSK', value: data.SPSK, operator: 'eq', logic: 'and' }]),
                        logic: 'and'
                    }
                }).pipe(
                    takeUntil(this._unsubscribe),
                    map(paynow => {
                        data['PayNow'] = paynow[0].MobileNumber
                        return data;
                    }))
            }),
            mergeMap((data: any) => {
                return this._propertyTransactionDetailSerivce.finds({
                    filter: {
                        filters: kendoFilter([
                            { field: 'PTOSK', value: route.params.invoicesk, operator: 'eq', logic: 'and' },
                            { field: 'SPSK', value: route.params.spsk, operator: 'eq', logic: 'and' },
                        ]),
                        logic: 'and'
                    }
                }).pipe(
                    takeUntil(this._unsubscribe),
                    map(prop => {
                        data['DetailSK'] = prop[0]['SysKey'];
                        data['TotalGrossComm'] = prop[0]['GrossComm'];
                        data['TotalExpense'] = prop[0]['MarketingExpense'];
                        data['SchemeDetailSK'] = prop[0]['SchemeDetailSK'];
                        //data['CommissionType'] = prop[0]['CommissionType'];
                        return data;
                    }))
            }),
            mergeMap((data: any) => {
                return this._commissionStructureService.findByKey(data['SchemeDetailSK'])
                    .pipe(takeUntil(this._unsubscribe),
                        map(scheme => {
                            data['CommissionType'] = scheme['Type'];
                            return data;
                        })
                    )
            }),
            mergeMap((data: any) => {
                return this._allocationService.finds({
                    filter: {
                        filters: kendoFilter([
                            { field: 'SysKey', value: route.params.alosk, operator: 'eq', logic: 'and' }
                        ]),
                        logic: 'and'
                    }
                }).pipe(
                    takeUntil(this._unsubscribe),
                    map(alo => {

                        let commReceived = alo.reduce((sum, current) => sum + current['AmountBeforeGST'], 0);
                        let commDueToSP = alo.reduce((sum, current) => sum + current['AmountBeforeGST'], 0);
                        let gstValue = alo.reduce((sum, current) => sum + current['GSTValue'], 0);
                        let expense = alo.reduce((sum, current) => sum + current['MarketingExpense'], 0);
                        let gstPercent = alo[0]['GSTPercent'];

                        data['MarketingPromotion'] = expense;
                        data['CommissionReceived'] = commReceived;
                        data['CommissionDueToSP'] = commDueToSP;
                        data['SalespersonGrossCommission'] = commDueToSP - expense;
                        data['GSTValue'] = gstValue;
                        data['GSTPercent'] = gstPercent;

                        return data;
                    }))
            }),
            mergeMap((data: any) => {

                return this._salesInvoice.finds({
                    filter: {
                        filters: kendoFilter([
                            { field: 'SysKey', value: route.params.invoicesk, operator: 'eq', logic: 'and' }
                        ]),
                        logic: 'and'
                    }
                }).pipe(
                    takeUntil(this._unsubscribe),
                    map(inv => {

                        data['InvoiceNo'] = inv[0]['R1'];
                        return data;
                    }))
            }),
            mergeMap((data: any) => {
                return this._payableSchemeService.finds({
                    filter: {
                        filters: kendoFilter([
                            { field: 'ALOSK', value: route.params.alosk, operator: 'eq', logic: 'and' },
                            { field: 'RecordStatus', value: 8, operator: 'eq', logic: 'and' }
                        ]),
                        logic: 'and'
                    }
                }).pipe(
                    takeUntil(this._unsubscribe),
                    map((payable: []) => {

                        let associateComm = payable.reduce((sum, current) => sum + current['AssociateComm'], 0);
                        data['AddGST'] = 0;

                        if (data['SalespersonType'] == 20) {

                            let taxDetail = getComissionContext({
                                tax: data['GSTPercent'],
                                amount: associateComm,
                                call: 'total',
                            });

                            data['GrossCommissionPayableToSalesperson'] = taxDetail.gross;
                            data['AddGST'] = taxDetail.taxAmount;
                        }

                        data['CommissionPayableToSalesperson'] = associateComm;

                        return data;
                    }))
            }),
            mergeMap((data: any) => {
                return this._commissionPayableService.finds({
                    filter: {
                        filters: kendoFilter([
                            { field: 'ALOSK', value: route.params.alosk, operator: 'eq', logic: 'and' },
                            { field: 'RecordStatus', value: 8, operator: 'eq', logic: 'and' }
                        ]),
                        logic: 'and'
                    }
                }).pipe(
                    takeUntil(this._unsubscribe),
                    map((paid: []) => {
                        let commPaid = paid.reduce((sum, current) => sum + current['NetPaid'], 0);

                        data['CommissionPaid'] = commPaid;
                        data['Balance'] = data['CommissionPayableToSalesperson'] - commPaid;

                        return data;
                    }))
            }),
        );
    }
}