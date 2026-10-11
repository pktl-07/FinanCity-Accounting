namespace FC.Domain.Report.DAO
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq.Expressions;
    using FC.Core;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;
    using FC.Domain.Report.Filter;
    using FC.Domain.Sales.Entity;
  
    public class FinancialReportDAO : DAO
    {
        #region Methods

        public DataTable CommissionReceivableReport(Expression<Func<CommissionReceivableReportFilter, bool>> filter)
        {
            try
            {
                int? fromDate = null;
                int? toDate = null;

                var a = filter.CreateObject((mem, value) =>
                {
                    if (mem == $"{nameof(CommissionReceivableReportFilter.SalesDate)}")
                    {
                        if (fromDate == null)
                            fromDate = int.Parse(value);
                        else if (toDate == null)
                            toDate = int.Parse(value);
                    }
                });

                var query = $@"SELECT 
                {With<SalesInvoiceEntity>(x => x.SysKey)} [InvoiceSK],
                dbo.ClientName({With<SalesInvoiceEntity>(x => x.SysKey)}) [Bank],
                dbo.MainClientName({With<SalesInvoiceEntity>(x => x.SysKey)}) [ClientName],";

                if (!string.IsNullOrWhiteSpace(a.SPSK))
                {
                    query += $@"(SELECT ROUND(ISNULL({With<PropertyTransactionDetailEntity>(x => x.GrossComm)},0),2)
                        FROM {Table<PropertyTransactionDetailEntity>()} WHERE {With<PropertyTransactionDetailEntity>(x => x.SPSK)}='{a.SPSK}'
                        AND {With<PropertyTransactionDetailEntity>(x => x.PTOSK)} = {With<SalesInvoiceEntity>(x => x.SysKey)}) [GSTInvoiceAmt], ";

                    query += $@"(SELECT ISNULL(SUM({With<AllocationEntity>(x => x.AmountBeforeGST)}),0)
                        FROM {Table<AllocationEntity>()} WHERE {With<AllocationEntity>(x => x.SPSK)}={a.SPSK}
                        AND {With<AllocationEntity>(x => x.HeaderSK)} = {With<SalesInvoiceEntity>(x => x.SysKey)}
                        AND {With<AllocationEntity>(x => x.TransType)} = {(int)TransType.AllocationCommission}) [ReceivedAmt] ";
                }
                else
                {
                    query += $@"(SELECT [Sub].ActualAmt FROM {Table<SalesInvoiceEntity>()} [Sub] 
                            WHERE [Sub].SysKey = {With<SalesInvoiceEntity>(x => x.SysKey)}) [GSTInvoiceAmt],
                            (SELECT ISNULL(SUM({With<CommissionReceivedEntity>(x => x.TotalAmt)}),0.00)
                            FROM {Table<CommissionReceivedEntity>()} 
                                WHERE {With<CommissionReceivedEntity>(x => x.RecordStatus)}={(int)RecordStatus.Post}
                                AND {With<CommissionReceivedEntity>(x => x.InvoiceSK)} = {With<SalesInvoiceEntity>(x => x.SysKey)}) [ReceivedAmt]";
                }

                query += $@" FROM {Table<SalesInvoiceEntity>()}
                  INNER JOIN {Table<PropertyTransactionEntity>()} ON {With<PropertyTransactionEntity>(x => x.SysKey)}={With<SalesInvoiceEntity>(x => x.SysKey)}
                  INNER JOIN {Table<PersonEntity>()} ON {With<PersonEntity>(x => x.SysKey)} = {With<PropertyTransactionEntity>(x => x.MailingClientSK)}
                  WHERE {With<SalesInvoiceEntity>(x => x.RecordStatus)}={(int)RecordStatus.Post}
                    AND {With<SalesInvoiceEntity>(x => x.ReceivableStatus)}={(int)SalesViewType.Pending}
                    AND {With<SalesInvoiceEntity>(x => x.InvoiceDate)} <= {toDate}";

                if (!string.IsNullOrWhiteSpace(a.SPSK))
                {
                    query += $@" AND {With<SalesInvoiceEntity>(x => x.SysKey)}
                    IN (SELECT {With<PropertyTransactionDetailEntity>(x => x.PTOSK)} 
                    FROM { Table<PropertyTransactionDetailEntity>()} 
                    WHERE {With<PropertyTransactionDetailEntity>(x => x.SPSK)}='{a.SPSK}')";
                }

                query += $@" GROUP BY 
                   {With<SalesInvoiceEntity>(x => x.SysKey)},
                   {With<SalesInvoiceEntity>(x => x.InvoiceDate)},
                   {With<PersonEntity>(x => x.Name)},
                   {With<SalesInvoiceEntity>(x => x.ReceivableStatus)}";

                query += $" ORDER BY {With<SalesInvoiceEntity>(x => x.InvoiceDate)}";

                try
                {
                    return DbExcutor.ExcuteDataTable(
                       query: query,
                       connection: "FC.Biz",
                       name: "CommissionReceivableDataTable");
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public IEnumerable<CommissionReceivedReportDTO> CommissionReceivedReport(Expression<Func<CommissionReceivedReportFilter, bool>> filter)
        {
            try
            {
                int? fromDate = null;
                int? toDate = null;
                var a = filter.CreateObject((mem, value) =>
                {
                    if (mem == $"{nameof(CommissionReceivableReportFilter.SalesDate)}")
                    {
                        if (fromDate == null)
                            fromDate = int.Parse(value);
                        else
                            toDate = int.Parse(value);
                    }
                });

                var query = $@"SELECT {With<CommissionReceivedEntity>(x => x.R1)} [ReceiptNo],
                      dbo.MainClientName({With<SalesInvoiceEntity>(x => x.SysKey)}) [{nameof(CommissionReceivedReportDTO.Client)}],
                      dbo.ClientName({With<SalesInvoiceEntity>(x => x.SysKey)}) [{nameof(CommissionReceivedReportDTO.Bank)}],
                      {With<SalesInvoiceEntity>(x => x.SysKey)} [InvoiceSK],
                      {With<SalesInvoiceEntity>(x => x.R1)} [InvoiceNo],
                      {With<CommissionReceivedEntity>(x => x.ReceiptDate)} [ReceiptDate],
                      {With<SalesInvoiceEntity>(x => x.VoucherId)} [Inv_VoucherId],
                      {With<PropertyTransactionEntity>(x => x.R1)} [TransNo],
                      {With<CommissionReceivedEntity>(x => x.CBSK)} [CBSK],
                      CASE
                      WHEN {With<CommissionReceivedEntity>(x => x.PaymentMode)}={(int)PaymentMode.CHEQUE} THEN '{PaymentMode.CHEQUE.ToDescription()}'
                      WHEN {With<CommissionReceivedEntity>(x => x.PaymentMode)} ={(int)PaymentMode.NETS} THEN '{PaymentMode.NETS.ToDescription().ToUpper()}'
                      WHEN {With<CommissionReceivedEntity>(x => x.PaymentMode)} = {(int)PaymentMode.TT} THEN '{PaymentMode.TT.ToDescription().ToUpper()}'
                      WHEN {With<CommissionReceivedEntity>(x => x.PaymentMode)}={(int)PaymentMode.Cash} THEN '{PaymentMode.Cash.ToDescription().ToUpper()}'
                      END [ReceivedMode],
                      {With<CommissionReceivedEntity>(x => x.ChequeNo)} [ChequeNo],
                      {With<PropertyTransactionEntity>(x => x.SecondRef)} [SecondRef],
                      {With<CommissionReceivedEntity>(x => x.Prefix)} [CR_Prfix],
                      {With<CommissionReceivedEntity>(x => x.VoucherId)} [CR_VoucherID],
                      dbo.AddressFrm({With<ContactEntity>(x => x.BuildingHouseNo)}, {With<ContactEntity>(x => x.Address1)},{With<ContactEntity>(x => x.Address2)}, {With<ContactEntity>(x => x.FloorUnit)}, {With<ContactEntity>(x => x.PostalCode)}) [PropertyContact],";

                if (!string.IsNullOrWhiteSpace(a.SPSK))
                {
                    query += $@"(SELECT ROUND({With<PropertyTransactionDetailEntity>(x => x.GrossComm)},2)
                            FROM {Table<PropertyTransactionDetailEntity>()} WHERE {With<PropertyTransactionDetailEntity>(x => x.SPSK)}='{a.SPSK}'
                            AND {With<PropertyTransactionDetailEntity>(x => x.PTOSK)} = {With<SalesInvoiceEntity>(x => x.SysKey)}) [InvoiceAmt],";

                    query += $@"(SELECT ROUND({With<AllocationEntity>(x => x.AmountBeforeGST)},2) FROM {Table<AllocationEntity>()} WHERE {With<AllocationEntity>(x => x.SPSK)}='{a.SPSK}'
                        AND {With<AllocationEntity>(x => x.HeaderSK)} = {With<SalesInvoiceEntity>(x => x.SysKey)} AND {With<AllocationEntity>(x => x.TransType)} = {(int)TransType.AllocationCommission}) [ReceivedAmt],";
                }
                else
                {
                    query += $@" ROUND({With<SalesInvoiceEntity>(x => x.ActualAmt)},2)  [InvoiceAmt],
                       ROUND({With<CommissionReceivedEntity>(x => x.TotalAmt)},2) [ReceivedAmt],";
                }

                query += $@"{With<CommissionReceivedEntity>(x => x.Auto)} [Auto]
                    FROM {Table<PropertyTransactionEntity>()}
                    INNER JOIN {Table<SalesInvoiceEntity>()} ON {With<PropertyTransactionEntity>(x => x.SysKey)} = {With<SalesInvoiceEntity>(x => x.SysKey)}
                    INNER JOIN {Table<ContactEntity>()} ON {With<PropertyTransactionEntity>(x => x.SysKey)}= {With<ContactEntity>(x => x.SysKey)}
                    INNER JOIN {Table<CommissionReceivedEntity>()} ON {With<SalesInvoiceEntity>(x => x.SysKey)}={With<CommissionReceivedEntity>(x => x.InvoiceSK)}
                    WHERE { With<CommissionReceivedEntity>(x => x.RecordStatus)}= {(int)RecordStatus.Post}
                    AND {With<CommissionReceivedEntity>(x => x.ReceiptDate)} BETWEEN {fromDate} AND {toDate}";

                if (!string.IsNullOrWhiteSpace(a.SPSK))
                {
                    query += $@" AND {With<SalesInvoiceEntity>(x => x.SysKey)}
                    IN (SELECT {With<PropertyTransactionDetailEntity>(x => x.PTOSK)} 
                    FROM {Table<PropertyTransactionDetailEntity>()} 
                    WHERE {With<PropertyTransactionDetailEntity>(x => x.SPSK)}={a.SPSK})";
                }

                if (!string.IsNullOrWhiteSpace(a.CBSK))
                {
                    query += $@" AND {With<CommissionReceivedEntity>(x => x.CBSK)}= '{a.CBSK}'";
                }

                query += $@" ORDER BY {With<CommissionReceivedEntity>(x => x.Prefix)},
                    {With<CommissionReceivedEntity>(x => x.VoucherId)}";

                return DbExcutor<CommissionReceivedReportDTO>.ExcuteReader(query);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable CommissionPayableBSReport(Expression<Func<CommissionPayableBSReportFilter, bool>> filter)
        {
            try
            {
                var query = $@"SELECT 
                        {With<PersonEntity>(x => x.SysKey)} [{nameof(CommissionPayableBSReportFilter.SPSK)}],
                        {With<PersonEntity>(x => x.TransType)} [{nameof(CommissionPayableBSReportFilter.SalespersonType)}],
                        {With<PropertyTransactionEntity>(x => x.R1)} [TransNo],
                        {With<SalesInvoiceEntity>(x => x.R1)} [InvoiceNo],
                        {With<SalesInvoiceEntity>(x => x.SysKey)} [InvoiceSK],
                        dbo.ConvertCustomDateFormat({With<SalesInvoiceEntity>(x => x.InvoiceDate)}) [InvoiceDate],
                        {With<SalesInvoiceEntity>(x => x.InvoiceDate)} [{nameof(CommissionPayableBSReportFilter.SalesDate)}],
                        {With<PersonEntity>(x => x.Name)} [Name],
                      
                        CASE 
                          WHEN {With<PropertyTransactionDetailEntity>(x => x.TransType)} = {(int)TransType.MainSalesperson} THEN 'Main SP'
                          WHEN {With<PropertyTransactionDetailEntity>(x => x.TransType)} = {(int)TransType.InternalSalesperson} THEN 'Internal SP'
                          WHEN {With<PropertyTransactionDetailEntity>(x => x.TransType)} = {(int)TransType.ExternalSalesPerson} THEN 'External SP'
                        END [SPType],
                       
                       dbo.AddressFrm({With<ContactEntity>(x => x.BuildingHouseNo)}, {With<ContactEntity>(x => x.Address1)},{With<ContactEntity>(x => x.Address2)}, {With<ContactEntity>(x => x.FloorUnit)}, {With<ContactEntity>(x => x.PostalCode)}) [PropertyAddress],
                       {With<PropertyTransactionDetailEntity>(x => x.GrossComm)} [GrossComm],
                       {With<PropertyTransactionDetailEntity>(x => x.MarketingExpense)} [MarketingExpense], 
                       {With<CompanyEntity>(x => x.CompanyName)} [CompanyName],
                       
                       ((ROUND({With<PropertyTransactionDetailEntity>(x => x.GrossComm)},2) 
                         - ROUND({With<PropertyTransactionDetailEntity>(x => x.MarketingExpense)},2)) / 100) 
                         * (100 - {With<PropertyTransactionDetailEntity>(x => x.CurrentCommission)}) [AgencyFee],

                        (SELECT ISNULL(SUM({With<CommissionPaymentEntity>(x => x.NetPaid)}),0) FROM {Table<CommissionPaymentEntity>()}
                        INNER JOIN {Table<AllocationEntity>()} ON {With<CommissionPaymentEntity>(x => x.ALOSK)} = {With<AllocationEntity>(x => x.SysKey)}
                        WHERE {With<AllocationEntity>(x => x.DetailSK)} = {With<PropertyTransactionDetailEntity>(x => x.SysKey)}
                        AND {With<AllocationEntity>(x => x.TransType)} = {(int)TransType.AllocationCommission}
                        AND {With<CommissionPaymentEntity>(x => x.RecordStatus)} = {(int)RecordStatus.Post}) [CommissionPaid]

                        FROM {Table<PropertyTransactionDetailEntity>()}
                        INNER JOIN {Table<SalesInvoiceEntity>()} ON {With<SalesInvoiceEntity>(x => x.SysKey)} = {With<PropertyTransactionDetailEntity>(x => x.PTOSK)} 
                        LEFT JOIN {Table<CompanyEntity>() } ON {With<CompanyEntity>(x => x.SysKey) }={With<PropertyTransactionDetailEntity>(x => x.VendorSK)}
                        INNER JOIN {Table<PropertyTransactionEntity>()} ON {With<PropertyTransactionEntity>(x => x.SysKey)} = {With<SalesInvoiceEntity>(x => x.SysKey)} 
                        INNER JOIN {Table<ContactEntity>()} ON {With<ContactEntity>(x => x.SysKey)} = {With<SalesInvoiceEntity>(x => x.SysKey)}
                        INNER JOIN {Table<PersonEntity>()} ON {With<PersonEntity>(x => x.SysKey)} = {With<PropertyTransactionDetailEntity>(x => x.SPSK)}
                        WHERE {With<SalesInvoiceEntity>(x => x.RecordStatus)} = {(int)RecordStatus.Post}
                        AND {With<PropertyTransactionDetailEntity>(x => x.PayableStatus)} = {(int)SalesViewType.Pending}";

                filter = filter.Erase($"{nameof(CommissionPayableBSReportFilter.CobrokerAccuralChangeDate)}");

                query = $@"SELECT * FROM 
                    ({query}) [tbl] 
                    WHERE {filter.ToSQL()}";

                return DbExcutor.ExcuteDataTable(
                     query: query,
                     connection: "FC.Biz",
                     name: "CommissionPayableDataTable");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public decimal OpeningPayableBalance(int startDate)
        {
            var query = $@"SELECT 
                        {With<SalesInvoiceEntity>(x => x.InvoiceDate)} [{nameof(CommissionPayableBSReportFilter.SalesDate)}],
                       
                        ISNULL((((ROUND({With<PropertyTransactionDetailEntity>(x => x.GrossComm)},2) 
                          - ROUND({With<PropertyTransactionDetailEntity>(x => x.MarketingExpense)},2)) / 100) 
                          * ({With<PropertyTransactionDetailEntity>(x => x.CurrentCommission)})),0) 
                        [SPPayable], 
                        
                        (SELECT ISNULL(SUM(ROUND({With<CommissionPaymentEntity>(x => x.NetPaid)},2)),0) FROM {Table<CommissionPaymentEntity>()}
                            INNER JOIN {Table<AllocationEntity>()} ON {With<CommissionPaymentEntity>(x => x.ALOSK)} = {With<AllocationEntity>(x => x.SysKey)}
                            WHERE {With<CommissionPaymentEntity>(x => x.RecordStatus)} = {(int)RecordStatus.Post}
                            AND {With<AllocationEntity>(x => x.DetailSK)} = {With<PropertyTransactionDetailEntity>(x => x.SysKey)}
                            AND {With<AllocationEntity>(x => x.TransType)} = {(int)TransType.AllocationCommission})
                        [SPPaid]

                        FROM {Table<PropertyTransactionDetailEntity>()}
                        INNER JOIN {Table<SalesInvoiceEntity>()} ON {With<SalesInvoiceEntity>(x => x.SysKey)} = {With<PropertyTransactionDetailEntity>(x => x.PTOSK)} 
                        WHERE {With<SalesInvoiceEntity>(x => x.RecordStatus)} = {(int)RecordStatus.Post}
                        AND {With<SalesInvoiceEntity>(x => x.InvoiceDate)} < {startDate}";

            query = $@"SELECT ISNULL(SUM([tbl].SPPayable),0) - ISNULL(SUM([tbl].SPPaid),0) FROM 
                    ({query}) [tbl]";
            try
            {
                return (decimal)DbExcutor.ExcuteScalar(query, "FC.Biz");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable PayableHeaderDataSet(Expression<Func<CommissionPayableReportFilter, bool>> filter)
        {
            try
            {
                string spsk = null;
                int? transType = null;
                int? fromdate = null;
                int? todate = null;
                int? status = null;
                
                filter.CreateObject((mem, value) =>
                {
                    if (mem == $"{nameof(CommissionPayableReportFilter.SPSK)}")
                        if (spsk == null)
                            spsk = value;
                    if (mem == $"{nameof(CommissionPayableReportFilter.SalespersonType)}")
                        if (transType == null)
                            transType = int.Parse(value);
                    if (mem == $"{nameof(CommissionPayableReportFilter.SalesDate)}")
                    {
                        if (fromdate == null)
                            fromdate = int.Parse(value);
                        else if (todate == null)
                            todate = int.Parse(value);
                    }

                    if (mem == $"{nameof(CommissionPayableReportFilter.PaidStatus)}")
                        if (status == null)
                            status = int.Parse(value);
                });

                var query = $@" 

                SELECT {With<SalesInvoiceEntity>(x => x.SysKey)} [InvoiceSK],
                {With<SalesInvoiceEntity>(x => x.InvoiceDate)} [{nameof(CommissionPayableReportFilter.SalesDate)}],    
                {With<SalesInvoiceEntity>(x => x.R1)} [InvoiceNo],
                dbo.AddressFrm({With<ContactEntity>(x => x.BuildingHouseNo)}, {With<ContactEntity>(x => x.Address1)},{With<ContactEntity>(x => x.Address2)}, {With<ContactEntity>(x => x.FloorUnit)}, {With<ContactEntity>(x => x.PostalCode)}) [PropertyAddress],
                {With<PropertyTransactionEntity>(x => x.R1)} [TransNo],
                dbo.ConvertCustomDateFormat({With<SalesInvoiceEntity>(x => x.InvoiceDate)}) [InvoiceDate],
                {With<SalesInvoiceEntity>(x => x.ActualAmt)} [InvoiceAmount],
                (SELECT SUM(ROUND({With<PropertyTransactionDetailEntity>(x => x.MarketingExpense)},2)) FROM {Table<PropertyTransactionDetailEntity>()} WHERE {With<PropertyTransactionDetailEntity>(x => x.PTOSK)}={With<SalesInvoiceEntity>(x => x.SysKey)}) [MarketingExpense],
                
                CASE {With<SalesInvoiceEntity>(x => x.ReceivableStatus)}
                WHEN {(int)SalesViewType.Pending} THEN (SELECT SUM((ROUND({With<PropertyTransactionDetailEntity>(x => x.GrossComm)},2)) * (100 - {With<PropertyTransactionDetailEntity>(x => x.CurrentCommission)})) FROM {Table<PropertyTransactionDetailEntity>()} WHERE {With<PropertyTransactionDetailEntity>(x => x.PTOSK)}={With<SalesInvoiceEntity>(x => x.SysKey)})
                WHEN {(int)SalesViewType.Close} THEN (SELECT SUM(ROUND({With<PayableSchemeEntity>(x => x.AgencyComm)},2)) FROM {Table<PayableSchemeEntity>()} WHERE {With<PayableSchemeEntity>(x => x.InvoiceSK)}={With<SalesInvoiceEntity>(x => x.SysKey)})
                END [CompanyFee],
                
                CASE {With<SalesInvoiceEntity>(x => x.PayableStatus)}
                WHEN {(int)SalesViewType.Pending} THEN '{SalesViewType.Pending.ToDescription()}'
                WHEN {(int)SalesViewType.Close} THEN '{SalesViewType.Close.ToDescription()}'
                END [PayableStatus],
                
                {With<SalesInvoiceEntity>(x => x.PayableStatus)} [{nameof(CommissionPayableReportFilter.PaidStatus)}]
                FROM {Table<SalesInvoiceEntity>()}
                INNER JOIN {Table<ContactEntity>()} ON {With<ContactEntity>(x => x.SysKey)} = {With<SalesInvoiceEntity>(x => x.SysKey)}
                INNER JOIN {Table<PropertyTransactionEntity>()} ON {With<PropertyTransactionEntity>(x => x.SysKey)} = {With<SalesInvoiceEntity>(x => x.SysKey)}
                WHERE {With<SalesInvoiceEntity>(x => x.RecordStatus)} = {(int)RecordStatus.Post}
                AND {With<SalesInvoiceEntity>(x => x.SysKey)} IN (SELECT {With<AllocationEntity>(x => x.HeaderSK)} FROM {Table<AllocationEntity>()} WHERE {With<AllocationEntity>(x => x.TransType)} = {(int)TransType.AllocationCommission} AND {With<AllocationEntity>(x => x.RecordStatus)} = {(int)RecordStatus.Post})";

                if (!string.IsNullOrWhiteSpace(spsk))
                    query += $@" AND {With<SalesInvoiceEntity>(x => x.SysKey)} IN (SELECT {With<PropertyTransactionDetailEntity>(x => x.PTOSK)} FROM {Table<PropertyTransactionDetailEntity>()} WHERE {With<PropertyTransactionDetailEntity>(x => x.SPSK)} ='{spsk}')";

                if (transType != null)
                {
                    if (transType == (int)TransType.InternalSalesperson)
                        query += $@" AND {With<SalesInvoiceEntity>(x => x.SysKey)} IN (SELECT {With<PropertyTransactionDetailEntity>(x => x.PTOSK)} FROM {Table<PropertyTransactionDetailEntity>()} WHERE {With<PropertyTransactionDetailEntity>(x => x.TransType)} BETWEEN {(int)TransType.InternalSalesperson} AND {(int)TransType.MainSalesperson})";

                    else if (transType == (int)TransType.ExternalSalesPerson)
                        query += $@" AND {With<SalesInvoiceEntity>(x => x.SysKey)} IN (SELECT {With<PropertyTransactionDetailEntity>(x => x.PTOSK)} FROM {Table<PropertyTransactionDetailEntity>()} WHERE {With<PropertyTransactionDetailEntity>(x => x.TransType)}={transType})";
                }

                if (status == (int)SalesViewType.Close)
                {
                    filter = filter.Erase($"{nameof(CommissionPayableReportFilter.SPSK)}",
                       $"{nameof(CommissionPayableReportFilter.SalespersonType)}");
                }
                else
                {
                    filter = filter.Erase($"{nameof(CommissionPayableReportFilter.SPSK)}",
                        $"{nameof(CommissionPayableReportFilter.SalespersonType)}",
                        $"{nameof(CommissionPayableReportFilter.SalesDate)}");

                    filter = filter.Constant(x => x.SalesDate <= todate);
                }

                query = $@"SELECT * FROM ({query}) [tbl]
                WHERE {filter.ToSQL()}
                ORDER BY [tbl].[InvoiceNo]";

                return DbExcutor.ExcuteDataTable(
                     query: query,
                     connection: "FC.Biz",
                     name: "PayableHeaderDataTable");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable PayableDetailDataSet(Expression<Func<CommissionPayableReportFilter, bool>> filter)
        {
            try
            {
                int? transType = null;
                int? fromdate = null;
                int? todate = null;
                int? status = null;
                int? fromPaidDate = null;
                int? toPaidDate = null;

                filter.CreateObject((mem, value) =>
                {
                    if (mem == $"{nameof(CommissionPayableReportFilter.SalespersonType)}")
                        if (transType == null)
                            transType = int.Parse(value);
                    if (mem == "SalesDate")
                    {
                        if (fromdate == null)
                            fromdate = int.Parse(value);
                        else if (todate == null)
                            todate = int.Parse(value);
                    }

                    if (mem == $"{nameof(CommissionPayableReportFilter.PaidStatus)}")
                        if (status == null)
                            status = int.Parse(value);

                    if (mem == $"{nameof(CommissionPayableReportFilter.PaidDate)}")
                    {
                        if (fromPaidDate == null)
                            fromPaidDate = int.Parse(value);
                        else if (toPaidDate == null)
                            toPaidDate = int.Parse(value);
                    }
                });

                var query = $@" 
                SELECT 
                {With<SalesInvoiceEntity>(x => x.SysKey)} [InvoiceSK],    
                {With<PropertyTransactionDetailEntity>(x => x.PayableStatus)} [{nameof(CommissionPayableReportFilter.PaidStatus)}],
                {With<SalesInvoiceEntity>(x => x.InvoiceDate)} [{nameof(CommissionPayableReportFilter.SalesDate)}],
                {With<PropertyTransactionDetailEntity>(x => x.SPSK)} [{nameof(CommissionPayableReportFilter.SPSK)}],
                {With<PersonEntity>(x => x.Name)} [SalespersonName],
                {With<CompanyEntity>(x => x.CompanyName)} [CompanyName],
               
                --(SELECT ISNULL(SUM(ROUND({With<AllocationEntity>(x => x.MarketingExpense)},2)),0) FROM {Table<AllocationEntity>()} WHERE {With<AllocationEntity>(x => x.DetailSK)} = {With<PropertyTransactionDetailEntity>(x => x.SysKey)} AND {With<AllocationEntity>(x => x.RecordStatus)} = {(int)RecordStatus.Post} AND {With<AllocationEntity>(x => x.TransType)} = {(int)TransType.AllocationCommission}) [MarketingExpense],
                --(SELECT ISNULL(SUM(ROUND({With<AllocationEntity>(x => x.AmountBeforeGST)},2)),0) FROM {Table<AllocationEntity>()} WHERE {With<AllocationEntity>(x => x.DetailSK)} = {With<PropertyTransactionDetailEntity>(x => x.SysKey)} AND {With<AllocationEntity>(x => x.RecordStatus)} = {(int)RecordStatus.Post} AND {With<AllocationEntity>(x => x.TransType)} = {(int)TransType.AllocationCommission}) [GrossComm],
                {With<PropertyTransactionDetailEntity>(x => x.GrossComm)} [GrossComm],
                {With<PropertyTransactionDetailEntity>(x => x.MarketingExpense)} [MarketingExpense],

                {With<PropertyTransactionDetailEntity>(x => x.TransType)} [{nameof(CommissionPayableReportFilter.SalespersonType)}],
                CASE {With<PropertyTransactionDetailEntity>(x => x.TransType)}
                WHEN {(int)TransType.MainSalesperson} THEN '{TransType.MainSalesperson.ToDescription()}'
                WHEN {(int)TransType.InternalSalesperson} THEN '{TransType.InternalSalesperson.ToDescription()}'
                WHEN {(int)TransType.ExternalSalesPerson} THEN '{TransType.ExternalSalesPerson.ToDescription()}'
                END [SPType],

                CASE {With<PropertyTransactionDetailEntity>(x => x.PayableStatus)}
                WHEN {(int)SalesViewType.Pending} THEN '{SalesViewType.Pending.ToDescription()}'
                WHEN {(int)SalesViewType.Close} THEN '{SalesViewType.Close.ToDescription()}'
                END [PayableStatus],";

                if (fromPaidDate != null)
                {
                    query += $@"{With<CommissionPaymentEntity>(x => x.DatePaid)} [{nameof(CommissionPayableReportFilter.PaidDate)}],
                            dbo.ConvertCustomDateFormat({With<CommissionPaymentEntity>(x => x.DatePaid)}) [PaymentDate],";
                }

                query += $@"
                --(SELECT SUM(ROUND({With<PayableSchemeEntity>(x => x.AssociateComm)},2)) FROM {Table<PayableSchemeEntity>()} INNER JOIN {Table<AllocationEntity>()} ON {With<PayableSchemeEntity>(x => x.ALOSK)} = {With<AllocationEntity>(x => x.SysKey)} WHERE {With<AllocationEntity>(x => x.DetailSK)} = {With<PropertyTransactionDetailEntity>(x => x.SysKey)} AND {With<AllocationEntity>(x => x.RecordStatus)} = 8 AND {With<AllocationEntity>(x => x.TransType)} = {(int)TransType.AllocationCommission}) [SalespersonPayable],
                --(SELECT SUM(ROUND({With<PayableSchemeEntity>(x => x.AgencyComm)},2)) FROM {Table<PayableSchemeEntity>()} INNER JOIN {Table<AllocationEntity>()} ON {With<PayableSchemeEntity>(x => x.ALOSK)} = {With<AllocationEntity>(x => x.SysKey)} WHERE {With<AllocationEntity>(x => x.DetailSK)} = {With<PropertyTransactionDetailEntity>(x => x.SysKey)} AND {With<AllocationEntity>(x => x.RecordStatus)} = 8 AND {With<AllocationEntity>(x => x.TransType)} = {(int)TransType.AllocationCommission}) [CompanyFee],
                ((({With<PropertyTransactionDetailEntity>(x => x.GrossComm)} - {With<PropertyTransactionDetailEntity>(x => x.MarketingExpense)}) / 100) * ({With<PropertyTransactionDetailEntity>(x => x.CurrentCommission)})) [SalespersonPayable],
                ((({With<PropertyTransactionDetailEntity>(x => x.GrossComm)} - {With<PropertyTransactionDetailEntity>(x => x.MarketingExpense)}) / 100) * (100 - {With<PropertyTransactionDetailEntity>(x => x.CurrentCommission)})) [CompanyFee],
                (SELECT ISNULL(SUM(ROUND({With<CommissionPaymentEntity>(x => x.NetPaid)},2)),0) FROM {Table<CommissionPaymentEntity>()} INNER JOIN {Table<AllocationEntity>()} ON {With<AllocationEntity>(x => x.SysKey)} = {With<CommissionPaymentEntity>(x => x.ALOSK)} WHERE {With<AllocationEntity>(x => x.DetailSK)} = {With<PropertyTransactionDetailEntity>(x => x.SysKey)} AND {With<CommissionPaymentEntity>(x => x.RecordStatus)} = 8 AND {With<AllocationEntity>(x => x.TransType)} = {(int)TransType.AllocationCommission}) [SalespersonPaid]                 
                FROM {Table<SalesInvoiceEntity>()}
                INNER JOIN {Table<PropertyTransactionDetailEntity>()} ON {With<SalesInvoiceEntity>(x => x.SysKey)} = {With<PropertyTransactionDetailEntity>(x => x.PTOSK)}
                INNER JOIN {Table<PersonEntity>()} ON {With<PersonEntity>(x => x.SysKey)} = {With<PropertyTransactionDetailEntity>(x => x.SPSK)}
                INNER JOIN {Table<CompanyEntity>()} ON {With<CompanyEntity>(x => x.SysKey)} = {With<PersonEntity>(x => x.VendorSK)}
                ";

                if (fromPaidDate != null)
                {
                    query += $@"INNER JOIN {Table<AllocationEntity>()} ON {With<AllocationEntity>(x => x.DetailSK)} = {With<PropertyTransactionDetailEntity>(x => x.SysKey)}
                                INNER JOIN {Table<CommissionPaymentEntity>()} ON {With<AllocationEntity>(x => x.SysKey)} = {With<CommissionPaymentEntity>(x => x.ALOSK)}";
                }

                query += $@"WHERE {With<SalesInvoiceEntity>(x => x.RecordStatus)} = {(int)RecordStatus.Post}";

                if (fromPaidDate != null)
                {
                    query += $@" AND {With<CommissionPaymentEntity>(x => x.RecordStatus)} = {(int)RecordStatus.Post}
                    AND {With<AllocationEntity>(x => x.TransType)} = {(int)TransType.AllocationCommission}";
                }

                if (transType != null)
                {
                    if (transType == (int)TransType.InternalSalesperson)
                        query += $@" AND {With<SalesInvoiceEntity>(x => x.SysKey)} IN (SELECT {With<PropertyTransactionDetailEntity>(x => x.PTOSK)} FROM {Table<PropertyTransactionDetailEntity>()} WHERE {With<PropertyTransactionDetailEntity>(x => x.TransType)} BETWEEN {(int)TransType.InternalSalesperson} AND {(int)TransType.MainSalesperson})";

                    else if (transType == (int)TransType.ExternalSalesPerson)
                        query += $@" AND {With<SalesInvoiceEntity>(x => x.SysKey)} IN (SELECT {With<PropertyTransactionDetailEntity>(x => x.PTOSK)} FROM {Table<PropertyTransactionDetailEntity>()} WHERE {With<PropertyTransactionDetailEntity>(x => x.TransType)}={transType})";
                }

                if (status == (int)SalesViewType.Close)
                {
                    filter = filter.Erase($"{nameof(CommissionPayableReportFilter.SPSK)}",
                       $"{nameof(CommissionPayableReportFilter.SalespersonType)}");
                }
                else
                {
                    filter = filter.Erase($"{nameof(CommissionPayableReportFilter.SPSK)}",
                        $"{nameof(CommissionPayableReportFilter.SalespersonType)}",
                        $"{nameof(CommissionPayableReportFilter.SalesDate)}");

                    filter = filter.Constant((x) => x.SalesDate <= todate);
                }

                query = $@"SELECT * FROM ({query}) [tbl]
                WHERE {filter.ToSQL()}";

                return DbExcutor.ExcuteDataTable(
                     query: query,
                     connection: "FC.Biz",
                     name: "PayableDetailDataTable");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        #endregion
    }
}
