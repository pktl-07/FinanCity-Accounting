namespace FC.Domain.Report.DAO
{
    using System;
    using System.Data;
    using System.Linq.Expressions;
    using FC.Core;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;
    using FC.Domain.Report.Filter;
    using FC.Domain.Sales.Entity;

    public class SalesReportDAO : DAO
    {
        #region Methods

        public DataTable IndividulaSalesReport(Expression<Func<IndividualSalesReportFilter, bool>> filter)
        {
            try
            {
                var query = $@"
                    SELECT * FROM ({GetIndividualSalesQuery()}) [tbl]
                    WHERE {filter.ToSQL()}
                    ORDER BY [SalesDate]";

                return DbExcutor.ExcuteDataTable(
                     query: query,
                     connection: "FC.Biz",
                     name: "SaleDetailDataTable");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable SalesSummaryReport(Expression<Func<SalesSummaryReportFilter, bool>> filter)
        {
            try
            {
                var query = $@"SELECT * FROM (
                    {SalesSummaryQuery(filter: filter)}) [tbl]
                WHERE {filter.ToSQL()}";

                return DbExcutor.ExcuteDataTable(
                     query: query,
                     connection: "FC.Biz",
                     name: "SaleSummaryDataTable");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable TopSalesReport(Expression<Func<TopSalesReportFilter, bool>> filter)
        {
            try
            {
                int? fromDate = null;
                int? toDate = null;
                var a = filter.CreateObject((mem, value) =>
                {
                    if (mem == $"{nameof(TopSalesReportFilter.SalesDate)}")
                    {
                        if (fromDate == null)
                            fromDate = int.Parse(value);
                        else if (toDate == null)
                            toDate = int.Parse(value);
                    }
                });

                var query = $@"SELECT 
                    {With<PersonEntity>(x => x.Name)} [Name],
                    {With<PersonEntity>(x => x.NRIC)} [NRIC],
                    SUM(ISNULL({With<PayableSchemeEntity>(x => x.AssociateComm)},0)) [GrossComm],
                    {With<PropertyTransactionEntity>(x => x.SecondRef)} [SecondRef]
                    FROM {Table<PersonEntity>()}
                    LEFT JOIN {Table<PayableSchemeEntity>()} ON {With<PersonEntity>(x => x.SysKey)}={With<PayableSchemeEntity>(x => x.SPSK)}
                    INNER JOIN {Table<AllocationEntity>()} ON {With<AllocationEntity>(x => x.SysKey)} = {With<PayableSchemeEntity>(x => x.ALOSK)}
                    INNER JOIN {Table<PropertyTransactionDetailEntity>()} ON {With<PropertyTransactionDetailEntity>(x => x.PTOSK)} = {With<AllocationEntity>(x => x.HeaderSK)}
                    INNER JOIN {Table<PropertyTransactionEntity>()} ON {With<PropertyTransactionEntity>(x => x.SysKey)}={With<AllocationEntity>(x => x.HeaderSK)}
                    INNER JOIN {Table<SalesInvoiceEntity>()} ON {With<SalesInvoiceEntity>(x => x.SysKey)}={With<PropertyTransactionEntity>(x => x.SysKey)}
                    AND {With<PropertyTransactionDetailEntity>(x => x.SPSK)}= {With<AllocationEntity>(x => x.SPSK)}
                    WHERE {With<AllocationEntity>(x => x.RecordStatus)} >= {(int)RecordStatus.Post}
                    AND {With<PersonEntity>(x => x.TransType)}={(int)TransType.InternalSalesperson}
                    AND {With<PropertyTransactionDetailEntity>(x => x.ReceivableStatus)}={(int)SalesViewType.Close}
                    AND {With<SalesInvoiceEntity>(x => x.InvoiceDate)} BETWEEN {fromDate} AND {toDate}
                    AND {With<AllocationEntity>(x => x.TransType)} = {(int)TransType.AllocationCommission}
                    GROUP BY 
                        {With<PersonEntity>(x => x.Name)},
                        {With<PersonEntity>(x => x.NRIC)},
                        {With<PropertyTransactionEntity>(x => x.SecondRef)}";

                query = $@"SELECT TOP {a.Top} * FROM 
                        ({query}) [tbl] 
                     ORDER BY GrossComm";

                return DbExcutor.ExcuteDataTable(
                     query: query,
                     connection: "FC.Biz",
                     name: "TopSale");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable PropertyTransactionListReport(Expression<Func<PropertyTransactionListReportFilter, bool>> filter)
        {
            try
            {
                var query = $@"SELECT 
                {With<PropertyTransactionEntity>(x => x.R1)} [InvoiceNo],
                dbo.ClientName({With<PropertyTransactionEntity>(x => x.SysKey)}) [Bank],
                dbo.ClientSK({With<PropertyTransactionEntity>(x => x.SysKey)}) [{nameof(PropertyTransactionListReportFilter.VendorSK)}],
                {With<PropertyTransactionEntity>(x => x.GrossCommission)} [InvoiceAmt],
                (SELECT ISNULL(SUM({With<PropertyTransactionDetailEntity>(x => x.GrossComm)}),0) FROM {Table<PropertyTransactionDetailEntity>()} WHERE {With<PropertyTransactionDetailEntity>(x => x.PTOSK)} = {With<PropertyTransactionEntity>(x => x.SysKey)} AND {With<PropertyTransactionDetailEntity>(x => x.TransType)} = {(int)TransType.ExternalSalesPerson}) [CoBrokerPayment],                
                {With<PropertyTransactionEntity>(x => x.TransactedPrice)} [TransactedPrice],
                {With<TNC_TransactionTypeEntity>(x => x.Description)} [TNC_TransactionType],
                {With<TNC_TransactionTypeEntity>(x => x.SysKey)} [{nameof(PropertyTransactionListReportFilter.TransactionType)}],
                (SELECT ISNULL(SUM({With<MarketingExpenseEntity>(x => x.Amount)}),0) FROM {Table<MarketingExpenseEntity>()} WHERE {With<MarketingExpenseEntity>(x => x.PTOSK)}={With<PropertyTransactionEntity>(x => x.SysKey)}) [MarketingExpense],
                CASE {With<PropertyTransactionEntity>(x => x.RecordStatus)} 
                    WHEN {(int)RecordStatus.Void} THEN 'VOID'
                    WHEN {(int)RecordStatus.Post} THEN 'POST'
                    ELSE 'ENTER'
                END [Status],
                {With<PropertyTransactionEntity>(x => x.RecordStatus)} [{nameof(PropertyTransactionListReportFilter.RecordStatus)}],
                {With<PersonEntity>(x => x.Name)} [MainSP],
                dbo.ConvertCustomDateFormat({With<PropertyTransactionEntity>(x => x.TransactionDate)}) [InvoiceDate],
                {With<ContactEntity>(x => x.FloorUnit)} [Floor],
                {With<ContactEntity>(x => x.BuildingHouseNo)} [Block],
                {With<ContactEntity>(x => x.FloorUnit)} [Unit],
                {With<PropertyTransactionEntity>(x => x.TransactionDate)} [{nameof(PropertyTransactionListReportFilter.SalesDate)}],
                {With<ContactEntity>(x => x.PostalCode)} [Postal Code],
                (SELECT {With<TNC_PropertyTypeEntity>(x => x.Description)} FROM {Table<TNC_PropertyTypeEntity>()} WHERE {With<TNC_PropertyTypeEntity>(x => x.SysKey)}={With<TNC_Entity>(x => x.PropertyTypeSK)}) [TNC_PropertyType],
                (SELECT {With<TNC_PropertyTypeEntity>(x => x.SysKey)} FROM {Table<TNC_PropertyTypeEntity>()} WHERE {With<TNC_PropertyTypeEntity>(x => x.SysKey)}={With<TNC_Entity>(x => x.PropertyTypeSK)}) [{nameof(PropertyTransactionListReportFilter.PropertyType)}],
                dbo.AddressFrm({With<ContactEntity>(x => x.BuildingHouseNo)}, {With<ContactEntity>(x => x.Address1)},{With<ContactEntity>(x => x.Address2)}, {With<ContactEntity>(x => x.FloorUnit)}, {With<ContactEntity>(x => x.PostalCode)}) [Address]
                FROM {Table<PropertyTransactionEntity>()}
                INNER JOIN {Table<TNC_Entity>()} ON {With<TNC_Entity>(x => x.SysKey)} = {With<PropertyTransactionEntity>(x => x.SysKey)}
                LEFT JOIN {Table<TNC_TransactionTypeEntity>()} ON {With<TNC_Entity>(x => x.TransactionTypeSK)} = {With<TNC_TransactionTypeEntity>(x => x.SysKey)}
                INNER JOIN {Table<SalesInvoiceEntity>()} ON {With<PropertyTransactionEntity>(x => x.SysKey)}={With<SalesInvoiceEntity>(x => x.SysKey)}
                INNER JOIN {Table<ContactEntity>()} ON {With<ContactEntity>(x => x.SysKey)}={With<PropertyTransactionEntity>(x => x.SysKey)}
                INNER JOIN {Table<PersonEntity>()} ON {With<PersonEntity>(x => x.SysKey)}={With<PropertyTransactionEntity>(x => x.MailingSPSK)}";

                query = $@"SELECT * FROM ({query}) [tbl]
                    WHERE {filter.ToSQL()}
                ORDER BY InvoiceNo";

                return DbExcutor.ExcuteDataTable(
                    query: query,
                    connection: "FC.Biz",
                    name: "SaleSummaryDataTable");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion

        #region Utilities

        private string GetIndividualClosedQuery()
        {
            var query = "";

            query += $@" SELECT 
                dbo.ClientName({With<PropertyTransactionEntity>(x => x.SysKey)}) [ClientName], 
                {With<PropertyTransactionEntity>(x => x.TransactedPrice)} [LoanAmount],
                SUM(ROUND({With<AllocationEntity>(x => x.MarketingExpense)},2)) [MarketingExpense],
                {With<SalesInvoiceEntity>(x => x.SysKey)} [InvoiceSK],
                {With<SalesInvoiceEntity>(x => x.R1)} [InvoiceNo],
                {With<PropertyTransactionEntity>(x => x.R1)} [TransNo],
                {With<SalesInvoiceEntity>(x => x.InvoiceDate)} [SalesDate],
                dbo.ConvertCustomDateFormat({ With<PropertyTransactionEntity>(x => x.TransactionDate)}) [TransDate],
                {With<TNC_PropertyTypeEntity>(x => x.Description)} [PropertyType],
                dbo.ConvertCustomDateFormat({With<SalesInvoiceEntity>(x => x.InvoiceDate)}) [InvoiceDate],
                dbo.AddressFrm({With<ContactEntity>(x => x.BuildingHouseNo)}, {With<ContactEntity>(x => x.Address1)}, {With<ContactEntity>(x => x.Address2)}, {With<ContactEntity>(x => x.FloorUnit)}, {With<ContactEntity>(x => x.PostalCode)}) [PropertyAddress],
                {With<ContactEntity>(x => x.Address1)} [Street],
                {With<PersonEntity>(x => x.SysKey)} [SPSK],
                CASE {With<PropertyTransactionDetailEntity>(x => x.TransType)}
                WHEN {(int)TransType.InternalSalesperson}  THEN '{TransType.InternalSalesperson.ToDescription()}'
                WHEN {(int)TransType.MainSalesperson} THEN '{TransType.MainSalesperson.ToDescription()}'
                WHEN {(int)TransType.ExternalSalesPerson} THEN '{TransType.ExternalSalesPerson.ToDescription()}'
                END [SPType],
                {With<CompanyEntity>(x => x.CompanyName)} [AgencyName],
                {With<PersonEntity>(x => x.Name)} [SPName],
               
                CASE {With<PersonEntity>(x => x.TransType)}
                WHEN {(int)TransType.ExternalSalesPerson} THEN 
                    SUM({With<PayableSchemeEntity>(x => x.GrossComm)}) / ((100 + {With<PropertyTransactionDetailEntity>(x => x.TaxPercent)}) / 100)
                  + SUM(ROUND({With<AllocationEntity>(x => x.MarketingExpense)},2))
                    
                    ELSE SUM(ROUND({With<PayableSchemeEntity>(x => x.GrossComm)},2))
                  + SUM(ROUND({With<AllocationEntity>(x => x.MarketingExpense)},2))
                    END [GrossComm],

                (ISNULL({With<PayableSchemeEntity>(x => x.AssociatePercent)},100 )) [SPPercent],
                
                CASE {With<PersonEntity>(x => x.TransType)}
                    WHEN {(int)TransType.ExternalSalesPerson} 
                    THEN ROUND(SUM({With<PayableSchemeEntity>(x => x.GrossComm)}) / ((100 + {With<PropertyTransactionDetailEntity>(x => x.TaxPercent)}) / 100 ),2) 
                    ELSE 
                    SUM(ROUND({With<PayableSchemeEntity>(x => x.AssociateComm)} ,2)) 
                END [SPNetComm],
                {(int)SalesViewType.Close} [{nameof(ICommissionReceivedStatusFor.ReceivedStatus)}],
                {With<PropertyTransactionDetailEntity>(x => x.PayableStatus)} [{nameof(ICommissionPaidStatusFor.PaidStatus)}],
                '{SalesViewType.Close.ToDescription()}' [Status],
                 CASE {With<PropertyTransactionDetailEntity>(x => x.PayableStatus)}     
                    WHEN {(int)SalesViewType.Pending} THEN '{SalesViewType.Pending.ToDescription()}'
                    WHEN {(int)SalesViewType.Close} THEN '{SalesViewType.Close.ToDescription()}'
                END [PayableStatus]
                FROM {Table<PropertyTransactionDetailEntity>()}
                INNER JOIN {Table<PropertyTransactionEntity>()} ON {With<PropertyTransactionEntity>(x => x.SysKey)} = { With<PropertyTransactionDetailEntity>(x => x.PTOSK)}
                LEFT JOIN { Table<TNC_Entity>()} ON {With<PropertyTransactionEntity>(x => x.SysKey)}={With<TNC_Entity>(x => x.SysKey)}
                INNER JOIN {Table<ContactEntity>()} ON {With<ContactEntity>(x => x.SysKey)}={With<PropertyTransactionEntity>(x => x.SysKey)}
                INNER JOIN {Table<AllocationEntity>()} ON {With<PropertyTransactionDetailEntity>(x => x.SysKey)} = {With<AllocationEntity>(x => x.DetailSK)}
                INNER JOIN {Table<SalesInvoiceEntity>()} ON {With<SalesInvoiceEntity>(x => x.SysKey)} = {With<AllocationEntity>(x => x.HeaderSK)}
                INNER JOIN {Table<PayableSchemeEntity>()} ON {With<AllocationEntity>(x => x.SysKey)} = {With<PayableSchemeEntity>(x => x.ALOSK)}
                INNER JOIN {Table<PersonEntity>()} ON {With<PersonEntity>(x => x.SysKey)} = {With<PayableSchemeEntity>(x => x.SPSK)}
                LEFT JOIN {Table<CompanyEntity>()} ON {With<CompanyEntity>(x => x.SysKey)} = {With<PersonEntity>(x => x.VendorSK)}
                LEFT JOIN  {Table<TNC_PropertyTypeEntity>()} ON {With<TNC_PropertyTypeEntity>(x => x.SysKey)}={With<TNC_Entity>(x => x.PropertyTypeSK)}
                    WHERE {With<PropertyTransactionDetailEntity>(x => x.ReceivableStatus)} = {(int)SalesViewType.Close} 
                    AND {With<AllocationEntity>(x => x.RecordStatus)} = {(int)RecordStatus.Post}
                    AND {With<SalesInvoiceEntity>(x => x.RecordStatus)} = {(int)RecordStatus.Post}
                    AND {With<AllocationEntity>(x => x.TransType)} = {(int)TransType.AllocationCommission}
                GROUP BY
                {With<PropertyTransactionEntity>(x => x.SysKey)},
                {With<PropertyTransactionEntity>(x => x.MailingClientSK)},
                {With<PropertyTransactionEntity>(x => x.TransactedPrice)},
                {With<PropertyTransactionDetailEntity>(x => x.SysKey)},
                {With<PropertyTransactionDetailEntity>(x => x.MarketingExpense)},
                {With<PropertyTransactionDetailEntity>(x => x.TransType)},
                {With<PropertyTransactionDetailEntity>(x => x.TaxPercent)},
                {With<PayableSchemeEntity>(x => x.AssociatePercent)},
                {With<PersonEntity>(x => x.Name)},
                {With<PersonEntity>(x => x.CEANumber)},
                {With<CompanyEntity>(x => x.CompanyName)},
                {With<ContactEntity>(x => x.BuildingHouseNo)},
                {With<ContactEntity>(x => x.Address1)},
                {With<ContactEntity>(x => x.Address2)},
                {With<ContactEntity>(x => x.FloorUnit)},
                {With<ContactEntity>(x => x.PostalCode)},
                {With<PersonEntity>(x => x.TransType)},
                {With<PropertyTransactionEntity>(x => x.TransactionDate)},
                {With<PropertyTransactionEntity>(x => x.R1)},
                {With<SalesInvoiceEntity>(x => x.ReceivableStatus)},
                {With<SalesInvoiceEntity>(x => x.R1)},
                {With<SalesInvoiceEntity>(x => x.Prefix)},
                {With<SalesInvoiceEntity>(x => x.VoucherId)},
                {With<SalesInvoiceEntity>(x => x.InvoiceDate)},
                {With<TNC_PropertyTypeEntity>(x => x.Description)},
                {With<PersonEntity>(x => x.SysKey)},
                {With<AllocationEntity>(x => x.MarketingExpense)},
                {With<PropertyTransactionDetailEntity>(x => x.ReceivableStatus)},
                {With<PropertyTransactionDetailEntity>(x => x.PayableStatus)},
                {With<SalesInvoiceEntity>(x => x.SysKey)}";

            return query;
        }

        private string SalesSummaryQuery(Expression<Func<SalesSummaryReportFilter, bool>> filter)
        {
            var query = $@"SELECT
               dbo.ClientName({With<SalesInvoiceEntity>(x => x.SysKey)}) [Bank],
               dbo.ClientSK({With<SalesInvoiceEntity>(x => x.SysKey)}) [{nameof(IVendorFor.VendorSK)}],
               (SELECT ISNULL(SUM({With<MarketingExpenseEntity>(x => x.Amount)}),0) FROM {Table<MarketingExpenseEntity>()} WHERE {With<MarketingExpenseEntity>(x => x.PTOSK)} = {With<PropertyTransactionEntity>(x => x.SysKey)}) [MarketingExpense],
               {With<SalesInvoiceEntity>(x => x.ReceivableStatus)} [{nameof(ICommissionReceivedStatusFor.ReceivedStatus)}],
               {With<SalesInvoiceEntity>(x => x.PayableStatus)} [{nameof(ICommissionPaidStatusFor.PaidStatus)}],
               dbo.ConvertCustomDateFormat({With<PropertyTransactionEntity>(x => x.SPDate)}) [SubmmissionDate],
               {With<SalesInvoiceEntity>(x => x.SysKey)} [InvoiceSK],
               {With<TNC_PropertyTypeEntity>(x => x.Description)} [TNC_PropertyType],
               {With<TNC_Entity>(x => x.LoanAmount)} [TransactedPrice],
               {With<TNC_TransactionTypeEntity>(x => x.Description)} [TNC_TransactionType],
               {With<PersonEntity>(x => x.Name)} [MainSP],
               {With<PropertyTransactionEntity>(x => x.R1)} [TransactionNo],
               {With<PropertyTransactionEntity>(x => x.SecondRef)} [SecondRef],
               dbo.AddressFrm({With<ContactEntity>(x => x.BuildingHouseNo)}, {With<ContactEntity>(x => x.Address1)},{With<ContactEntity>(x => x.Address2)}, {With<ContactEntity>(x => x.FloorUnit)}, {With<ContactEntity>(x => x.PostalCode)}) [Address],
               {With<SalesInvoiceEntity>(x => x.R1)} [InvoiceNo],
               dbo.ConvertCustomDateFormat({With<SalesInvoiceEntity>(x => x.InvoiceDate)}) [InvoiceDate],
               {With<TNC_TransactionTypeEntity>(x => x.Description)} [TransactionType],
               {With<TNC_PropertyTypeEntity>(x => x.Description)} [PropertyType],
               {With<SalesInvoiceEntity>(x => x.InvoiceDate)} [SalesDate],
               ({With<SalesInvoiceEntity>(x => x.ActualAmt)})
               / (( 100 + {With<SalesInvoiceEntity>(x => x.ActualGST)}) / 100) [InvoiceAmt],
               (SELECT ISNULL(SUM({With<PropertyTransactionDetailEntity>(x => x.GrossComm)}),0)
               FROM {Table<PropertyTransactionDetailEntity>()} WHERE {With<PropertyTransactionDetailEntity>(x => x.TransType)}= {(int)TransType.ExternalSalesPerson} AND {With<PropertyTransactionDetailEntity>(x => x.PTOSK)}= {With<SalesInvoiceEntity>(x => x.SysKey)}) [CoBrokerPayment],
               CASE WHEN {With<SalesInvoiceEntity>(x => x.ReceivableStatus)}={(int)SalesViewType.Pending}
                    THEN '{SalesViewType.Pending.ToDescription()}' 
                    ELSE '{SalesViewType.Close.ToDescription()}' 
               END [Status]
               FROM {Table<PropertyTransactionEntity>()}
               INNER JOIN {Table<ContactEntity>()} ON {With<PropertyTransactionEntity>(x => x.SysKey)}={With<ContactEntity>(x => x.SysKey)}
               INNER JOIN {Table<SalesInvoiceEntity>()} ON { With<SalesInvoiceEntity>(x => x.SysKey)}={ With<PropertyTransactionEntity>(x => x.SysKey)}
               INNER JOIN {Table<PersonEntity>()} ON {With<PersonEntity>(x => x.SysKey)} = {With<PropertyTransactionEntity>(x => x.MailingSPSK)}
               LEFT JOIN {Table<TNC_Entity>()} ON { With<TNC_Entity>(x => x.SysKey)}={ With<SalesInvoiceEntity>(x => x.SysKey)}
               LEFT JOIN {Table<TNC_PropertyTypeEntity>()} ON { With<TNC_PropertyTypeEntity>(x => x.SysKey)}={ With<TNC_Entity>(x => x.PropertyTypeSK)}
               LEFT JOIN {Table<TNC_TransactionTypeEntity>()} ON { With<TNC_TransactionTypeEntity>(x => x.SysKey)}={ With<TNC_Entity>(x => x.TransactionTypeSK)}
               WHERE {With<SalesInvoiceEntity>(x => x.RecordStatus)}>={(int)RecordStatus.Post}";

            return query;
        }

        private string GetIndividualSalesQuery()
        {
            var query = $@" SELECT 
            dbo.ClientName({With<PropertyTransactionEntity>(x => x.SysKey)}) [ClientName],     
            {With<PropertyTransactionEntity>(x => x.TransactedPrice)} [LoanAmount],
            ROUND({With<PropertyTransactionDetailEntity>(x => x.MarketingExpense)},2) [MarketingExpense],
            {With<SalesInvoiceEntity>(x => x.SysKey)} [InvoiceSK],
            {With<PersonEntity>(x => x.FranchiseeSK)} [FranchiseeSK],
            {With<SalesInvoiceEntity>(x => x.R1)} [InvoiceNo],
            {With<PropertyTransactionEntity>(x => x.R1)} [TransNo],
            {With<SalesInvoiceEntity>(x => x.InvoiceDate)} [SalesDate],  
            dbo.ConvertCustomDateFormat({With<PropertyTransactionEntity>(x => x.TransactionDate)}) [TransDate],
            {With<TNC_PropertyTypeEntity>(x => x.Description)} [PropertyType],
            dbo.ConvertCustomDateFormat({With<SalesInvoiceEntity>(x => x.InvoiceDate)}) [InvoiceDate],
            dbo.AddressFrm({With<ContactEntity>(x => x.BuildingHouseNo)}, {With<ContactEntity>(x => x.Address1)},{With<ContactEntity>(x => x.Address2)},{With<ContactEntity>(x => x.FloorUnit)}, {With<ContactEntity>(x => x.PostalCode)}) [PropertyAddress],
            {With<ContactEntity>(x => x.Address1)} [Street],
            {With<PersonEntity>(x => x.NRIC)} [NRIC],
            {With<PersonEntity>(x => x.SysKey)} [SPSK],

            CASE {With<PropertyTransactionDetailEntity>(x => x.TransType)}
                WHEN {(int)TransType.MainSalesperson} THEN '{TransType.MainSalesperson.ToDescription()}'
                WHEN {(int)TransType.InternalSalesperson} THEN '{TransType.InternalSalesperson.ToDescription()}'
                WHEN {(int)TransType.ExternalSalesPerson} THEN '{TransType.ExternalSalesPerson.ToDescription()}'
            END [SPType],

            {With<CompanyEntity>(x => x.CompanyName)} [AgencyName],
            {With<PersonEntity>(x => x.Name)} [SPName],
            {With<PropertyTransactionDetailEntity>(x => x.GrossComm)} [GrossComm],
            {With<PropertyTransactionDetailEntity>(x => x.CurrentCommission)} [SPPercent],

            CASE {With<PersonEntity>(x => x.TransType)}
                 WHEN {(int)TransType.ExternalSalesPerson} 
                 THEN (SELECT
                    ISNULL(SUM(ROUND({With<PayableSchemeEntity>(x => x.GrossComm)},2)),0) / ((100 + {With<PropertyTransactionDetailEntity>(x => x.TaxPercent)}) / 100)
                    FROM {Table<PayableSchemeEntity>()}
                    INNER JOIN {Table<AllocationEntity>()} ON {With<AllocationEntity>(x => x.SysKey)} = {With<PayableSchemeEntity>(x => x.ALOSK)}
                    WHERE {With<AllocationEntity>(x => x.HeaderSK)} = {With<SalesInvoiceEntity>(x => x.SysKey)} 
                    AND {With<AllocationEntity>(x => x.DetailSK)} = {With<PropertyTransactionDetailEntity>(x => x.SysKey)} 
                    AND {With<AllocationEntity>(x => x.RecordStatus)} = {(int)RecordStatus.Post}
                    AND {With<AllocationEntity>(x => x.TransType)} = {(int)TransType.AllocationCommission}) 
                ELSE 
                   (SELECT 
                   ISNULL(SUM(ROUND({With<PayableSchemeEntity>(x => x.AssociateComm)} ,2)), 0) 
                   FROM {Table<PayableSchemeEntity>()} 
                   INNER JOIN {Table<AllocationEntity>()} ON {With<AllocationEntity>(x => x.SysKey)} = {With<PayableSchemeEntity>(x => x.ALOSK)}
                   WHERE {With<AllocationEntity>(x => x.HeaderSK)} = {With<SalesInvoiceEntity>(x => x.SysKey)} 
                   AND {With<AllocationEntity>(x => x.DetailSK)} = {With<PropertyTransactionDetailEntity>(x => x.SysKey)} 
                   AND {With<AllocationEntity>(x => x.RecordStatus)} = {(int)RecordStatus.Post}
                   AND {With<AllocationEntity>(x => x.TransType)} = {(int)TransType.AllocationCommission})
            END [SPNetComm],

            {With<PropertyTransactionDetailEntity>(x => x.ReceivableStatus)} [{nameof(ICommissionReceivedStatusFor.ReceivedStatus)}],
            {With<PropertyTransactionDetailEntity>(x => x.PayableStatus)} [{nameof(ICommissionPaidStatusFor.PaidStatus)}],
          
            CASE {With<PropertyTransactionDetailEntity>(x => x.ReceivableStatus)}     
                WHEN {(int)SalesViewType.Pending} THEN '{SalesViewType.Pending.ToDescription()}'
                WHEN {(int)SalesViewType.Close} THEN '{SalesViewType.Close.ToDescription()}'
            END [Status],

            CASE {With<PropertyTransactionDetailEntity>(x => x.PayableStatus)}     
                WHEN {(int)SalesViewType.Pending} THEN '{SalesViewType.Pending.ToDescription()}'
                WHEN {(int)SalesViewType.Close} THEN '{SalesViewType.Close.ToDescription()}'
            END [PayableStatus]

            FROM {Table<PropertyTransactionEntity>()}
            LEFT JOIN {Table<TNC_Entity>()} ON {With<PropertyTransactionEntity>(x => x.SysKey)}={With<TNC_Entity>(x => x.SysKey)}
            INNER JOIN {Table<PropertyTransactionDetailEntity>()} ON {With<PropertyTransactionEntity>(x => x.SysKey)}={With<PropertyTransactionDetailEntity>(x => x.PTOSK)}
            INNER JOIN {Table<SalesInvoiceEntity>()} ON {With<PropertyTransactionEntity>(x => x.SysKey)}={With<SalesInvoiceEntity>(x => x.SysKey)}
            INNER JOIN {Table<PersonEntity>()} ON {With<PersonEntity>(x => x.SysKey)}={With<PropertyTransactionDetailEntity>(x => x.SPSK)}
            INNER JOIN {Table<EmploymentEntity>()} ON {With<PersonEntity>(x => x.SysKey)}={With<EmploymentEntity>(x => x.SysKey)}
            LEFT JOIN {Table<CompanyEntity>()} ON {With<CompanyEntity>(x => x.SysKey)} = {With<PersonEntity>(x => x.FranchiseeSK)}
            LEFT JOIN {Table<ContactEntity>()} ON {With<ContactEntity>(x => x.SysKey)}= {With<PropertyTransactionEntity>(x => x.SysKey)}
            LEFT JOIN {Table<TNC_PropertyTypeEntity>()} ON {With<TNC_PropertyTypeEntity>(x => x.SysKey)}={With<TNC_Entity>(x => x.PropertyTypeSK)}";

            return query;
        }

        #endregion
    }
}
