namespace FC.Domain.Sales.DAO
{
    using System;
    using System.Linq.Expressions;
    using FC.Core;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;
    using FC.Core.Infrastructure.Interfaces;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Entity;
    using Ninject;

    public class PropertyTransactionDAO : DAO<PropertyTransactionEntity>
    {
        private IFinanCityIdentity _identity
        {
            get { return DI.Current.Get<IFinanCityIdentity>(); }
        }

        #region Methods

        public IDataPagerResult<PropertyTransactionDTO> GetPager(int currentPage, int pageSize, Expression<Func<PropertyTransactionDTO, bool>> where = null)
        {
            try
            {
                var query = $@"SELECT 
                {With<PropertyTransactionEntity>(x => x.CURRATE)} [{nameof(PropertyTransactionDTO.CURRATE)}],
                {With<PropertyTransactionEntity>(x => x.CURROP)} [{nameof(PropertyTransactionDTO.CURROP)}],
                {With<PropertyTransactionEntity>(x => x.CURRSK)} [{nameof(PropertyTransactionDTO.CURRSK)}],
                {With<PropertyTransactionEntity>(x => x.R1)} [{nameof(PropertyTransactionDTO.SalesNo)}],
                {With<PropertyTransactionEntity>(x => x.SysKey)} [{nameof(PropertyTransactionDTO.PropertySK)}],
                {With<SalesInvoiceEntity>(x => x.R1)} [{nameof(PropertyTransactionDTO.InvoiceNo)}],
                {With<SalesInvoiceEntity>(x => x.RecordStatus)} [{nameof(PropertyTransactionDTO.InvoiceStatus)}],
                (SELECT ISNULL(SUM(ROUND({With<MarketingExpenseEntity>(x => x.Amount)},2)),0) FROM {Table<MarketingExpenseEntity>()} WHERE {With<MarketingExpenseEntity>(x => x.PTOSK)} = {With<PropertyTransactionEntity>(x => x.SysKey)} ) [{nameof(PropertyTransactionDTO.MarketingPromotion)}],
                {With<PropertyTransactionEntity>(x => x.TransactionDate)} [{nameof(PropertyTransactionDTO.TransactionDate)}],
                {With<PropertyTransactionEntity>(x => x.ExclusiveListing)} [{nameof(PropertyTransactionDTO.Exclusive)}],
                dbo.[AddressFrm]({With<ContactEntity>(x => x.BuildingHouseNo)},{With<ContactEntity>(x => x.Address1)},{With<ContactEntity>(x => x.Address2)},{With<ContactEntity>(x => x.FloorUnit)},{With<ContactEntity>(x => x.PostalCode)}) [{nameof(PropertyTransactionDTO.PropertyAddress)}],
                (SELECT {With<PersonEntity>(x => x.Name)} FROM {Table<PersonEntity>()} INNER JOIN {Table<PropertyTransactionDetailEntity>()} ON {With<PersonEntity>(x => x.SysKey)}={With<PropertyTransactionDetailEntity>(x => x.SPSK)} WHERE {With<PropertyTransactionDetailEntity>(x => x.PTOSK)}={With<PropertyTransactionEntity>(x => x.SysKey)} AND {With<PropertyTransactionDetailEntity>(x => x.IsMain)}=1) [{nameof(PropertyTransactionDTO.SalespersonName)}],
                (SELECT {With<PersonEntity>(x => x.Name)} FROM {Table<PersonEntity>()} INNER JOIN {Table<PTOClientEntity>()} ON {With<PersonEntity>(x => x.SysKey)}={With<PTOClientEntity>(x => x.ClientSK)} WHERE {With<PTOClientEntity>(x => x.PTOSK)}={With<PropertyTransactionEntity>(x => x.SysKey)} AND {With<PTOClientEntity>(x => x.IsMain)}=1) [{nameof(PropertyTransactionDTO.ClientName)}],
                {With<PropertyTransactionEntity>(x => x.TransactedPrice)} [{nameof(PropertyTransactionDTO.CommissionAmount)}],
                {With<CurrencyEntity>(x => x.Currency_AlphaCode)} [{nameof(PropertyTransactionDTO.CurrencyCode)}],
                {With<PropertyTransactionEntity>(x => x.RecordStatus)} [{nameof(PropertyTransactionDTO.RecordStatus)}],
                {With<PropertyTransactionEntity>(x => x.Auto)} [{nameof(PropertyTransactionDTO.AUTO)}],
                {With<PropertyTransactionEntity>(x => x.TransType)} [{nameof(PropertyTransactionDTO.TransType)}],
                {With<CompanyEntity>(x => x.CompanyName)} [{nameof(PropertyTransactionDTO.MortgageBank)}],
                (SELECT {With<PTOClientEntity>(x => x.ClientSK)} FROM {Table<PTOClientEntity>()} WHERE {With<PTOClientEntity>(x => x.PTOSK)} = {With<PropertyTransactionEntity>(x => x.SysKey)})  [{nameof(PropertyTransactionDTO.ClientSK)}],
                (SELECT {With<TNC_Entity>(x => x.LoanAmount)} FROM {Table<TNC_Entity>()} WHERE {With<TNC_Entity>(x => x.SysKey)} = {With<PropertyTransactionEntity>(x => x.SysKey)})  [{nameof(PropertyTransactionDTO.LoanAmount)}],
                (SELECT ({With<PropertyTransactionDetailEntity>(x => x.SPSK)}) FROM {Table<PropertyTransactionDetailEntity>()} WHERE {With<PropertyTransactionDetailEntity>(x => x.PTOSK)} = {With<PropertyTransactionEntity>(x => x.SysKey)} AND {With<PropertyTransactionDetailEntity>(x => x.TransType)} = {(int)TransType.MainSalesperson}) [{nameof(PropertyTransactionDTO.MainSalespersonSK)}]
                FROM {Table<PropertyTransactionEntity>()}
                LEFT JOIN {Table<SalesInvoiceEntity>()} ON {With<SalesInvoiceEntity>(x => x.SysKey)} = {With<PropertyTransactionEntity>(x => x.SysKey)}
                LEFT JOIN {Table<PersonEntity>()} ON {With<PersonEntity>(x => x.SysKey)} = {With<PropertyTransactionEntity>(x => x.BankerSK)}
                LEFT JOIN {Table<CompanyEntity>()} ON {With<CompanyEntity>(x => x.SysKey)} = {With<PersonEntity>(x => x.VendorSK)}
                INNER JOIN {Table<ContactEntity>()} ON {With<ContactEntity>(x => x.SysKey)} = {With<PropertyTransactionEntity>(x => x.SysKey)}
                INNER JOIN {Table<CurrencyEntity>()} ON {With<CurrencyEntity>(x => x.SysKey)} = {With<PropertyTransactionEntity>(x => x.CURRSK)}";

                if (_identity.AccountType == (int)UserAccountType.SalespersonAccount)
                {
                    query += $@"WHERE {With<PropertyTransactionEntity>(x => x.SysKey)} IN (
                        SELECT {With<PropertyTransactionDetailEntity>(x => x.PTOSK)} 
                        FROM {Table<PropertyTransactionDetailEntity>()}
                        WHERE {With<PropertyTransactionDetailEntity>(x => x.SPSK)} = '{_identity.SPSK}'
                    )";
                }

                query = $@"SELECT * FROM ({query}) [tbl] WHERE {where.ToSQL()} ORDER BY 
                      [tbl].RecordStatus, 
                      [tbl].SalesNo DESC";

                return ExcutePagerResult<PropertyTransactionDTO>(query,
                    currentPage, pageSize);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string RunMigratingKeyChangeProcess(PropertyTransactionEntity property)
        {
            try
            {
                var newKey = GeneralUtility.Key;

                DbExcutor.ExcuteNonQuery($@"
                 UPDATE {Table<PropertyTransactionEntity>()} SET {With<PropertyTransactionEntity>(x => x.SysKey)} = '{newKey}' WHERE {With<PropertyTransactionEntity>(x => x.SysKey)} = '{property.SysKey}'    
                 UPDATE {Table<TNC_Entity>()} SET {With<TNC_Entity>(x => x.SysKey)} = '{newKey}' WHERE {With<TNC_Entity>(x => x.SysKey)} = '{property.SysKey}'
                 UPDATE {Table<MarketingExpenseEntity>()} SET {With<MarketingExpenseEntity>(x => x.PTOSK)} = '{newKey}' WHERE {With<MarketingExpenseEntity>(x => x.PTOSK)} = '{property.SysKey}'
                 UPDATE {Table<ContactEntity>()} SET {With<ContactEntity>(x => x.SysKey)} = '{newKey}' WHERE {With<ContactEntity>(x => x.SysKey)} = '{property.SysKey}'
                 UPDATE {Table<PTOClientEntity>()} SET {With<PTOClientEntity>(x => x.PTOSK)} = '{newKey}' WHERE {With<PTOClientEntity>(x => x.PTOSK)} = '{property.SysKey}'                      
                 UPDATE {Table<PropertyTransactionDetailEntity>()} SET {With<PropertyTransactionDetailEntity>(x => x.PTOSK)} = '{newKey}' WHERE {With<PropertyTransactionDetailEntity>(x => x.PTOSK)} = '{property.SysKey}'   
                 UPDATE {Table<OverridingProfileEntity>()} SET {With<OverridingProfileEntity>(x => x.PTOSK)} = '{newKey}' WHERE {With<OverridingProfileEntity>(x => x.PTOSK)} = '{property.SysKey}'
                 UPDATE {Table<SalesInvoiceEntity>()} SET {With<SalesInvoiceEntity>(x => x.SysKey)} = '{newKey}' WHERE {With<SalesInvoiceEntity>(x => x.SysKey)} = '{property.SysKey}'
                 UPDATE {Table<NotificationEntity>()} SET {With<NotificationEntity>(x => x.RefSK)} = '{newKey}' WHERE {With<NotificationEntity>(x => x.RefSK)} = '{property.SysKey}'
                 UPDATE {Table<CurrencyTransactionEntity>()} SET {With<CurrencyTransactionEntity>(x => x.ParentSK)} = '{newKey}' WHERE {With<CurrencyTransactionEntity>(x => x.ParentSK)} = '{property.SysKey}'
                 UPDATE {Table<SalesInvoiceLogEntity>()} SET {With<SalesInvoiceLogEntity>(x => x.PTOSK)} = '{newKey}' WHERE {With<SalesInvoiceLogEntity>(x => x.PTOSK)} = '{property.SysKey}'
                ", CommonConstant.SalesConnection);

                return newKey;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
