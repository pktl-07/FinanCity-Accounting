import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AuthGuard } from 'src/@common/Infrastructure/auth.guard';
import { PropertyTransactionResolverService } from 'src/@sales/services/resolver/property-transaction-resolver.service';
import { SalesInvoiceResolverService } from 'src/@sales/services/resolver/sales-invoice.resolver';
import { CommissionReceivedResolverService } from 'src/@sales/services/resolver/commission-received-resolver.service';
import { CommissionReceiveAllocationResolveService } from 'src/@sales/services/resolver/commission-allocation-resolver.service';
import { MyCompanyResolverService } from 'src/@sales/services/resolver/my-company-resolver.service';
import { CommissionPayableResolverService } from 'src/@sales/services/resolver/commission-payable-resolver.service';
import { JournalLedgerResolverService } from 'src/@accounts/services/resolver/journal-ledger-resolver.service';
import { JournalLedgerDetailResolverService } from 'src/@accounts/services/resolver/journal-ledger-detail-resolver.service';
import { CashbookTransactionResolverService } from 'src/@accounts/services/resolver/cashbook-transaction-resolver.service';
import { ChangeOfStatusResolveService } from 'src/@sales/services/resolver/change-of-status-resolver.service';
import { BranchOutDetailResolveService } from 'src/@sales/services/resolver/branch-out-detail-resolver.service';
import { ReferralSalesOrderResolverService } from 'src/@sales/services/resolver/referral-sales-order-resolver.service';
import { AuditTrailResolverService } from 'src/@accounts/services/resolver/audti-trail-resolver.service';

const routes: any = [
  {
    path: '',
    text: 'Home',
    icon: 'fas fa-home',
    canActivate: [AuthGuard],
    loadChildren: () => import('./components/home/home.module').then(m => m.HomeModule),
  }, {
    text: 'Login',
    path: 'Login',
    loadChildren: () => import('../@user-accounts/components/user-login/user-login.module').then(m => m.UserLoginModule),
    display: 'none'
  }, {
    text: 'Business',
    path: 'Business',
    icon: 'far fa-exchange',
    children: [
      {
        text: 'Sales Order',
        path: 'SalesTransaction',
        children: [
          { text: 'Mortgage Sales Transaction', path: 'SalesOrderTransaction', loadChildren: () => import('../@sales/components/property-transaction-list/property-transaction-list.module').then(m => m.PropertyTransactionListModule), color: 'green', icon: 'far fa-exchange', canActivate: [AuthGuard] },
          { text: 'Referral Sales Transaction', path: 'ReferralSalesTransaction', loadChildren: () => import('../@sales/components/referral-sales-order-list/referral-sales-order-list.module').then(m => m.ReferralSalesOrderListModule), color: 'solid-blue', icon: 'far fa-exchange', canActivate: [AuthGuard] },
        ]
      }, {
        text: 'Property Transaction Detail', path: 'PropertyTransactionDetail', resolve: { data: PropertyTransactionResolverService }, loadChildren: () => import('../@sales/components/property-transaction-detail/property-transaction-detail.module').then(m => m.PropertyTransactionDetailModule), display: 'none', color: 'solid-green', icon: 'far fa-exchange', canActivate: [AuthGuard]
      }, {
        text: 'Property Transaction Detail', path: 'PropertyTransactionDetail/:id', resolve: { data: PropertyTransactionResolverService }, loadChildren: () => import('../@sales/components/property-transaction-detail/property-transaction-detail.module').then(m => m.PropertyTransactionDetailModule), display: 'none', color: 'solid-green', icon: 'far fa-exchange', canActivate: [AuthGuard]
      }, {
        text: 'Mortgage Sales Detail', path: 'ReferralSalesOrderDetail', resolve: { data: ReferralSalesOrderResolverService }, loadChildren: () => import('../@sales/components/referral-sales-order-detail/referral-sales-order-detail.module').then(m => m.ReferralSalesOrderDetailModule), display: 'none', color: 'solid-green', icon: 'far fa-exchange', canActivate: [AuthGuard]
      }, {
        text: 'Mortgage Sales Detail', path: 'ReferralSalesOrderDetail/:id', resolve: { data: ReferralSalesOrderResolverService }, loadChildren: () => import('../@sales/components/referral-sales-order-detail/referral-sales-order-detail.module').then(m => m.ReferralSalesOrderDetailModule), display: 'none', color: 'solid-green', icon: 'far fa-exchange', canActivate: [AuthGuard]
      }, {
        text: 'Sales Invoice', path: 'SalesInvoice', loadChildren: () => import('../@sales/components/sales-invoice-list/sales-invoice-list.module').then(m => m.SalesInvoiceListModule), color: 'red', icon: 'far fa-file-invoice', canActivate: [AuthGuard]
      }, {
        text: 'Sales Invoice', path: 'SalesInvoiceDetail/:id', resolve: { data: SalesInvoiceResolverService }, loadChildren: () => import('../@sales/components/sales-invoice-detail/sales-invoice-detail.module').then(m => m.SalesInvoiceDetailModule), display: 'none', color: 'solid-red', icon: 'far fa-file-invoice', canActivate: [AuthGuard]
      }, {
        text: 'Commission Receivable', path: 'CommissionReceived', loadChildren: () => import('../@sales/components/commission-received-list/commission-received-list.module').then(m => m.CommissionReceivedListModule), color: 'brown', icon: 'far fa-receipt', canActivate: [AuthGuard]
      }, {
        text: 'Commission Receivable', path: 'CommissionReceivedDetail/:id', resolve: { data: CommissionReceivedResolverService }, loadChildren: () => import('../@sales/components/commission-received-detail/commission-received-detail.module').then(m => m.CommissionReceivedDetailModule), display: 'none', color: 'solid-brown', icon: 'far fa-receipt', canActivate: [AuthGuard]
      }, {
        text: 'Commission Allocation', path: 'CommissionReceivedAllocation', loadChildren: () => import('../@sales/components/commission-received-allocation-list/commission-received-allocation-list.module').then(m => m.CommissionReceivedAllocationListModule), color: 'pupple', icon: 'far fa-users', canActivate: [AuthGuard]
      }, {
        text: 'Commission Allocation', path: 'CommissionReceivedAllocationDetail/:id', resolve: { data: CommissionReceiveAllocationResolveService }, loadChildren: () => import('../@sales/components/commission-received-allocation-detail/commission-received-allocation-detail.module').then(m => m.CommissoinReceivedAllocationDetailModule), display: 'none', color: 'solid-pupple', icon: 'far fa-users', canActivate: [AuthGuard]
      }, {
        text: 'Commission Payable', path: 'CommissionPayable', loadChildren: () => import('../@sales/components/commission-payable-list/commission-payable-list.module').then(m => m.CommissionReceivedAllocationListModule), color: 'blue', icon: 'fab fa-paypal', canActivate: [AuthGuard]
      }, {
        text: 'Commission Payable', path: 'CommissionPayableDetail/:alosk/:invoicesk/:spsk', resolve: { data: CommissionPayableResolverService }, loadChildren: () => import('../@sales/components/commission-payable-detail/commission-payable-detail.module').then(m => m.CommissionPayableDetailModule), display: 'none', color: 'solid-blue', icon: 'fab fa-paypal">', canActivate: [AuthGuard]
      }]
  }, {
    text: 'Accounting',
    path: 'Accounting',
    icon: 'fas fa-balance-scale',
    children: [{
      text: 'Journal Ledger', path: 'JournalLedger', resolve: { data: JournalLedgerResolverService }, loadChildren: () => import('../@accounts/components/journal-ledger-list/journal-ledger-list.module').then(m => m.JournalLedgerListModule), color: 'solid-green', icon: 'far fa-paper-plane', canActivate: [AuthGuard]
    }, {
      text: 'Journal Ledger', path: 'JournalLedgerDetail', resolve: { data: JournalLedgerDetailResolverService }, loadChildren: () => import('../@accounts/components/journal-ledger-detail/journal-ledger-detail.module').then(m => m.JournalLedgerDetailModule), display: 'none', color: 'solid-green', icon: 'far fa-paper-plane', canActivate: [AuthGuard]
    }, {
      text: 'Journal Ledger', path: 'JournalLedgerDetail/:parentId/:moduleCode/:postDate', resolve: { data: JournalLedgerDetailResolverService }, loadChildren: () => import('../@accounts/components/journal-ledger-detail/journal-ledger-detail.module').then(m => m.JournalLedgerDetailModule), display: 'none', color: 'pupple', icon: 'far fa-calendar-plus', canActivate: [AuthGuard]
    }, {
      text: 'Chart of Account', path: 'ChartOfAccounts', loadChildren: () => import('../@accounts/components/chart-of-accounts/chart-of-accounts.module').then(m => m.ChartOfAccountModule), color: 'brown', icon: 'far fa-globe', canActivate: [AuthGuard]
    }, {
      text: 'Accounting Period', path: 'AccountingPeriod', loadChildren: () => import('../@accounts/components/accounting-peroid/accounting-peroid.module').then(m => m.AccountingPeriodModule), color: 'solid-blue', icon: 'far fa-calendar-plus', canActivate: [AuthGuard]
    }, {
      text: 'Close Accounting Year', path: 'CloseAccountingYear', loadChildren: () => import('../@accounts/components/year-closing/year-closing.module').then(m => m.YearClosingModule), color: 'brown', icon: 'far fa-calendar-times', canActivate: [AuthGuard]
    }, {
      text: 'Audit Trail', path: 'AuditTrail', resolve: { data: AuditTrailResolverService }, loadChildren: () => import('../@accounts/components/audit-trail-list/audit-trail-list.module').then(m => m.AuditTrailListModule), color: 'brown', icon: 'far fa-check', canActivate: [AuthGuard]
    }]
  }, {
    text: 'Cashbook',
    path: 'Cashbook',
    icon: 'fas fa-sack-dollar',
    children: [{
      text: 'Cashbook Transaction', path: 'CashbookTransaction', loadChildren: () => import('../@accounts/components/cashbook-transaction-list/cashbook-transaction-list.module').then(m => m.CashbookTransactionListModule), color: 'pupple', icon: 'fas fa-sack-dollar', canActivate: [AuthGuard]
    }, {
      text: 'Cashbook Transaction', path: 'CashbookTransactionDetail', resolve: { data: CashbookTransactionResolverService }, loadChildren: () => import('../@accounts/components/cashbook-transaction-detail/cashbook-transaction-detail.module').then(m => m.CashbookTransactionDetailModule), display: 'none', color: 'pupple', icon: 'fas fa-sack-dollar', canActivate: [AuthGuard]
    }, {
      text: 'Cashbook Transaction', path: 'CashbookTransactionDetail/:parentId/:type', resolve: { data: CashbookTransactionResolverService }, loadChildren: () => import('../@accounts/components/cashbook-transaction-detail/cashbook-transaction-detail.module').then(m => m.CashbookTransactionDetailModule), display: 'none', color: 'pupple', icon: 'fas fa-sack-dollar', canActivate: [AuthGuard]
    }, {
      text: 'Cashbook Transfer', path: 'CashbookTransfer', loadChildren: () => import('../@accounts/components/cashbook-transfer-list/cashbook-transfer-list.module').then(m => m.CashbookTransferListModule), color: 'blue', icon: 'fas fa-hand-holding-usd', canActivate: [AuthGuard]
    }]
  }, {
    text: 'Management',
    path: 'Management',
    icon: 'fas fa-puzzle-piece',
    children: [
      {
        text: 'Salesperson Management',
        path: 'SalespersonManagement',
        color: 'solid-pupple',
        icon: 'fal fa-user-chart',
        children: [{
          text: 'Salesperson Management', path: 'Recruitment', loadChildren: () => import('../@sales/components/salesperson-management-list/salesperson-management-list.module').then(m => m.SalespersonManagementListModule), color: 'solid-pupple', icon: 'fal fa-user-chart', canActivate: [AuthGuard]
        }, {
          text: 'Change of Status', path: 'ChangeOfStatus', loadChildren: () => import('../@sales/components/change-of-status-list/change-of-status-list.module').then(m => m.ChangeOfStatusListModule), color: 'blue', icon: 'fal fa-star', canActivate: [AuthGuard]
        }, {
          text: 'Change of Status', path: 'ChangeOfStatusDetail', resolve: { data: ChangeOfStatusResolveService }, loadChildren: () => import('../@sales/components/change-of-status-detail/change-of-status-detail.module').then(m => m.ChangeOfStatusDetailModule), display: 'none', color: 'blue', icon: 'fal fa-star', canActivate: [AuthGuard]
        }, {
          text: 'Change of Status', path: 'ChangeOfStatusDetail/:id', resolve: { data: ChangeOfStatusResolveService }, loadChildren: () => import('../@sales/components/change-of-status-detail/change-of-status-detail.module').then(m => m.ChangeOfStatusDetailModule), display: 'none', color: 'blue', icon: 'fal fa-star', canActivate: [AuthGuard]
        }, {
          text: 'Branch Out', path: 'BranchOut', loadChildren: () => import('../@sales/components/branch-out-list/branch-out-list.module').then(m => m.BranchOutListModule), color: 'green', icon: 'fal fa-code-branch', canActivate: [AuthGuard]
        }, {
          text: 'Branch Out', path: 'BranchOutDetail/:id', resolve: { data: BranchOutDetailResolveService }, loadChildren: () => import('../@sales/components/branch-out-detail/branch-out-detail.module').then(m => m.BranchOutDetailModule), display: 'none', color: 'green', icon: 'fas fa-code-branch', canActivate: [AuthGuard]
        }, {
          text: 'Branch Out', path: 'BranchOutDetail', resolve: { data: BranchOutDetailResolveService }, loadChildren: () => import('../@sales/components/branch-out-detail/branch-out-detail.module').then(m => m.BranchOutDetailModule), display: 'none', color: 'green', icon: 'fal fa-user-chart', canActivate: [AuthGuard]
        }]
      }, {
        text: 'Client Management', path: 'ClientManagement', loadChildren: () => import('../@sales/components/client-management-list/client-management-list.module').then(m => m.ClientManagementListModule), color: 'solid-brown', icon: 'fal fa-users-class', canActivate: [AuthGuard]
      }, {
        text: 'Lead Management', path: 'LeadManagement', loadChildren: () => import('../@sales/components/lead-management-list/lead-management-list.module').then(m => m.LeadManagementListModule), color: 'green', icon: 'fal fa-users-class', canActivate: [AuthGuard]
      }, {
        text: 'Vendor Management', path: 'VendorManagement', loadChildren: () => import('../@sales/components/vendor-management-list/vendor-management-list.module').then(m => m.VendorManagementListModule), color: 'solid-red', icon: 'fal fa-hospital-user', canActivate: [AuthGuard]
      }, {
        text: 'External Salesperson', path: 'ExternalSalesperson', loadChildren: () => import('../@sales/components/external-salesperson-list/external-salesperson-list.module').then(m => m.ExternalSalespersonListModule), color: 'solid-blue', icon: 'far fa-user-tie', canActivate: [AuthGuard]
      },
    ]
  }, {
    text: 'Setup',
    path: 'Configuration',
    icon: 'fas fa-cog',
    children: [{
      text: 'Company',
      path: 'Company',
      children: [
        { text: 'My Company', path: 'MyCompany', resolve: { data: MyCompanyResolverService }, loadChildren: () => import('../@sales/components/my-company-setup/my-company-setup.module').then(m => m.MyCompanySetupModule), color: 'green', icon: 'fal fa-building', canActivate: [AuthGuard] },
        { text: 'Franchise', path: 'FranchiseeManagement', loadChildren: () => import('../@sales/components/franchisee-management-list/franchisee-management-list.module').then(m => m.FranchiseeManagementListModule), color: 'brown', icon: 'fal fa-hospital-user', canActivate: [AuthGuard] }
      ]
    }, {
      text: 'Accounts',
      path: 'Accounts',
      children: [
        { text: 'Cashbook', path: 'Cashbook', loadChildren: () => import('../@accounts/components/cashbook-setup-list/cashbook-setup-list.module').then(m => m.CashoobkSetupListModule), color: 'solid-green', icon: 'fab fa-cc-visa', canActivate: [AuthGuard] },
        { text: 'GST', path: 'GST', loadChildren: () => import('../@accounts/components/tax-list/tax-list.module').then(m => m.TaxListModule), color: 'solid-blue', icon: 'fal fa-hands-usd', canActivate: [AuthGuard] },
        { text: 'Currency', path: 'CurrencyRate', loadChildren: () => import('../@sales/components/currency-rate-list/currency-rate-list.module').then(m => m.CurrencyRateListModule), color: 'brown', icon: 'fal fa-money-bill-wave', canActivate: [AuthGuard] },
        { text: 'Job', path: 'Job', loadChildren: () => import('../@accounts/components/job-list/job-list.module').then(m => m.JobListModule), color: 'green', icon: 'fas fa-hammer', canActivate: [AuthGuard] },
      ],
    }, {
      text: 'Sales',
      path: 'Sales',
      children: [
        { text: 'Commission Structure', path: 'CommissionScheme', loadChildren: () => import('../@sales/components/commissoin-scheme-list/commissoin-scheme-list.module').then(m => m.CommissoinSchemeListModule), color: 'blue', icon: 'fal fa-file-user', canActivate: [AuthGuard] },
        { text: 'Promotion Item', path: 'PromotionItem', loadChildren: () => import('../@sales/components/promotion-item-list/promotion-item-list.module').then(m => m.PromotionItemListModule), color: 'red', icon: 'fal fa-user-graduate', canActivate: [AuthGuard] },
        { text: 'Sales Order Type', path: 'SalesOrderType', loadChildren: () => import('../@sales/components/sales-order-type-list/sales-order-type-list.module').then(m => m.SalesOrderTypeListModule), color: 'pupple', icon: 'fal fa-compress-wide', canActivate: [AuthGuard] },
      ]
    }, {
      text: 'Others',
      path: 'Others',
      children: [
        { text: 'Professional Experience', path: 'IndustryExam', loadChildren: () => import('../@sales/components/industryexam-list/industryexam-list.module').then(m => m.IndustryExamListModule), color: 'red', icon: 'fal fa-user-graduate', canActivate: [AuthGuard] },
        { text: 'Academic Qualification', path: 'Qualification', loadChildren: () => import('../@sales/components/qualification-list/qualification-list.module').then(m => m.QualificationListModule), color: 'solid-brown', icon: 'fal fa-user-graduate', canActivate: [AuthGuard] },
        { text: 'Bank Rates', path: 'BankBestRate', loadChildren: () => import('../@sales/components/bank-best-rate-list/bank-best-rate-list.module').then(m => m.BankBestRateListModule), color: 'blue', icon: 'fal fa-heart-rate', canActivate: [AuthGuard] },
        { text: 'ID Reference Index', path: 'RecordIdIndexer', loadChildren: () => import('../@sales/components/record-id-indexer-list/record-id-indexer-list.module').then(m => m.RecordIdIndexerListModule), color: 'pupple', icon: 'fal fa-list-ol', canActivate: [AuthGuard] },
      ]
    }]
  }, {
    path: '',
    loadChildren: () => import('../@printing/components/printing-home/printing-home.component').then(m => m.Module),
    display: 'none'
  }, {
    text: 'Reports',
    path: 'Reports',
    icon: 'fas fa-chart-bar',
    loadChildren: () => import('../@printing/components/printing-home/printing-home.component').then(m => m.Module),
  }, {
    text: 'Users',
    path: 'Users',
    icon: 'fas fa-user',
    children: [
      {
        text: 'User Management', path: 'UserManagement', loadChildren: () => import('../@user-accounts/components/user-management-list/user-management-list.module').then(m => m.UserListManagementModule), color: 'pupple', icon: 'fal fa-user-shield', canActivate: [AuthGuard]
      }, {
        text: 'Change My Password', path: 'ChangePassword', loadChildren: () => import('../@user-accounts/components/change-password/change-password.module').then(m => m.ChangePasswordModule), color: 'solid-brown', icon: 'fal fa-user-shield', canActivate: [AuthGuard]
      }, {
        text: 'Reset My Password', path: 'ResetPassword/:key', loadChildren: () => import('../@user-accounts/components/reset-password/reset-password.module').then(m => m.ResetPasswordModule), color: 'solid-brown', icon: 'fal fa-user-shield', display: 'none'
      }
    ]
  },
  { path: '**', redirectTo: '', display: 'none' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { relativeLinkResolution: 'legacy' })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
