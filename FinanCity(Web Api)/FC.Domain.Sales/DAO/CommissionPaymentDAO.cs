namespace FC.Domain.Sales.DAO
{
    using System;
    using System.Data;
    using System.Linq.Expressions;
    using FC.Core;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Entity;

    public class CommissionPaymentDAO : DAO<CommissionPaymentEntity>
    {
        #region Methods

        public IDataPagerResult<CommissionPayableDTO> GetPager(int currentPage, int pageSize, Expression<Func<CommissionPayableDTO, bool>> where = null)
        {
            try
            {
                var query = $@" SELECT 
                 {With<SalesInvoiceEntity>(x => x.SysKey)} [{nameof(CommissionPayableDTO.InvoiceSK)}],
                 {With<SalesInvoiceEntity>(x => x.R1)} [{nameof(CommissionPayableDTO.InvoiceNo)}],
                 {With<PropertyTransactionEntity>(x => x.R1)} [{nameof(CommissionPayableDTO.SalesOrderNo)}],
                 ROUND({With<SalesInvoiceEntity>(x => x.ActualGST)},2) [{nameof(CommissionPayableDTO.GSTPercent)}],
                 dbo.[AddressFrm]({With<ContactEntity>(x => x.BuildingHouseNo)}, {With<ContactEntity>(x => x.Address1)}, {With<ContactEntity>(x => x.Address2)} , {With<ContactEntity>(x => x.FloorUnit)}, {With<ContactEntity>(x => x.PostalCode)}) [{nameof(CommissionPayableDTO.Address)}],            
                 (SELECT SUM(ROUND({With<AllocationEntity>(x => x.AmountBeforeGST)},2)) FROM {Table<AllocationEntity>()} WHERE {With<AllocationEntity>(x => x.HeaderSK)} = {With<SalesInvoiceEntity>(x => x.SysKey)} AND {With<AllocationEntity>(x => x.TransType)} = {(int)TransType.AllocationCommission}) [{nameof(CommissionPayableDTO.GrossReceivableAmt)}], 
                 (SELECT ROUND(SUM({With<CommissionReceivedEntity>(x => x.TotalAmt)}),2) FROM {Table<CommissionReceivedEntity>()} WHERE {With<CommissionReceivedEntity>(x => x.InvoiceSK)}={With<SalesInvoiceEntity>(x => x.SysKey)} AND {With<CommissionReceivedEntity>(x => x.RecordStatus)}= {(int)RecordStatus.Post}) [{nameof(CommissionPayableDTO.ReceivableAmt)}],
                 (SELECT SUM(ROUND({With<AllocationEntity>(x => x.MarketingExpense)},2)) FROM {Table<AllocationEntity>()} WHERE {With<AllocationEntity>(x => x.HeaderSK)} = {With<SalesInvoiceEntity>(x => x.SysKey)} AND {With<AllocationEntity>(x => x.TransType)} = {(int)TransType.AllocationCommission} ) [{nameof(CommissionPayableDTO.MarketingExpense)}],
                 (SELECT SUM(ROUND({With<PayableSchemeEntity>(x => x.AgencyComm)},2)) FROM {Table<PayableSchemeEntity>()} WHERE {With<PayableSchemeEntity>(x => x.ALOSK)} IN (SELECT {With<AllocationEntity>(x => x.SysKey)} FROM {Table<AllocationEntity>()} WHERE {With<AllocationEntity>(x => x.RecordStatus)} = {(int)RecordStatus.Post} AND {With<AllocationEntity>(x => x.HeaderSK)} = {With<SalesInvoiceEntity>(x => x.SysKey)} AND {With<AllocationEntity>(x => x.TransType)} = {(int)TransType.AllocationCommission})) [{nameof(CommissionPayableDTO.AgencyComm)}],
                 (SELECT SUM(ROUND({With<PayableSchemeEntity>(x => x.AssociateComm)},2)) FROM {Table<PayableSchemeEntity>()} WHERE {With<PayableSchemeEntity>(x => x.ALOSK)} IN (SELECT {With<AllocationEntity>(x => x.SysKey)} FROM {Table<AllocationEntity>()} WHERE {With<AllocationEntity>(x => x.RecordStatus)} = {(int)RecordStatus.Post} AND {With<AllocationEntity>(x => x.HeaderSK)} = {With<SalesInvoiceEntity>(x => x.SysKey)} AND {With<AllocationEntity>(x => x.TransType)} = {(int)TransType.AllocationCommission})) [{nameof(CommissionPayableDTO.TotalPayable)}],
                 dbo.[HelperFunction2]({With<SalesInvoiceEntity>(x => x.SysKey)}) [{nameof(CommissionPayableDTO.TotalPaid)}],
                 {With<SalesInvoiceEntity>(x => x.PayableStatus)} [{nameof(CommissionPayableDTO.SalesStatus)}]
                 FROM {Table<PropertyTransactionEntity>()}
                 INNER JOIN {Table<SalesInvoiceEntity>()} ON {With<PropertyTransactionEntity>(x => x.SysKey)} = {With<SalesInvoiceEntity>(x => x.SysKey)}
                 INNER JOIN  {Table<ContactEntity>()} ON {With<PropertyTransactionEntity>(x => x.SysKey)} = {With<ContactEntity>(x => x.SysKey)}
                 WHERE 
                 {With<SalesInvoiceEntity>(x => x.SysKey)} IN (SELECT {With<AllocationEntity>(x => x.HeaderSK)} FROM {Table<AllocationEntity>()} WHERE {With<AllocationEntity>(x => x.RecordStatus)} = {(int)RecordStatus.Post} AND {With<AllocationEntity>(x => x.TransType)} = {(int)TransType.AllocationCommission})   
                 GROUP BY 
                 {With<SalesInvoiceEntity>(x => x.ActualGST)},
                 {With<SalesInvoiceEntity>(x => x.InvoiceDate)},
                 {With<SalesInvoiceEntity>(x => x.SysKey)},
                 {With<SalesInvoiceEntity>(x => x.R1)},
                 {With<PropertyTransactionEntity>(x => x.R1)},
                 {With<SalesInvoiceEntity>(x => x.PayableStatus)},
                 {With<ContactEntity>(x => x.Address1)},
                 {With<ContactEntity>(x => x.Address2)},
                 {With<ContactEntity>(x => x.PostalCode)},
                 {With<ContactEntity>(x => x.BuildingHouseNo)},
                 {With<ContactEntity>(x => x.FloorUnit)}";

                query = $@"SELECT * FROM ({query}) [tbl] 
                        WHERE {where.ToSQL()} ORDER BY 
                        [tbl].SalesStatus,
                        [tbl].InvoiceNo DESC";

                return ExcutePagerResult<CommissionPayableDTO>(query: query, currentPage: currentPage, pageSize: pageSize);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IDataPagerResult<CommissionPayableDetailDTO> GetDetailPager(int currentPage, int pageSize, Expression<Func<CommissionPayableDetailDTO, bool>> where = null)
        {
            try
            {
                var query = $@"SELECT {With<PersonEntity>(x => x.SysKey)} [{nameof(CommissionPayableDetailDTO.SPSK)}],
                 {With<AllocationEntity>(x => x.SysKey)} [{nameof(CommissionPayableDetailDTO.ALOSK)}],
                 {With<SalesInvoiceEntity>(x => x.R1)} [{nameof(CommissionPayableDetailDTO.InvoiceNo)}],   
                 {With<SalesInvoiceEntity>(x => x.SysKey)} [{nameof(CommissionPayableDetailDTO.InvoiceSK)}],
                 {With<PersonEntity>(x => x.Name)} [{nameof(CommissionPayableDetailDTO.SPName)}],
                 {With<PersonEntity>(x => x.NRIC)} [{nameof(CommissionPayableDetailDTO.NRIC)}],
                 {With<CompanyEntity>(x => x.CompanyName)} [{nameof(CommissionPayableDetailDTO.Agency)}],
                 {With<PropertyTransactionDetailEntity>(x => x.TransType)} [{nameof(CommissionPayableDetailDTO.SalespersonType)}],
                 (SELECT ISNULL(SUM(ROUND({With<CommissionPaymentEntity>(x => x.NetPaid)},2)),0) FROM {Table<CommissionPaymentEntity>()} WHERE {With<CommissionPaymentEntity>(x => x.ALOSK)} = {With<AllocationEntity>(x => x.SysKey)} AND {With<CommissionPaymentEntity>(x => x.RecordStatus)} >= {(int)RecordStatus.Post} AND {With<AllocationEntity>(x => x.TransType)} = {(int)TransType.AllocationCommission}) [{nameof(CommissionPayableDetailDTO.PaidAmount)}],
                 SUM(ROUND({With<AllocationEntity>(x => x.AllocateAmount)},2)) [{nameof(CommissionPayableDetailDTO.AllocatedAmount)}],
                 SUM(ROUND({With<AllocationEntity>(x => x.AmountBeforeGST)},2)) [{nameof(CommissionPayableDetailDTO.GrossAllocatedAmount)}],
                 SUM(ROUND({With<AllocationEntity>(x => x.GSTValue)},2)) [{nameof(CommissionPayableDetailDTO.GSTValue)}],
                 {With<AllocationEntity>(x => x.GSTPercent)} [{nameof(CommissionPayableDetailDTO.GSTPercent)}],
                 SUM(ROUND({With<AllocationEntity>(x => x.MarketingExpense)},2)) [{nameof(CommissionPayableDetailDTO.MarketingExpense)}],    
                 (SELECT SUM(ROUND({With<PayableSchemeEntity>(x => x.AgencyComm)},2)) FROM {Table<PayableSchemeEntity>()} WHERE {With<PayableSchemeEntity>(x => x.ALOSK)} = {With<AllocationEntity>(x => x.SysKey)}) [{nameof(CommissionPayableDetailDTO.AgencyAmount)}],
                 (SELECT SUM(ROUND({With<PayableSchemeEntity>(x => x.AssociateComm)},2)) FROM {Table<PayableSchemeEntity>()} WHERE {With<PayableSchemeEntity>(x => x.ALOSK)} = {With<AllocationEntity>(x => x.SysKey)}) [{nameof(CommissionPayableDetailDTO.PayableAmount)}],
                 (CASE {With<PropertyTransactionDetailEntity>(x => x.TransType)} 
                     WHEN {(int)TransType.ExternalSalesPerson} THEN  '{TransType.ExternalSalesPerson.ToDescription()}'  
                     WHEN {(int)TransType.InternalSalesperson} THEN '{TransType.InternalSalesperson.ToDescription()}' 
                     WHEN {(int)TransType.MainSalesperson} THEN '{TransType.MainSalesperson.ToDescription()}' END ) [{nameof(CommissionPayableDetailDTO.CoBrokerType)}]
                 FROM {Table<PropertyTransactionEntity>()}
                 INNER JOIN {Table<PropertyTransactionDetailEntity>()} ON {With<PropertyTransactionEntity>(x => x.SysKey)} = {With<PropertyTransactionDetailEntity>(x => x.PTOSK)}
                 INNER JOIN {Table<PersonEntity>()} ON {With<PersonEntity>(x => x.SysKey)} = {With<PropertyTransactionDetailEntity>(x => x.SPSK)}
                 LEFT JOIN {Table<CompanyEntity>()} ON {With<PersonEntity>(x => x.VendorSK)} = {With<CompanyEntity>(x => x.SysKey)}
                 INNER JOIN {Table<SalesInvoiceEntity>()} ON  {With<PropertyTransactionEntity>(x => x.SysKey)} = {With<SalesInvoiceEntity>(x => x.SysKey)}
                 LEFT JOIN  {Table<AllocationEntity>()} ON {With<PropertyTransactionDetailEntity>(x => x.SysKey)} = {With<AllocationEntity>(x => x.DetailSK)}
                 WHERE {With<AllocationEntity>(x => x.RecordStatus)} = {(int)RecordStatus.Post}
                 AND {With<AllocationEntity>(x => x.TransType)} = {(int)TransType.AllocationCommission}
                 GROUP BY {With<PersonEntity>(x => x.SysKey)},
                 {With<SalesInvoiceEntity>(x => x.ActualAmt)},
                 {With<SalesInvoiceEntity>(x => x.R1)},
                 {With<SalesInvoiceEntity>(x => x.SysKey)},
                 {With<PersonEntity>(x => x.Name)},
                 {With<PersonEntity>(x => x.NRIC)},
                 {With<CompanyEntity>(x => x.CompanyName)},
                 {With<PropertyTransactionDetailEntity>(x => x.TransType)},
                 {With<PropertyTransactionDetailEntity>(x => x.MarketingExpense)},
                 {With<PropertyTransactionDetailEntity>(x => x.SysKey)},
                 {With<AllocationEntity>(x => x.SysKey)},
                 {With<AllocationEntity>(x => x.TransType)},
                 {With<AllocationEntity>(x => x.AllocateAmount)},
                 {With<AllocationEntity>(x => x.GSTPercent)}";

                query = $@"SELECT * FROM ({query}) [tbl] WHERE {where.ToSQL()} ORDER BY 
                        [tbl].SalespersonType DESC";

                return ExcutePagerResult<CommissionPayableDetailDTO>(query: query, currentPage: currentPage, pageSize: pageSize);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable CommissionPaymentPrinting(string key)
        {
            try
            {
                return DbExcutor.ExcuteDataTable(
                  query: $"RPT_004 <# {key} #>",
                  connection: "FC.Biz",
                  name: "ComissionPaidPrinting");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
