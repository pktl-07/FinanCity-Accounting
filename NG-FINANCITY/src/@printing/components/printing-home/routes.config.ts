// import { AuthGuard } from 'src/@common/Infrastructure/auth.guard';
// export const routes: any[] = [
//     {
//         text: 'Reports',
//         path: 'Reports',
//         component: () => {
//             let component: any;
//             setTimeout(() => {
//                 import('./printing-home.component').then(c => {
//                     component = c.PrintingHomeComponent;
//                 });
//             }, 10)
//             return component;
//         },
//         canActivate: [AuthGuard],
//         children: [
//             // {
//             //     text: '',
//             //     path: '',
//             //     component: () => {
//             //         let com: any;
//             //         setTimeout(async () => {
//             //             const c = await import('../individual-sales.component');
//             //             com = c.IndividualSalesComponent;
//             //         }, 100);
//             //         return com;
//             //     },
//             //     outlet: 'report',
//             // },
//             //******** Sales Report */
//             {
//                 text: 'Individual Sales',
//                 path: 'IndividualSales',
//                 //component: IndividualSalesComponent,
//                 loadChildren: () => import('../individual-sales.component').then(x => x.Module),
//                 group: 'Sales Report',
//                 outlet: 'report'
//             },
//             {
//                 text: 'Sales Summary',
//                 path: 'SalesSummary',
//                 //component: SalesSummaryComponent,
//                 loadChildren: () => import('../sales-summary.component').then(x => x.Module),
//                 group: 'Sales Report',
//                 outlet: 'report'
//             },
//             {
//                 text: 'Sales Details',
//                 path: 'SalesDetail',
//                 //component: SalesDtailComponent,
//                 loadChildren: () => import('../sales-detail.component').then(x => x.Module),
//                 group: 'Sales Report',
//                 outlet: 'report'
//             },
//             // // {
//             // //   text: 'Sales Management',
//             // //   path: 'SalesManagement',
//             // //   component: SalesManagementComponent,
//             // //   group: 'Sales Report',
//             // //   outlet: 'report'
//             // // },
//             {
//                 text: 'Sales Transaction List',
//                 path: 'PropertyTransactionList',
//                 // component: PropertyTransactionComponent,
//                 loadChildren: () => import('../property-transaction.component').then(x => x.Module),
//                 group: 'Sales Report',
//                 outlet: 'report'
//             },
//             {
//                 text: 'Top Sales',
//                 path: 'TopSales',
//                 loadChildren: () => import('../top-sales-report.component').then(x => x.Module),
//                 group: 'Sales Report',
//                 outlet: 'report'
//             },
//             // //******** Sales Bonus Report */
//             // // {
//             // //   text: 'Recruitment Incentive Report',
//             // //   path: 'RecruitmentIncentiveReport',
//             // //   component: RecruitmentIncentiveReportComponent,
//             // //   group: 'Sales Bonus Report',
//             // //   outlet: 'report'
//             // // },
//             // // {
//             // //   text: 'Recruitment Incentive Payable Report',
//             // //   path: 'RecruitmentIncentivePayableReport',
//             // //   component: RecruitmentIncentivePayableReportComponent,
//             // //   group: 'Sales Bonus Report',
//             // //   outlet: 'report'
//             // // },
//             // // {
//             // //   text: 'Recruitment Incentive Paid Report',
//             // //   path: 'RecruitmentIncentivePaidReport',
//             // //   component: RecruitmentIncentivePaidReportComponent,
//             // //   group: 'Sales Bonus Report',
//             // //   outlet: 'report'
//             // // },
//             {
//                 text: 'Sales Bonus Report',
//                 path: 'SalesBonusReport',
//                 loadChildren: () => import('../sales-bonus-report.component').then(x => x.Module),
//                 group: 'Sales Bonus Report',
//                 outlet: 'report'
//             },
//             // // ******** Accounts Receivable Report */
//             {
//                 text: 'Commission Receivable',
//                 path: 'CommissionReceivableReport',
//                 loadChildren: () => import('../commission-receivable-report.component').then(x => x.Module),
//                 group: 'Accounts Receivable Report',
//                 outlet: 'report'
//             },
//             {
//                 text: 'Commission Received',
//                 path: 'CommissionReceivedReport',
//                 loadChildren: () => import('../commission-received-report.component').then(x => x.Module),
//                 group: 'Accounts Receivable Report',
//                 outlet: 'report'
//             },
//             // ******** Accounts Payable Report*/
//             {
//                 text: 'Commission Payable',
//                 path: 'CommissionPayableReport',
//                 loadChildren: () => import('../commission-payble-to-internal-component').then(x => x.Module),
//                 group: 'Accounts Payable Report',
//                 outlet: 'report'
//             },
//             {
//                 text: 'Commission Payable (By Individual SP)',
//                 path: 'SalespersonCommissionPayableReportV1',
//                 loadChildren: () => import('../commission-payable-v1-report.component').then(x => x.Module),
//                 group: 'Accounts Payable Report',
//                 outlet: 'report'
//             },
//             {
//                 text: 'Account Payable (BS Audit)',
//                 path: 'SalespersonCommissionPayableReportBS',
//                 loadChildren: () => import('../commission-payable-bs-report-component').then(x => x.Module),
//                 group: 'Accounts Payable Report',
//                 outlet: 'report'
//             },
//             {
//                 text: 'Salesperson Commission Paid',
//                 path: 'SalespersonCommissionPaidReport',
//                 loadChildren: () => import('../commission-paid-report-component').then(x => x.Module),
//                 group: 'Accounts Payable Report',
//                 outlet: 'report'
//             },
//             // // {
//             // //   text: 'Commission Paid (By Individual SP - IRAS)',
//             // //   path: 'CommissionPaidStatutoryReport',
//             // //   component: CommissionPiadV1ReportComponent,
//             // //   group: 'Accounts Payable Report',
//             // //   outlet: 'report'
//             // // },
//             // //******** Aged Receivables Report*/
//             {
//                 text: 'Aged Receivables Summary',
//                 path: 'AgedReceivablesSummaryReport',
//                 loadChildren: () => import('../aged-receivables-summary-report.component').then(x => x.Module),
//                 group: 'Aged Receivables Report',
//                 outlet: 'report'
//             }, {
//                 text: 'Aged Receivables Detail',
//                 path: 'AgedReceivablesDetailReport',
//                 loadChildren: () => import('../age-receivables-detail-report.component').then(x => x.Module),
//                 group: 'Aged Receivables Report',
//                 outlet: 'report'
//             },
//             // //******** General Ledger Report*/
//             {
//                 text: 'General Ledger Summary',
//                 path: 'GeneralLedgerSummary',
//                 loadChildren: () => import('../general-ledger-summary-report.component').then(x => x.Module),
//                 group: 'General Ledger Report',
//                 outlet: 'report'
//             },
//             {
//                 text: 'General Ledger Detail',
//                 path: 'GeneralLedgerDetail',
//                 loadChildren: () => import('../general-ledger-detail-report.component').then(x => x.Module),
//                 group: 'General Ledger Report',
//                 outlet: 'report'
//             },
//             {
//                 text: 'General Ledger Ref. Summary',
//                 path: 'GeneralLedgerRefSummary',
//                 loadChildren: () => import('../general-ledger-ref-summary-report.component').then(x => x.Module),
//                 group: 'General Ledger Report',
//                 outlet: 'report'
//             },
//             {
//                 text: 'General Ledger Ref. Detail',
//                 path: 'GeneralLedgerRefDetail',
//                 loadChildren: () => import('../general-ledger-ref-detail-report.component').then(x => x.Module),
//                 group: 'General Ledger Report',
//                 outlet: 'report'
//             },
//             // //******** Cashbook Report */
//             {
//                 text: 'Cashbook Log',
//                 path: 'CashbookLog',
//                 loadChildren: () => import('../cashbook-log-report.component').then(x => x.Module),
//                 group: 'Cashbook Report',
//                 outlet: 'report'
//             },
//             {
//                 text: 'Receipt Log',
//                 path: 'CashbookReceiptLog',
//                 loadChildren: () => import('../cashbook-receipt-report.component').then(x => x.Module),
//                 group: 'Cashbook Report',
//                 outlet: 'report'
//             },
//             {
//                 text: 'Payment Log',
//                 path: 'CashbookPaymentLog',
//                 loadChildren: () => import('../cashbook-payment-log-report.component').then(x => x.Module),
//                 group: 'Cashbook Report',
//                 outlet: 'report'
//             },
//             // //******** Job Report */
//             {
//                 text: 'Job Activity Summary',
//                 path: 'JobActivitySummary',
//                 loadChildren: () => import('../job-actvity-summary-report-component').then(x => x.Module),
//                 group: 'Job Report',
//                 outlet: 'report'
//             },
//             {
//                 text: 'Job Activity Detail',
//                 path: 'JobActivityDetail',
//                 loadChildren: () => import('../job-activity-detail-report.component').then(x => x.Module),
//                 group: 'Job Report',
//                 outlet: 'report'
//             },
//             {
//                 text: 'Job Activity List',
//                 path: 'JobActivityList',
//                 loadChildren: () => import('../job-list-report.component').then(x => x.Module),
//                 group: 'Job Report',
//                 outlet: 'report'
//             },
//             // // {
//             // //   text: 'Financial Report',
//             // //   path: 'JobActivityList',
//             // //   component: JobActivityListReportComponent,
//             // //   group: 'Job Report',
//             // //   outlet: 'report'
//             // // },
//             // //******** Accounting Report */
//             {
//                 text: 'Trial Balance',
//                 path: 'TrialBalance',
//                 loadChildren: () => import('../trial-balance.component').then(x => x.Module),
//                 group: 'Accounting Report',
//                 outlet: 'report'
//             },
//             {
//                 text: 'Balance Sheet',
//                 path: 'BalanceSheet',
//                 loadChildren: () => import('../balance-sheet.component').then(x => x.Module),
//                 group: 'Accounting Report',
//                 outlet: 'report'
//             },
//             {
//                 text: 'Income Statement',
//                 path: 'IncomeStatement',
//                 loadChildren: () => import('../income-statement.component').then(x => x.Module),
//                 group: 'Accounting Report',
//                 outlet: 'report'
//             },
//             {
//                 text: 'Comparable Income Statement [Year]',
//                 path: 'ComparableYearlyIncomeStatement',
//                 loadChildren: () => import('../compare-incomestatement-yearly-report.component').then(x => x.Module),
//                 group: 'Accounting Report',
//                 outlet: 'report'
//             },
//             {
//                 text: 'Comparable Income Statement [Quarter]',
//                 path: 'ComparableQuarterlyIncomeStatement',
//                 loadChildren: () => import('../compare-incomestatement-quarterly-report.component').then(x => x.Module),
//                 group: 'Accounting Report',
//                 outlet: 'report'
//             },
//             {
//                 text: 'Comparable Income Statement [Monthly]',
//                 path: 'ComparableMonthlyIncomeStatement',
//                 loadChildren: () => import('../compare-incomestatement-monthly-report.component').then(x => x.Module),
//                 group: 'Accounting Report',
//                 outlet: 'report'
//             },
//             //******** GST Report */
//             {
//                 text: 'GST (IRAS)',
//                 path: 'IRASReport',
//                 loadChildren: () => import('../iras-report.component').then(x => x.Module),
//                 group: 'GST Report',
//                 outlet: 'report'
//             },
//         ]
//     }
// ]
