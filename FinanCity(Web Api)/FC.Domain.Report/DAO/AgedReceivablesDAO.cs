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

    public class AgedReceivablesDAO : DAO
    {
        #region Methods

        public DataTable AgedReceivableSummaryReport(Expression<Func<AgedReceivableFilter, bool>> filter)
        {
            try
            {
                var query = $@"SELECT [Tbl].ClientName,
                           [Tbl].ClientSK [ClientSK],
                               SUM([Tbl].InvoiceAmount) [TotalInvoiceAmount],
                               SUM([Tbl].Thirty) [Thirty],
                               SUM([Tbl].Sixty) [Sixty],
                               SUM([Tbl].Ninety) [Ninety],
                               SUM([Tbl].MoreThanNinety) [MoreThanNinety]
                           FROM ({AgingQuery(filter: filter)}) [Tbl] 
                           WHERE {filter.ToSQL()}
                           GROUP BY 
                           [Tbl].ClientSK,
                           [Tbl].ClientName
                           ORDER BY [Tbl].ClientName";

                return DbExcutor.ExcuteDataTable(
                      query: query,
                      connection: "FC.Biz",
                      name: "AgingReportSummaryDataTable");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable AgedReceivableDetailReport(Expression<Func<AgedReceivableFilter, bool>> filter)
        {
            try
            {
                var query = $@"SELECT * FROM ({AgingQuery(filter: filter)}) [tbl]
                        WHERE {filter.ToSQL()}
                        ORDER BY [tbl].InvoiceNo";

                return DbExcutor.ExcuteDataTable(
                    query: query,
                    connection: "FC.Biz",
                    name: "AgingReportDetailDataTable");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string AgingQuery(Expression<Func<AgedReceivableFilter, bool>> filter)
        {
            var a = filter.CreateObject();
            var query = $@"SELECT 
                   dbo.ClientSK({With<PropertyTransactionEntity>(x => x.SysKey)}) [{nameof(AgedReceivableFilter.ClientSK)}],  
                   {With<SalesInvoiceEntity>(x => x.InvoiceDate)} [{nameof(AgedReceivableFilter.SalesDate)}],
                   {With<SalesInvoiceEntity>(x => x.SysKey)} [InvoiceSK],
                   dbo.ClientName({With<PropertyTransactionEntity>(x => x.SysKey)}) [ClientName],
                   {With<SalesInvoiceEntity>(x => x.R1)} [InvoiceNo],
                   {With<PropertyTransactionEntity>(x => x.R1)} [SecondRef],
                   {With<PaymentTermEntity>(x => x.Description)} [PaymentTerm],
                   dbo.ConvertCustomDateFormat({With<SalesInvoiceEntity>(x => x.InvoiceDate)}) [InvoiceDate],
                   ({With<SalesInvoiceEntity>(x => x.ActualAmt)} ) [InvoiceAmount],
                   dbo.ConvertCustomDateFormat(dbo.AddCustomDayFormat({With<SalesInvoiceEntity>(x => x.InvoiceDate)}, {With<PaymentTermEntity>(x => x.Day)})) [DueDate],
                   CASE 
                     WHEN DATEDIFF(day, dbo.AddCustomDayFormat({With<SalesInvoiceEntity>(x => x.InvoiceDate)}, {With<PaymentTermEntity>(x => x.Day)}) , CAST({a.SalesDate} AS VARCHAR(50))) <= 30 
                     THEN {With<SalesInvoiceEntity>(x => x.ActualAmt)}
                        - (SELECT ISNULL(SUM({With<CommissionReceivedEntity>(x => x.TotalAmt)}),0)
                        FROM {Table<CommissionReceivedEntity>()}  
                        WHERE {With<CommissionReceivedEntity>(x => x.RecordStatus)} = {(int)RecordStatus.Post} 
                        AND DATEDIFF(day, dbo.AddCustomDayFormat({With<SalesInvoiceEntity>(x => x.InvoiceDate)}, {With<PaymentTermEntity>(x => x.Day)}) , CAST({a.SalesDate} AS VARCHAR(50))) <= 30  
                        AND {With<CommissionReceivedEntity>(x => x.InvoiceSK)} = {With<SalesInvoiceEntity>(x => x.SysKey)})
                    ELSE 
                    0 
                    END [Thirty],
                    CASE 
                        WHEN DATEDIFF(day, dbo.AddCustomDayFormat({With<SalesInvoiceEntity>(x => x.InvoiceDate)}, {With<PaymentTermEntity>(x => x.Day)}) , CAST({a.SalesDate} AS VARCHAR(50))) BETWEEN 31 AND 60 
                        THEN  {With<SalesInvoiceEntity>(x => x.ActualAmt)}
                        - (SELECT ISNULL(SUM( {With<CommissionReceivedEntity>(x => x.TotalAmt)} ),0) 
                        FROM  {Table<CommissionReceivedEntity>()}  
                        WHERE  {With<CommissionReceivedEntity>(x => x.RecordStatus)} = {(int)RecordStatus.Post} 
                        AND  DATEDIFF(day, dbo.AddCustomDayFormat({With<SalesInvoiceEntity>(x => x.InvoiceDate)}, {With<PaymentTermEntity>(x => x.Day)}) , CAST({a.SalesDate} AS VARCHAR(50))) BETWEEN 31 AND 60  
                        AND  {With<CommissionReceivedEntity>(x => x.InvoiceSK)} = {With<SalesInvoiceEntity>(x => x.SysKey)} )
                     ELSE 
                     0 
                     END [Sixty],
                CASE 
                     WHEN DATEDIFF(day, dbo.AddCustomDayFormat({With<SalesInvoiceEntity>(x => x.InvoiceDate)}, {With<PaymentTermEntity>(x => x.Day)}) , CAST({a.SalesDate} AS VARCHAR(50))) BETWEEN 61 AND 90 
                     THEN  {With<SalesInvoiceEntity>(x => x.ActualAmt)} 
                        - (SELECT ISNULL(SUM( {With<CommissionReceivedEntity>(x => x.TotalAmt)}),0) 
                         FROM  {Table<CommissionReceivedEntity>()}  WHERE  {With<CommissionReceivedEntity>(x => x.RecordStatus)}  = {(int)RecordStatus.Post} 
                         AND DATEDIFF(day, dbo.AddCustomDayFormat({With<SalesInvoiceEntity>(x => x.InvoiceDate)}, {With<PaymentTermEntity>(x => x.Day)}) , CAST({a.SalesDate} AS VARCHAR(50))) BETWEEN 61 AND 90
                         AND {With<CommissionReceivedEntity>(x => x.InvoiceSK)} = {With<SalesInvoiceEntity>(x => x.SysKey)})
                     ELSE 
                     0 
                     END [Ninety],
                CASE 
                     WHEN DATEDIFF(day, dbo.AddCustomDayFormat({With<SalesInvoiceEntity>(x => x.InvoiceDate)}, {With<PaymentTermEntity>(x => x.Day)}) , CAST({a.SalesDate} AS VARCHAR(50))) > 90 
                     THEN  {With<SalesInvoiceEntity>(x => x.ActualAmt)} 
                        - (SELECT ISNULL(SUM( {With<CommissionReceivedEntity>(x => x.TotalAmt)}),0) 
                        FROM  {Table<CommissionReceivedEntity>()}  
                        WHERE  {With<CommissionReceivedEntity>(x => x.RecordStatus)}  = {(int)RecordStatus.Post} 
                        AND  DATEDIFF(day, dbo.AddCustomDayFormat({With<SalesInvoiceEntity>(x => x.InvoiceDate)}, {With<PaymentTermEntity>(x => x.Day)}) , CAST({a.SalesDate} AS VARCHAR(50))) > 90  
                        AND  {With<CommissionReceivedEntity>(x => x.InvoiceSK)} = {With<SalesInvoiceEntity>(x => x.SysKey)} )
                     ELSE 
                     0 
                     END [MoreThanNinety]
                FROM {Table<SalesInvoiceEntity>()}
                    INNER JOIN {Table<PaymentTermEntity>()} ON {With<PaymentTermEntity>(x => x.SysKey)} = {With<SalesInvoiceEntity>(x => x.PaymentTermSK)}
                    INNER JOIN {Table<PropertyTransactionEntity>()} ON {With<PropertyTransactionEntity>(x => x.SysKey)} = {With<SalesInvoiceEntity>(x => x.SysKey)}
                    INNER JOIN {Table<PersonEntity>()} ON {With<PersonEntity>(x => x.SysKey)} = {With<PropertyTransactionEntity>(x => x.MailingClientSK)}
                        WHERE {With<SalesInvoiceEntity>(x => x.RecordStatus)}={(int)RecordStatus.Post}
                        AND {With<SalesInvoiceEntity>(x => x.InvoiceDate)}<={a.SalesDate}
                        AND FLOOR({With<SalesInvoiceEntity>(x => x.ActualAmt)}) - FLOOR(ISNULL((SELECT SUM({With<CommissionReceivedEntity>(x => x.TotalAmt)}) FROM {Table<CommissionReceivedEntity>()}
                        WHERE {With<CommissionReceivedEntity>(x => x.InvoiceSK)}={With<SalesInvoiceEntity>(x => x.SysKey)}
                        AND {With<CommissionReceivedEntity>(x => x.ReceiptDate)} <= {a.SalesDate} 
                    
                    AND {With<CommissionReceivedEntity>(x => x.RecordStatus)}={(int)RecordStatus.Post}),0)) > 0";

            return query;
        }

        #endregion
    }
}
