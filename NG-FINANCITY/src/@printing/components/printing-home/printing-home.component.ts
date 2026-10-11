import { CommonModule } from '@angular/common';
import { Component, NgModule, OnDestroy, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { PanelBarItemModel, PanelBarExpandMode, LayoutModule } from '@progress/kendo-angular-layout';
import { FilterPanelModule } from 'src/@common/components/p-filter-panel/p-filter-panel-module';
import { PInputIconDirectiveModule } from 'src/@common/components/p-input-icon/p-input-icon.module';
import { PToolbarModule } from 'src/@common/components/p-toolbar/p-toolbar.module';
import { AuthGuard } from 'src/@common/Infrastructure/auth.guard';
import { IndividualSalesComponent } from '../individual-sales.component';

@Component({
  selector: 'app-printing-home',
  template: `
  <div class="layout">
    <div class="navigation-pane">
        <input placeholder="Quick Search" type="text" pInputIcon icon='fa fa-search' (input)='handleSearch($event)' kendoTextBox class="form-control">
        <kendo-panelbar (stateChange)="handleStateChange($event)" [items]="panelBarItems" [expandMode]="expandMode">
        </kendo-panelbar>
    </div>
    <div class="content-pane">
        <div class="card" style="min-height: 100%;">
            <div class="card-body" style="width: 100%;height: 100%;">
                <router-outlet name="report"></router-outlet>
            </div>
        </div>
    </div>
  </div>
  `,
  providers: [],
  styles: [`

    .layout {
      display:table-cell;
      display:flex;
      width:100%;
      height:99%;
    }

    .navigation-pane {
      min-width: 20%;
      min-height: 100%;
    }

    .content-pane {
      padding-left: 10pt;
      padding-right: 10pt;
      min-width: 80%;
      min-height: 100%;
    }

    .custom-template{
      padding:30px; 
      text-align:center;
    }
  `],
})
export class PrintingHomeComponent implements OnInit, OnDestroy {

  public expandMode: number = PanelBarExpandMode.Single;

  panelBarItems: any[] = [
    { id: 'default-0', expanded: true, title: 'Sales Report', children: [] },
    { id: 'default-1', expanded: false, title: 'Franchisee', children: [] },
    { id: 'default-2', expanded: false, title: 'Sales Bonus Report', children: [] },
    { id: 'default-3', expanded: false, title: 'Accounts Receivable Report', children: [] },
    { id: 'default-4', expanded: false, title: 'Accounts Payable Report', children: [] },
    { id: 'default-5', expanded: false, title: 'Aged Receivables Report', children: [] },
    { id: 'default-6', expanded: false, title: 'General Ledger Report', children: [] },
    { id: 'default-7', expanded: false, title: 'Cashbook Report', children: [] },
    { id: 'default-8', expanded: false, title: 'Job Report', children: [] },
    { id: 'default-9', expanded: false, title: 'Accounting Report', children: [] },
    { id: 'default-10', expanded: false, title: 'GST Report', children: [] },
  ];

  private _panelbarClone: any[] = [];

  constructor(public router: Router) {
  }

  ngOnInit(): void {
    this.mapItems(routes.find(x => x.path === 'Reports').children);
    this._panelbarClone = this.panelBarItems;
  }

  private mapItems(routes: any[], path?: string) {
    routes.filter(item => item.path != '' && item.path != '**').map(item => {
      const result: any = {
        title: item.text,
        path: item.path,
        id: item.path,
      };
      (this.panelBarItems.find(x => x.title == item.group).children as any[]).push(result);
    });
  }

  ngOnDestroy(): void {
  }

  handleSearch(event: any) {

    let search: string = event.target.value;
    let results = [];

    if (search == '') {
      this.panelBarItems = this._panelbarClone;
    } else {
      this._panelbarClone.forEach((parent: any) => {
        (parent.children as any[]).forEach(item => {
          if ((item['title'] as string).toLowerCase().indexOf(search.toLowerCase()) >= 0) {
            results.push(item);
          }
        })
      })
      this.panelBarItems = results;
    }
  }

  handleStateChange(data: Array<PanelBarItemModel>) {

    const focusedEvent: PanelBarItemModel = data.filter(item => item.focused === true)[0];

    if (!this.isDefaultItem(focusedEvent.id)) {
      this.router.navigate(["/Reports", { outlets: { report: [focusedEvent.id] } }]);
    }
  }

  isDefaultItem(item: any) {
    return item.indexOf('default') >= 0;
  }
}

export const routes: any[] = [
  {
    text: 'Reports',
    path: 'Reports',
    component: PrintingHomeComponent,
    canActivate: [AuthGuard],
    children: [
      {
        text: '',
        path: '',
        component: IndividualSalesComponent,
        outlet: 'report',
      },
      //******* Franschisee */
      {
        text: 'Franchisee Sales',
        path: 'FranchiseeSales',
        loadChildren: () => import('../franchisee-sales-report.component').then(x => x.Module),
        group: 'Franchisee',
        outlet: 'report'
      },
      //******** Sales Report */
      {
        text: 'Individual Sales',
        path: 'IndividualSales',
        loadChildren: () => import('../individual-sales.component').then(x => x.Module),
        group: 'Sales Report',
        outlet: 'report'
      },
      {
        text: 'Sales Summary',
        path: 'SalesSummary',
        loadChildren: () => import('../sales-summary.component').then(x => x.Module),
        group: 'Sales Report',
        outlet: 'report'
      },
      {
        text: 'Sales Details',
        path: 'SalesDetail',
        loadChildren: () => import('../sales-detail.component').then(x => x.Module),
        group: 'Sales Report',
        outlet: 'report'
      },
      {
        text: 'Sales Transaction List',
        path: 'PropertyTransactionList',
        loadChildren: () => import('../property-transaction.component').then(x => x.Module),
        group: 'Sales Report',
        outlet: 'report'
      },
      {
        text: 'Top Sales',
        path: 'TopSales',
        loadChildren: () => import('../top-sales-report.component').then(x => x.Module),
        group: 'Sales Report',
        outlet: 'report'
      },
      {
        text: 'Sales Bonus Report',
        path: 'SalesBonusReport',
        loadChildren: () => import('../sales-bonus-report.component').then(x => x.Module),
        group: 'Sales Bonus Report',
        outlet: 'report'
      },
      // // ******** Accounts Receivable Report */
      {
        text: 'Commission Receivable',
        path: 'CommissionReceivableReport',
        loadChildren: () => import('../commission-receivable-report.component').then(x => x.Module),
        group: 'Accounts Receivable Report',
        outlet: 'report'
      },
      {
        text: 'Commission Received',
        path: 'CommissionReceivedReport',
        loadChildren: () => import('../commission-received-report.component').then(x => x.Module),
        group: 'Accounts Receivable Report',
        outlet: 'report'
      },
      // ******** Accounts Payable Report*/
      {
        text: 'Commission Payable',
        path: 'CommissionPayableReport',
        loadChildren: () => import('../commission-payble-to-internal-component').then(x => x.Module),
        group: 'Accounts Payable Report',
        outlet: 'report'
      },
      {
        text: 'Commission Payable (By Individual SP)',
        path: 'SalespersonCommissionPayableReportV1',
        loadChildren: () => import('../commission-payable-v1-report.component').then(x => x.Module),
        group: 'Accounts Payable Report',
        outlet: 'report'
      },
      {
        text: 'Account Payable (BS Audit)',
        path: 'SalespersonCommissionPayableReportBS',
        loadChildren: () => import('../commission-payable-bs-report-component').then(x => x.Module),
        group: 'Accounts Payable Report',
        outlet: 'report'
      },
      {
        text: 'Salesperson Commission Paid',
        path: 'SalespersonCommissionPaidReport',
        loadChildren: () => import('../commission-paid-report-component').then(x => x.Module),
        group: 'Accounts Payable Report',
        outlet: 'report'
      },
      // //******** Aged Receivables Report*/
      {
        text: 'Aged Receivables Summary',
        path: 'AgedReceivablesSummaryReport',
        loadChildren: () => import('../aged-receivables-summary-report.component').then(x => x.Module),
        group: 'Aged Receivables Report',
        outlet: 'report'
      }, {
        text: 'Aged Receivables Detail',
        path: 'AgedReceivablesDetailReport',
        loadChildren: () => import('../age-receivables-detail-report.component').then(x => x.Module),
        group: 'Aged Receivables Report',
        outlet: 'report'
      },
      // //******** General Ledger Report*/
      {
        text: 'General Ledger Summary',
        path: 'GeneralLedgerSummary',
        loadChildren: () => import('../general-ledger-summary-report.component').then(x => x.Module),
        group: 'General Ledger Report',
        outlet: 'report'
      },
      {
        text: 'General Ledger Detail',
        path: 'GeneralLedgerDetail',
        loadChildren: () => import('../general-ledger-detail-report.component').then(x => x.Module),
        group: 'General Ledger Report',
        outlet: 'report'
      },
      {
        text: 'General Ledger Ref. Summary',
        path: 'GeneralLedgerRefSummary',
        loadChildren: () => import('../general-ledger-ref-summary-report.component').then(x => x.Module),
        group: 'General Ledger Report',
        outlet: 'report'
      },
      {
        text: 'General Ledger Ref. Detail',
        path: 'GeneralLedgerRefDetail',
        loadChildren: () => import('../general-ledger-ref-detail-report.component').then(x => x.Module),
        group: 'General Ledger Report',
        outlet: 'report'
      },
      // //******** Cashbook Report */
      {
        text: 'Cashbook Log',
        path: 'CashbookLog',
        loadChildren: () => import('../cashbook-log-report.component').then(x => x.Module),
        group: 'Cashbook Report',
        outlet: 'report'
      },
      {
        text: 'Receipt Log',
        path: 'CashbookReceiptLog',
        loadChildren: () => import('../cashbook-receipt-report.component').then(x => x.Module),
        group: 'Cashbook Report',
        outlet: 'report'
      },
      {
        text: 'Payment Log',
        path: 'CashbookPaymentLog',
        loadChildren: () => import('../cashbook-payment-log-report.component').then(x => x.Module),
        group: 'Cashbook Report',
        outlet: 'report'
      },
      // //******** Job Report */
      {
        text: 'Job Activity Summary',
        path: 'JobActivitySummary',
        loadChildren: () => import('../job-actvity-summary-report-component').then(x => x.Module),
        group: 'Job Report',
        outlet: 'report'
      },
      {
        text: 'Job Activity Detail',
        path: 'JobActivityDetail',
        loadChildren: () => import('../job-activity-detail-report.component').then(x => x.Module),
        group: 'Job Report',
        outlet: 'report'
      },
      {
        text: 'Job Activity List',
        path: 'JobActivityList',
        loadChildren: () => import('../job-list-report.component').then(x => x.Module),
        group: 'Job Report',
        outlet: 'report'
      },
      // //******** Accounting Report */
      {
        text: 'Trial Balance',
        path: 'TrialBalance',
        loadChildren: () => import('../trial-balance.component').then(x => x.Module),
        group: 'Accounting Report',
        outlet: 'report'
      },
      {
        text: 'Balance Sheet',
        path: 'BalanceSheet',
        loadChildren: () => import('../balance-sheet.component').then(x => x.Module),
        group: 'Accounting Report',
        outlet: 'report'
      },
      {
        text: 'Income Statement',
        path: 'IncomeStatement',
        loadChildren: () => import('../income-statement.component').then(x => x.Module),
        group: 'Accounting Report',
        outlet: 'report'
      },
      {
        text: 'Comparable Income Statement [Year]',
        path: 'ComparableYearlyIncomeStatement',
        loadChildren: () => import('../compare-incomestatement-yearly-report.component').then(x => x.Module),
        group: 'Accounting Report',
        outlet: 'report'
      },
      {
        text: 'Comparable Income Statement [Quarter]',
        path: 'ComparableQuarterlyIncomeStatement',
        loadChildren: () => import('../compare-incomestatement-quarterly-report.component').then(x => x.Module),
        group: 'Accounting Report',
        outlet: 'report'
      },
      {
        text: 'Comparable Income Statement [Monthly]',
        path: 'ComparableMonthlyIncomeStatement',
        loadChildren: () => import('../compare-incomestatement-monthly-report.component').then(x => x.Module),
        group: 'Accounting Report',
        outlet: 'report'
      },
      //******** GST Report */
      {
        text: 'GST (IRAS)',
        path: 'IRASReport',
        loadChildren: () => import('../iras-report.component').then(x => x.Module),
        group: 'GST Report',
        outlet: 'report'
      },
    ]
  }
]

@NgModule({
  declarations: [PrintingHomeComponent],
  imports: [
    CommonModule,
    LayoutModule,
    PToolbarModule,
    FilterPanelModule,
    InputsModule,
    PInputIconDirectiveModule,
    RouterModule.forChild(routes)
  ],
  exports: [PrintingHomeComponent],
})
export class Module { }