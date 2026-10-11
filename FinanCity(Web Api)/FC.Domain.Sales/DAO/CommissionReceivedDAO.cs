namespace FC.Domain.Sales.DAO
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq.Expressions;
    using FC.Core;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Entity;

    public class CommissionReceivedDAO : DAO<CommissionReceivedEntity>
    {
        #region Methods

        public IDataPagerResult<CommissionReceivedDTO> GetPager(int currentPage, int pageSize, Expression<Func<CommissionReceivedDTO, bool>> where = null)
        {
            try
            {
                return ExcutePagerResult<CommissionReceivedDTO>(GetCommissionReceivedDTOQuery(where: where), currentPage, pageSize);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IEnumerable<CommissionReceivedDTO> GetCommissionReceivedDTO(Expression<Func<CommissionReceivedDTO, bool>> where = null)
        {
            try
            {
                return DbExcutor<CommissionReceivedDTO>.ExcuteReader(GetCommissionReceivedDTOQuery(where: where));
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable CommissionReceivedPrinting(string key)
        {
            try
            {
                return DbExcutor.ExcuteDataTable(
                   query: $"RPT_003 <# {key} #>",
                   connection: "FC.Biz",
                   name: "CommReceivablePrinting");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion

        #region Utilities

        private string GetCommissionReceivedDTOQuery(Expression<Func<CommissionReceivedDTO, bool>> where = null)
        {
            var query = $@"SELECT 
                    {With<SalesInvoiceEntity>(x => x.SysKey)} [{nameof(CommissionReceivedDTO.InvoiceSK)}],
                    {With<SalesInvoiceEntity>(x => x.ActualGST)} [{nameof(CommissionReceivedDTO.InvoiceGST)}],
                    {With<SalesInvoiceEntity>(x => x.R1)} [{nameof(CommissionReceivedDTO.InvoiceNo)}],
                    {With<SalesInvoiceEntity>(x => x.InvoiceDate)} [{nameof(CommissionReceivedDTO.InvoiceDate)}],
                    {With<PropertyTransactionEntity>(x => x.R1)} [{nameof(CommissionReceivedDTO.TransactionNo)}],
                    dbo.[AddressFrm]({With<ContactEntity>(x => x.BuildingHouseNo)},{With<ContactEntity>(x => x.Address1)},{With<ContactEntity>(x => x.Address2)}, {With<ContactEntity>(x => x.FloorUnit)}, {With<ContactEntity>(x => x.PostalCode)}) [{nameof(CommissionReceivedDTO.Address)}],
                    {With<SalesInvoiceEntity>(x => x.ActualAmt)} [{nameof(CommissionReceivedDTO.InvoiceAmt)}],
                    {With<PropertyTransactionEntity>(x => x.SecondRef)} [{nameof(CommissionReceivedDTO.SecondRef)}],    
                    (SELECT {With<PersonEntity>(x => x.Name)} FROM {Table<PTOClientEntity>()} INNER JOIN {Table<PersonEntity>()} ON {With<PersonEntity>(x => x.SysKey)} = {With<PTOClientEntity>(x => x.ClientSK)} WHERE {With<PTOClientEntity>(x => x.PTOSK)}={With<PropertyTransactionEntity>(x => x.SysKey)} AND {With<PTOClientEntity>(x => x.IsMain)} = 1) [{nameof(CommissionReceivedDTO.MainClient)}],
                    (SELECT {With<PersonEntity>(x => x.Name)} FROM {Table<PropertyTransactionDetailEntity>()} INNER JOIN {Table<PersonEntity>()} ON {With<PersonEntity>(x => x.SysKey)} = {With<PropertyTransactionDetailEntity>(x => x.SPSK)} WHERE {With<PropertyTransactionDetailEntity>(x => x.PTOSK)}={With<PropertyTransactionEntity>(x => x.SysKey)} AND {With<PropertyTransactionDetailEntity>(x => x.IsMain)} = 1) [{nameof(CommissionReceivedDTO.MainSalesperson)}],
                    (SELECT ISNULL(SUM(ROUND({With<CommissionReceivedEntity>(x => x.TotalAmt)}, 2)), 0) FROM {Table<CommissionReceivedEntity>()} WHERE {With<CommissionReceivedEntity>(x => x.InvoiceSK)} = {With<SalesInvoiceEntity>(x => x.SysKey)} AND {With<CommissionReceivedEntity>(x => x.RecordStatus)} = {(int)RecordStatus.Post}) [{nameof(CommissionReceivedDTO.TotalReceived)}],
                    {With<SalesInvoiceEntity>(x => x.ReceivableStatus)} [{nameof(CommissionReceivedDTO.SalesStatus)}]                    
              
                FROM {Table<PropertyTransactionEntity>()}
                INNER JOIN {Table<ContactEntity>()} ON {With<PropertyTransactionEntity>(x => x.SysKey)}={With<ContactEntity>(x => x.SysKey)}
                INNER JOIN {Table<SalesInvoiceEntity>()} ON {With<SalesInvoiceEntity>(x => x.SysKey)}={With<PropertyTransactionEntity>(x => x.SysKey)}
                WHERE {With<SalesInvoiceEntity>(x => x.RecordStatus)} = {(int)RecordStatus.Post}";

            query = $@"SELECT * FROM ({query}) [tbl] WHERE {where.ToSQL()} ORDER BY 
                        [tbl].SalesStatus,
                        [tbl].InvoiceNo DESC";

            return query;
        }

        #endregion
    }  
}
