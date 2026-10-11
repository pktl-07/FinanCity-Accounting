namespace FC.Domain.Sales.DAO
{
    using System;
    using System.Linq.Expressions;
    using FC.Core;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Entity;

    public class AllocationDAO : DAO<AllocationEntity>
    {
        #region Methods

        public IDataPagerResult<CommissionReceivedAllocationDTO> GetPager(int currentPage, int pageSize, Expression<Func<CommissionReceivedAllocationDTO, bool>> where = null)
        {
            try
            {
                var query = $@"SELECT {With<SalesInvoiceEntity>(x => x.R1)} [{nameof(CommissionReceivedAllocationDTO.InvoiceNo)}],
                 {With<PropertyTransactionEntity>(x => x.R1)} [{nameof(CommissionReceivedAllocationDTO.TransactionNo)}],
                 {With<SalesInvoiceEntity>(x => x.SysKey)} [{nameof(CommissionReceivedAllocationDTO.InvoiceSK)}], 
                 {With<SalesInvoiceEntity>(x => x.ReceivableStatus)} [{nameof(CommissionReceivedAllocationDTO.SalesStatus)}],
                 {With<SalesInvoiceEntity>(x => x.InvoiceDate)} [{nameof(CommissionReceivedAllocationDTO.InvoiceDate)}],
                 dbo.[AddressFrm]({With<ContactEntity>(x => x.BuildingHouseNo)}, {With<ContactEntity>(x => x.Address1)},{With<ContactEntity>(x => x.Address2)}, {With<ContactEntity>(x => x.FloorUnit)},{With<ContactEntity>(x => x.PostalCode)}) [{nameof(CommissionReceivedDTO.Address)}],
                 (SELECT {With<PersonEntity>(x => x.Name)} FROM {Table<PTOClientEntity>()} INNER JOIN {Table<PersonEntity>()} ON {With<PersonEntity>(x => x.SysKey)} = {With<PTOClientEntity>(x => x.ClientSK)} WHERE {With<PTOClientEntity>(x => x.PTOSK)} = {With<SalesInvoiceEntity>(x => x.SysKey)} AND {With<PTOClientEntity>(x => x.IsMain)} = 1) [{nameof(CommissionReceivedAllocationDTO.ClientName)}],
                 (SELECT SUM(ROUND({With<CommissionReceivedEntity>(x => x.TotalAmt)},2)) FROM {Table<CommissionReceivedEntity>()} WHERE {With<CommissionReceivedEntity>(x => x.InvoiceSK)}={With<SalesInvoiceEntity>(x => x.SysKey)} AND {With<CommissionReceivedEntity>(x => x.RecordStatus)} = {(int)RecordStatus.Post}) [{nameof(CommissionReceivedAllocationDTO.TotalReceivedAmt)}],
                 (SELECT SUM(ROUND({With<AllocationEntity>(x => x.AllocateAmount)},2)) FROM {Table<AllocationEntity>()} WHERE {With<AllocationEntity>(x => x.HeaderSK)}={With<SalesInvoiceEntity>(x => x.SysKey)} AND {With<AllocationEntity>(x => x.RecordStatus)}={(int)RecordStatus.Post} AND {With<AllocationEntity>(x => x.TransType)} = {(int)TransType.AllocationCommission}) [{nameof(CommissionReceivedAllocationDTO.TotalAllocatedAmt)}]
                 FROM {Table<PropertyTransactionEntity>()}
                 INNER JOIN {Table<PropertyTransactionDetailEntity>()} ON {With<PropertyTransactionEntity>(x => x.SysKey)} = {With<PropertyTransactionDetailEntity>(x => x.PTOSK)}
                 INNER JOIN {Table<SalesInvoiceEntity>()} ON  {With<SalesInvoiceEntity>(x => x.SysKey)} = {With<PropertyTransactionDetailEntity>(x => x.PTOSK)}
                 INNER JOIN {Table<CommissionReceivedEntity>()} ON {With<SalesInvoiceEntity>(x => x.SysKey)} = {With<CommissionReceivedEntity>(x => x.InvoiceSK)}
                 INNER JOIN {Table<ContactEntity>()} ON {With<ContactEntity>(x => x.SysKey)} = {With<PropertyTransactionEntity>(x => x.SysKey)}
                 WHERE {With<CommissionReceivedEntity>(x => x.RecordStatus)} = {(int)RecordStatus.Post}
                 GROUP BY {With<SalesInvoiceEntity>(x => x.R1)},
                 {With<SalesInvoiceEntity>(x => x.SysKey)},
                 {With<SalesInvoiceEntity>(x => x.InvoiceDate)},
                 {With<SalesInvoiceEntity>(x => x.ReceivableStatus)},
                 {With<ContactEntity>(x => x.Address1)},
                 {With<ContactEntity>(x => x.Address2)},
                 {With<ContactEntity>(x => x.BuildingHouseNo)},
                 {With<ContactEntity>(x => x.FloorUnit)},
                 {With<ContactEntity>(x => x.PostalCode)},
                 {With<PropertyTransactionEntity>(x => x.R1)}";

                query = $@"SELECT * FROM ({query}) [tbl] WHERE {where.ToSQL()} ORDER BY 
                        [tbl].SalesStatus,
                        [tbl].InvoiceNo DESC";

                return ExcutePagerResult<CommissionReceivedAllocationDTO>(query, currentPage, pageSize);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public decimal GetSalesCommissoinToDate(string spsk)
        {
            try
            {
                var query = $@"SELECT ISNULL(SUM(ROUND({With<AllocationEntity>(x => x.AmountBeforeGST)},2)),0)
                    FROM {Table<AllocationEntity>()}
                    INNER JOIN {Table<PropertyTransactionEntity>()} ON {With<PropertyTransactionEntity>(x => x.SysKey)} = {With<AllocationEntity>(x => x.HeaderSK)}
                    WHERE {With<AllocationEntity>(x => x.TransType)} = {(int)TransType.AllocationCommission}
                    AND {With<AllocationEntity>(x => x.RecordStatus)} = {(int)RecordStatus.Post}
                    AND {With<AllocationEntity>(x => x.SPSK)} = '{spsk}'";

                return (decimal)DbExcutor.ExcuteScalar(query, "FC.Biz");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public decimal GetSalesCommissionToYear(string spsk, int fromDate, int toDate)
        {
            try
            {
                var query = $@"SELECT ISNULL(SUM(ROUND({With<AllocationEntity>(x => x.AmountBeforeGST)},2)),0) 
                    FROM {Table<AllocationEntity>()} 
                    INNER JOIN {Table<PropertyTransactionEntity>()} ON {With<PropertyTransactionEntity>(x => x.SysKey)} = {With<AllocationEntity>(x => x.HeaderSK)}
                    WHERE {With<AllocationEntity>(x => x.AllocationDate)} BETWEEN {fromDate} AND {toDate}
                    AND {With<AllocationEntity>(x => x.TransType)} = {(int)TransType.AllocationCommission}
                    AND {With<AllocationEntity>(x => x.SPSK)} = '{spsk}'
                    AND {With<AllocationEntity>(x => x.RecordStatus)} = {(int)RecordStatus.Post}";

                return (decimal)DbExcutor.ExcuteScalar(query, "FC.Biz");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public decimal GetSalesValueToDate(string spsk)
        {
            try
            {
                var query = $@"SELECT ISNULL(SUM(ROUND({With<PropertyTransactionEntity>(x => x.TransactedPrice)},2)),0)
                    FROM {Table<PropertyTransactionEntity>()}
                    INNER JOIN {Table<TNC_Entity>()} ON {With<TNC_Entity>(x => x.SysKey)} = {With<PropertyTransactionEntity>(x => x.SysKey)}
                    WHERE {With<PropertyTransactionEntity>(x => x.SysKey)}
                    IN (SELECT {With<AllocationEntity>(x => x.HeaderSK)} FROM {Table<AllocationEntity>()} 
                    WHERE {With<AllocationEntity>(x => x.TransType)} = {(int)TransType.AllocationCommission}
                    AND {With<AllocationEntity>(x => x.SPSK)} = '{spsk}'
                    AND {With<AllocationEntity>(x => x.RecordStatus)} = {(int)RecordStatus.Post})";

                return (decimal)DbExcutor.ExcuteScalar(query, "FC.Biz");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public decimal GetSalesValueToYear(string spsk, int fromDate, int toDate)
        {
            try
            {
                var query = $@"SELECT ISNULL(SUM(ROUND({With<PropertyTransactionEntity>(x => x.TransactedPrice)},2)),0)
                    FROM {Table<PropertyTransactionEntity>()}
                    INNER JOIN {Table<TNC_Entity>()} ON {With<TNC_Entity>(x => x.SysKey)} = {With<PropertyTransactionEntity>(x => x.SysKey)}
                    WHERE {With<PropertyTransactionEntity>(x => x.SysKey)}
                    IN (SELECT {With<AllocationEntity>(x => x.HeaderSK)} FROM {Table<AllocationEntity>()} 
                    WHERE {With<AllocationEntity>(x => x.AllocationDate)} BETWEEN {fromDate} AND {toDate}
                    AND {With<AllocationEntity>(x => x.TransType)} = {(int)TransType.AllocationCommission}
                    AND {With<AllocationEntity>(x => x.SPSK)} = '{spsk}'
                    AND {With<AllocationEntity>(x => x.RecordStatus)} = {(int)RecordStatus.Post})";

                return (decimal)DbExcutor.ExcuteScalar(query, "FC.Biz");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
       
        #endregion
    }
}
