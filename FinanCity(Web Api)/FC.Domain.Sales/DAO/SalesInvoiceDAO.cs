namespace FC.Domain.Sales.DAO
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq.Expressions;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Entity;

    public class SalesInvoiceDAO : DAO<SalesInvoiceEntity>
    {
        #region Methods

        public IDataPagerResult<InvoiceDTO> GetPager(int currentPage, int pageSize, Expression<Func<InvoiceDTO, bool>> filter = null)
        {
            try
            {
                var query = $@"SELECT {With<SalesInvoiceEntity>(x => x.R1)} {nameof(InvoiceDTO.InvoiceNo)},
                    {With<SalesInvoiceEntity>(x => x.SysKey)} [{nameof(InvoiceDTO.InvoiceSK)}],
                    {With<SalesInvoiceEntity>(x => x.CURRATE)} [{nameof(InvoiceDTO.CURRATE)}],
                    {With<SalesInvoiceEntity>(x => x.CURROP)} [{nameof(InvoiceDTO.CURROP)}],
                    {With<SalesInvoiceEntity>(x => x.CURRSK)} [{nameof(InvoiceDTO.CURRSK)}],
                    {With<CurrencyEntity>(x => x.Currency_AlphaCode)} [{nameof(InvoiceDTO.CurrencyCode)}],
                    {With<CompanyEntity>(x => x.CompanyName)} [{nameof(InvoiceDTO.MortgageBank)}],
                    (SELECT {With<PersonEntity>(x => x.Name)} FROM {Table<PTOClientEntity>()}
                        INNER JOIN {Table<PersonEntity>()} ON {With<PTOClientEntity>(x => x.ClientSK)} = {With<PersonEntity>(x => x.SysKey)}
                        WHERE {With<PTOClientEntity>(x => x.PTOSK)} = {With<SalesInvoiceEntity>(x => x.SysKey)}
                        AND {With<PTOClientEntity>(x => x.IsMain)} = 1
                    ) [{nameof(InvoiceDTO.ClientName)}],
                    (SELECT {With<PersonEntity>(x => x.Name)} FROM {Table<PropertyTransactionDetailEntity>()}
                        INNER JOIN {Table<PersonEntity>()} ON {With<PropertyTransactionDetailEntity>(x => x.SPSK)} = {With<PersonEntity>(x => x.SysKey)}
                        WHERE {With<PropertyTransactionDetailEntity>(x => x.PTOSK)} = {With<SalesInvoiceEntity>(x => x.SysKey)}
                        AND {With<PropertyTransactionDetailEntity>(x => x.IsMain)} = 1
                    ) [{nameof(InvoiceDTO.SalespersonName)}],
                    {With<PropertyTransactionEntity>(x => x.R1)} [{nameof(InvoiceDTO.RefNo)}],
                    {With<SalesInvoiceEntity>(x => x.JRef)} [{nameof(InvoiceDTO.JRef)}],
                    {With<SalesInvoiceEntity>(x => x.InvoiceDate)} [{nameof(InvoiceDTO.InvoiceDate)}],
                    dbo.[AddressFrm]({With<ContactEntity>(x => x.BuildingHouseNo)},{With<ContactEntity>(x => x.Address1)}, {With<ContactEntity>(x => x.Address2)}, {With<ContactEntity>(x => x.FloorUnit)}, {With<ContactEntity>(x => x.PostalCode)}) [{nameof(InvoiceDTO.Address)}], 
                    {With<SalesInvoiceEntity>(x => x.RecordStatus)} [{nameof(InvoiceDTO.RecordStatus)}],
                    {With<SalesInvoiceEntity>(x => x.ActualAmt)} [{nameof(InvoiceDTO.InvoiceAmt)}]
                    FROM {Table<PropertyTransactionEntity>()}
                    INNER JOIN {Table<SalesInvoiceEntity>()} ON {With<PropertyTransactionEntity>(x => x.SysKey)} = {With<SalesInvoiceEntity>(x => x.SysKey)}
                    LEFT JOIN {Table<PersonEntity>()} ON {With<PersonEntity>(x => x.SysKey)} = {With<PropertyTransactionEntity>(x => x.BankerSK)}
                    LEFT JOIN {Table<CompanyEntity>()} ON {With<CompanyEntity>(x => x.SysKey)} = {With<PersonEntity>(x => x.VendorSK)}
                    INNER JOIN {Table<ContactEntity>()} ON {With<PropertyTransactionEntity>(x => x.SysKey)} = {With<ContactEntity>(x => x.SysKey)}
                    INNER JOIN {Table<CurrencyEntity>()} ON {With<CurrencyEntity>(x => x.SysKey)} = {With<SalesInvoiceEntity>(x => x.CURRSK)}";

                query += $@" UNION 

                    SELECT {With<SalesInvoiceLogEntity>(x => x.R1)} {nameof(InvoiceDTO.InvoiceNo)},
                    {With<SalesInvoiceLogEntity>(x => x.SysKey)} [{nameof(InvoiceDTO.InvoiceSK)}],
                    {With<SalesInvoiceLogEntity>(x => x.CURRATE)} [{nameof(InvoiceDTO.CURRATE)}],
                    {With<SalesInvoiceLogEntity>(x => x.CURROP)} [{nameof(InvoiceDTO.CURROP)}],
                    {With<SalesInvoiceLogEntity>(x => x.CURRSK)} [{nameof(InvoiceDTO.CURRSK)}],
                    {With<CurrencyEntity>(x => x.Currency_AlphaCode)} [{nameof(InvoiceDTO.CurrencyCode)}],
                    {With<CompanyEntity>(x => x.CompanyName)} [{nameof(InvoiceDTO.MortgageBank)}],
                    (SELECT {With<PersonEntity>(x => x.Name)} FROM {Table<PTOClientEntity>()}
                        INNER JOIN {Table<PersonEntity>()} ON {With<PTOClientEntity>(x => x.ClientSK)} = {With<PersonEntity>(x => x.SysKey)}
                        WHERE {With<PTOClientEntity>(x => x.PTOSK)} = {With<PropertyTransactionEntity>(x => x.SysKey)}
                        AND {With<PTOClientEntity>(x => x.IsMain)} = 1
                    ) [{nameof(InvoiceDTO.ClientName)}],
                    (SELECT {With<PersonEntity>(x => x.Name)} FROM {Table<PropertyTransactionDetailEntity>()}
                        INNER JOIN {Table<PersonEntity>()} ON {With<PropertyTransactionDetailEntity>(x => x.SPSK)} = {With<PersonEntity>(x => x.SysKey)}
                        WHERE {With<PropertyTransactionDetailEntity>(x => x.PTOSK)} = {With<PropertyTransactionEntity>(x => x.SysKey)}
                        AND {With<PropertyTransactionDetailEntity>(x => x.IsMain)} = 1
                    ) [{nameof(InvoiceDTO.SalespersonName)}],
                    {With<PropertyTransactionEntity>(x => x.R1)} [{nameof(InvoiceDTO.RefNo)}],
                    {With<SalesInvoiceLogEntity>(x => x.JRef)} [{nameof(InvoiceDTO.JRef)}],
                    {With<SalesInvoiceLogEntity>(x => x.InvoiceDate)} [{nameof(InvoiceDTO.InvoiceDate)}],
                    dbo.[AddressFrm]({With<ContactEntity>(x => x.BuildingHouseNo)},{With<ContactEntity>(x => x.Address1)}, {With<ContactEntity>(x => x.Address2)}, {With<ContactEntity>(x => x.FloorUnit)}, {With<ContactEntity>(x => x.PostalCode)}) [{nameof(InvoiceDTO.Address)}], 
                    {With<SalesInvoiceLogEntity>(x => x.RecordStatus)} [{nameof(InvoiceDTO.RecordStatus)}],
                    {With<SalesInvoiceLogEntity>(x => x.ActualAmt)} [{nameof(InvoiceDTO.InvoiceAmt)}]
                    FROM {Table<PropertyTransactionEntity>()}
                    INNER JOIN {Table<SalesInvoiceLogEntity>()} ON {With<PropertyTransactionEntity>(x => x.SysKey)} = {With<SalesInvoiceLogEntity>(x => x.PTOSK)}
                    LEFT JOIN {Table<PersonEntity>()} ON {With<PersonEntity>(x => x.SysKey)} = {With<PropertyTransactionEntity>(x => x.BankerSK)}
                    LEFT JOIN {Table<CompanyEntity>()} ON {With<CompanyEntity>(x => x.SysKey)} = {With<PersonEntity>(x => x.VendorSK)}
                    INNER JOIN {Table<ContactEntity>()} ON {With<PropertyTransactionEntity>(x => x.SysKey)} = {With<ContactEntity>(x => x.SysKey)}
                    INNER JOIN {Table<CurrencyEntity>()} ON {With<CurrencyEntity>(x => x.SysKey)} = {With<SalesInvoiceLogEntity>(x => x.CURRSK)}";

                query = $@"SELECT * FROM ({query}) [tbl] WHERE {filter.ToSQL()} ORDER BY 
                            [tbl].RecordStatus, 
                            [tbl].InvoiceNo DESC";

                return ExcutePagerResult<InvoiceDTO>(query, currentPage, pageSize);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable InvoiceTemplateDataTable(string invoicesk)
        {
            try
            {
                return DbExcutor.ExcuteDataTable(
                   query: $"RPT_001_1 <# {invoicesk} #>",
                   connection: "FC.Biz",
                   name: "InvoicePrinting_1");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public override IEnumerable<SalesInvoiceEntity> Finds(Expression<Func<SalesInvoiceEntity, bool>> filter)
        {
            try
            {
                var query = $@"SELECT 
                {With<SalesInvoiceEntity>(x => x.SysKey)},
                {With<SalesInvoiceEntity>(x => x.R1)},
                {With<SalesInvoiceEntity>(x => x.Prefix)},
                {With<SalesInvoiceEntity>(x => x.Postfix)},
                {With<SalesInvoiceEntity>(x => x.VoucherId)},
                {With<SalesInvoiceEntity>(x => x.RecordStatus)},
                {With<SalesInvoiceEntity>(x => x.InvoiceDate)},
                {With<SalesInvoiceEntity>(x => x.Remark)},
                {With<SalesInvoiceEntity>(x => x.CompletionDate)},
                {With<SalesInvoiceEntity>(x => x.ReceivableStatus)},
                {With<SalesInvoiceEntity>(x => x.PayableStatus)},
                {With<SalesInvoiceEntity>(x => x.MailingType)},
                {With<SalesInvoiceEntity>(x => x.ActualAmt)},
                {With<SalesInvoiceEntity>(x => x.ActualGST)},
                {With<SalesInvoiceEntity>(x => x.TAXSK)},
                {With<SalesInvoiceEntity>(x => x.PaymentTermSK)},
                {With<SalesInvoiceEntity>(x => x.CURRSK)},
                {With<SalesInvoiceEntity>(x => x.CURRATE)},
                {With<SalesInvoiceEntity>(x => x.CURROP)},
                {With<SalesInvoiceEntity>(x => x.JRef)},
                {With<SalesInvoiceEntity>(x => x.SecondRef)}
                FROM {Table<SalesInvoiceEntity>()}
                UNION    
                SELECT 
                {With<SalesInvoiceLogEntity>(x => x.SysKey)},
                {With<SalesInvoiceLogEntity>(x => x.R1)},
                {With<SalesInvoiceLogEntity>(x => x.Prefix)},
                {With<SalesInvoiceLogEntity>(x => x.Postfix)},
                {With<SalesInvoiceLogEntity>(x => x.VoucherId)},
                {With<SalesInvoiceLogEntity>(x => x.RecordStatus)},
                {With<SalesInvoiceLogEntity>(x => x.InvoiceDate)},
                {With<SalesInvoiceLogEntity>(x => x.Remark)},
                {With<SalesInvoiceLogEntity>(x => x.CompletionDate)},
                {With<SalesInvoiceLogEntity>(x => x.ReceivableStatus)},
                {With<SalesInvoiceLogEntity>(x => x.PayableStatus)},
                {With<SalesInvoiceLogEntity>(x => x.MailingType)},
                {With<SalesInvoiceLogEntity>(x => x.ActualAmt)},
                {With<SalesInvoiceLogEntity>(x => x.ActualGST)},
                {With<SalesInvoiceLogEntity>(x => x.TAXSK)},
                {With<SalesInvoiceLogEntity>(x => x.PaymentTermSK)},
                {With<SalesInvoiceLogEntity>(x => x.CURRSK)},
                {With<SalesInvoiceLogEntity>(x => x.CURRATE)},
                {With<SalesInvoiceLogEntity>(x => x.CURROP)},
                {With<SalesInvoiceLogEntity>(x => x.JRef)},
                {With<SalesInvoiceLogEntity>(x => x.SecondRef)}    
                FROM {Table<SalesInvoiceLogEntity>()}";

                var f = filter.ToSQL().Replace("[AMS102]", "[tbl]");
                query = $@"SELECT * FROM ({query}) [tbl] WHERE {f}";

                return DbExcutor<SalesInvoiceEntity>.ExcuteReader(query);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public int LastVoucherId(string prefix, string postfix)
        {
            try
            {
                var query = $@"
                    SELECT ISNULL(MAX(VoucherId),0) FROM (
                    SELECT {With<SalesInvoiceEntity>(x => x.VoucherId)}
                        FROM {Table<SalesInvoiceEntity>()}
                        WHERE {With<SalesInvoiceEntity>(x => x.Prefix)} = <# {prefix} #>
                        AND {With<SalesInvoiceEntity>(x => x.Postfix)} = <# {postfix} #>
                    UNION ALL 
                    SELECT {With<SalesInvoiceLogEntity>(x => x.VoucherId)}
                        FROM {Table<SalesInvoiceLogEntity>()}
                        WHERE {With<SalesInvoiceLogEntity>(x => x.Prefix)} = <# {prefix} #>
                        AND {With<SalesInvoiceLogEntity>(x => x.Postfix)} = <# {postfix} #>) [tbl]";

                return (int)DbExcutor.ExcuteScalar(query, "FC.Biz");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
