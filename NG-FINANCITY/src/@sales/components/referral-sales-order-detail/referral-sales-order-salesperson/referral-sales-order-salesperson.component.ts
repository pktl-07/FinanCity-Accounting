import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { BehaviorSubject } from 'rxjs';
import { FormGroup } from '@angular/forms';
import { WindowState } from '@progress/kendo-angular-dialog';
import { kendoFilter, SubSink } from 'src/@common/service/utility-service';
import { SalespersonManagementService } from 'src/@sales/services/salesperson-management.service';
import { SalespersonManagementDetailComponent } from '../../salesperson-management-detail/salesperson-management-detail.component';
import { ReferralSalesOrderTypeService } from 'src/@sales/services/referral-sales-order-type.service';
import { SalesOrderTypeDetailComponent } from '../../sales-order-type-detail/sales-order-type-detail.component';
import { CommissionStructureDetailService } from 'src/@sales/services/commission-structure-detail.service';

@Component({
  selector: 'app-referral-sales-order-salesperson',
  templateUrl: './referral-sales-order-salesperson.component.html',
  providers: [
    PWindowComponentService,
    SalespersonManagementService,
    ReferralSalesOrderTypeService,
    CommissionStructureDetailService
  ]
})
export class ReferralSalesOrderSalespersonComponent implements OnDestroy, OnInit {

  @Input() formGroup: FormGroup;
  public _subsink = new SubSink();
  public _commissionTypeSubject = new BehaviorSubject([]);

  constructor(public _windowService: PWindowComponentService,
    public _commissionSchemeService: CommissionStructureDetailService,
    public _referralSalesOrderType: ReferralSalesOrderTypeService,
    public _salespersonManagementService: SalespersonManagementService) {
  }

  ngOnInit(): void {

    this._referralSalesOrderType.asyncFinds();
    this._subsink.sink = this._salespersonManagementService.subscribe();
    this._salespersonManagementService.asyncFinds({
      filter: {
        logic: 'and',
        filters: kendoFilter([
          { field: 'SysKey', value: this.formGroup.get('Internal').get('SPSK').value, logic: 'and', operator: 'eq' }
        ])
      }
    });
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  public itemDisabled(itemArgs: { dataItem: any, index: number }) {

    if (typeof itemArgs.dataItem.IsActive != 'undefined')
      return !itemArgs.dataItem.IsActive;

    return false;
  }

  private _filterText: string;
  handelSalespersonFilterChange(salesperson: string) {

    this._filterText = salesperson;
    if (salesperson.length >= 3) {
      this._salespersonManagementService.asyncFinds({
        filter: {
          logic: 'and',
          filters: kendoFilter([
            { field: 'Name', value: salesperson, operator: 'contains', logic: 'and' }
          ]),
        }
      })
    }
  }

  addNewSalesperson() {

    let state: WindowState = 'maximized';
    let window = this._windowService.open({ title: 'Salesperson Management', componentType: SalespersonManagementDetailComponent, state: state });
    let windowComponent = window.content.instance as SalespersonManagementDetailComponent;
    windowComponent.loadData({ Name: this._filterText });

    this._subsink.sink = windowComponent.OnSuccess
      .subscribe(item => {

        this._salespersonManagementService.asyncFinds({
          filter: {
            filters: kendoFilter([{ field: 'SysKey', value: item.data.Person.SysKey, operator: 'eq', logic: 'and' }]),
            logic: 'and'
          }
        })

        this.formGroup.get('Internal').get('SPSK').patchValue(item.data.Person.SysKey);
        this._windowService.close();
      })
  }

  handelSalesOrderFilterChange(salesOrderTypeSK: string) {

    this._filterText = salesOrderTypeSK;
    this._referralSalesOrderType.asyncFinds({
      filter: {
        logic: 'and',
        filters: kendoFilter([
          { field: 'Description', value: salesOrderTypeSK, operator: 'contains', logic: 'and' }
        ]),
      }
    })
  }

  addNewSalesOrderType() {

    let state: WindowState = 'maximized';
    let window = this._windowService.open({ title: 'Referral Sales Order Type', componentType: SalesOrderTypeDetailComponent, state: state });
    let windowComponent = window.content.instance as SalesOrderTypeDetailComponent;
    windowComponent.loadData({ Description: this._filterText });

    this._subsink.sink = windowComponent.OnSuccess
      .subscribe(item => {

        this._referralSalesOrderType.asyncFinds();

        this.formGroup.get('PropertyTransaction').get('SaleOrderTypeSK').setValue(item.data.SysKey);
        this._windowService.close();
      });
  }
}
