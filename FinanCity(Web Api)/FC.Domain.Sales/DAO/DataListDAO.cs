namespace FC.Domain.Sales.DAO
{
    using System;
    using System.Linq.Expressions;
    using FC.Core;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;
    using Ninject;
    using FC.Core.Infrastructure.Interfaces;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Entity;
  
    public class DataListDAO : DAO
    {
        #region Methods

        public IDataPagerResult<SalespersonCommissionDTO> GetSalespersonCommissionPager(int currentPage,
          int pageSize,
          Expression<Func<SalespersonCommissionDTO, bool>> where = null)
        {
            try
            {
                var query = $@"SELECT 
                    {With<PropertyTransactionEntity>(x => x.SysKey)} [{nameof(SalespersonCommissionDTO.PropertySK)}],
                    {With<PropertyTransactionEntity>(x => x.R1)} [{nameof(SalespersonCommissionDTO.TransactionNo)}],
                    {With<PropertyTransactionEntity>(x => x.TransactionDate)} [{nameof(SalespersonCommissionDTO.TransactionDate)}],
                    {With<PropertyTransactionDetailEntity>(x => x.MarketingExpense)} [{nameof(SalespersonCommissionDTO.MarketingExpense)}], 
                    {With<PropertyTransactionDetailEntity>(x => x.GrossComm)} [{nameof(SalespersonCommissionDTO.GrossComm)}],

                    CASE {With<PropertyTransactionDetailEntity>(x => x.PayableStatus)}
                    WHEN {(int)SalesViewType.Pending} 
                    THEN ROUND((ROUND({With<PropertyTransactionDetailEntity>(x => x.GrossComm)},2) / 100) * ({With<PropertyTransactionDetailEntity>(x => x.CurrentCommission)}),2)
                    WHEN {(int)SalesViewType.Close}
                    THEN (SELECT SUM(ROUND({With<PayableSchemeEntity>(x => x.AssociateComm)},2)) FROM {Table<PayableSchemeEntity>()} INNER JOIN {Table<AllocationEntity>()} ON {With<PayableSchemeEntity>(x => x.ALOSK)} = {With<AllocationEntity>(x => x.SysKey)} WHERE {With<AllocationEntity>(x => x.DetailSK)} = {With<PropertyTransactionDetailEntity>(x => x.SysKey)} AND {With<AllocationEntity>(x => x.RecordStatus)} = 8 AND {With<AllocationEntity>(x => x.TransType)} = {(int)TransType.AllocationCommission})
                    END [{nameof(SalespersonCommissionDTO.PayableAmount)}],

                    CASE {With<PropertyTransactionDetailEntity>(x => x.PayableStatus)}
                    WHEN {(int)SalesViewType.Pending} THEN ((({With<PropertyTransactionDetailEntity>(x => x.GrossComm)} - {With<PropertyTransactionDetailEntity>(x => x.MarketingExpense)}) / 100) * (100 - {With<PropertyTransactionDetailEntity>(x => x.CurrentCommission)}))
                    WHEN {(int)SalesViewType.Close} THEN (SELECT SUM(ROUND({With<PayableSchemeEntity>(x => x.AgencyComm)},2)) FROM {Table<PayableSchemeEntity>()} INNER JOIN {Table<AllocationEntity>()} ON {With<PayableSchemeEntity>(x => x.ALOSK)} = {With<AllocationEntity>(x => x.SysKey)} WHERE {With<AllocationEntity>(x => x.DetailSK)} = {With<PropertyTransactionDetailEntity>(x => x.SysKey)} AND {With<AllocationEntity>(x => x.RecordStatus)} = 8 AND {With<AllocationEntity>(x => x.TransType)} = {(int)TransType.AllocationCommission})
                    END [{nameof(SalespersonCommissionDTO.AgencyAmount)}],
                    
                    (SELECT ISNULL(SUM(ROUND({With<CommissionPaymentEntity>(x => x.NetPaid)},2)),0) FROM {Table<CommissionPaymentEntity>()} 
                    WHERE {With<CommissionPaymentEntity>(x => x.ALOSK)} IN (SELECT {With<AllocationEntity>(x => x.SysKey)} FROM {Table<AllocationEntity>()} WHERE {With<AllocationEntity>(x => x.DetailSK)} = {With<PropertyTransactionDetailEntity>(x => x.SysKey)} 
                    AND {With<AllocationEntity>(x => x.TransType)} = {(int)TransType.AllocationCommission})
                    AND {With<CommissionPaymentEntity>(x => x.RecordStatus)} >= {(int)RecordStatus.Post})
                    [{nameof(SalespersonCommissionDTO.AmountPaid)}],

                    dbo.MainClientName({With<PropertyTransactionEntity>(x => x.SysKey)}) [{nameof(SalespersonCommissionDTO.ClientName)}],
                    dbo.[AddressFrm]({With<ContactEntity>(x => x.BuildingHouseNo)},{With<ContactEntity>(x => x.Address1)},{With<ContactEntity>(x => x.Address2)},{With<ContactEntity>(x => x.FloorUnit)},{With<ContactEntity>(x => x.PostalCode)}) [{nameof(PropertyTransactionDTO.PropertyAddress)}],
                    {With<PropertyTransactionEntity>(x => x.TransactedPrice)} [{nameof(SalespersonCommissionDTO.LoanAmount)}],
                    
                    dbo.ClientName({With<PropertyTransactionEntity>(x => x.SysKey)}) [Bank],
                    {With<PropertyTransactionDetailEntity>(x => x.SPSK)} [{nameof(SalespersonCommissionDTO.SPSK)}],
                    {With<PropertyTransactionDetailEntity>(x => x.PayableStatus)} [{nameof(SalespersonCommissionDTO.Status)}]
                    FROM {Table<PropertyTransactionEntity>()}
                    INNER JOIN {Table<SalesInvoiceEntity>()} ON {With<SalesInvoiceEntity>(x => x.SysKey)} = {With<PropertyTransactionEntity>(x => x.SysKey)}
                    INNER JOIN {Table<ContactEntity>()} ON {With<ContactEntity>(x => x.SysKey)} = {With<PropertyTransactionEntity>(x => x.SysKey)}
                    INNER JOIN {Table<PropertyTransactionDetailEntity>()} ON {With<PropertyTransactionEntity>(x => x.SysKey)} = {With<PropertyTransactionDetailEntity>(x => x.PTOSK)}
                    WHERE {With<SalesInvoiceEntity>(x=>x.RecordStatus)} = {(int)RecordStatus.Post}";

                query = $@"SELECT * FROM ({query}) [tbl] 
                        WHERE {where.ToSQL()} ORDER BY 
                        [tbl].TransactionNo DESC ";

                return ExcutePagerResult<SalespersonCommissionDTO>(query, currentPage, pageSize);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IDataPagerResult<LeadAndContactDTO> GetClientAndLeadPager(int currentPage, int pageSize, Expression<Func<LeadAndContactDTO, bool>> where = null)
        {
            try
            {
                var c = $@"SELECT 
                {With<PersonEntity>(x => x.SysKey)} [{nameof(LeadAndContactDTO.SysKey)}],
                {With<PersonEntity>(x => x.R1)} [{nameof(LeadAndContactDTO.R1)}],
                {With<PersonEntity>(x => x.Name)} [{nameof(LeadAndContactDTO.Name)}],
                {With<PersonEntity>(x => x.NRIC)} [{nameof(LeadAndContactDTO.NRIC)}],
                dbo.AddressFrm({With<ContactEntity>(x => x.BuildingHouseNo)},{With<ContactEntity>(x => x.Address1)},{With<ContactEntity>(x => x.Address2)},{With<ContactEntity>(x => x.FloorUnit)},{With<ContactEntity>(x => x.PostalCode)}) [{nameof(LeadAndContactDTO.Address)}],
                {With<ContactEntity>(x => x.Email)} [{nameof(LeadAndContactDTO.Email)}],
                {With<ContactEntity>(x => x.Fax)} [{nameof(LeadAndContactDTO.Fax)}],
                {With<PersonEntity>(x => x.IsActive)} [{nameof(LeadAndContactDTO.IsActive)}],
                {With<PersonEntity>(x => x.TransType)} [{nameof(LeadAndContactDTO.TransType)}],
                {With<PersonEntity>(x => x.IsOrganization)} [{nameof(LeadAndContactDTO.IsOrganization)}],
                {With<CompanyEntity>(x => x.CompanyName)} [{nameof(LeadAndContactDTO.CompanyName)}],
                {With<CompanyEntity>(x => x.TransType)} [{nameof(LeadAndContactDTO.VendorType)}],
                {With<CompanyEntity>(x => x.SysKey)} [{nameof(LeadAndContactDTO.VendorSK)}],
                {With<ClientShareEntity>(x => x.RefSK)} [{nameof(LeadAndContactDTO.RefSK)}]
                FROM {Table<PersonEntity>()}
                INNER JOIN {Table<ClientShareEntity>()} ON {With<PersonEntity>(x => x.SysKey)} = {With<ClientShareEntity>(x => x.SysKey)}
                LEFT JOIN {Table<ContactEntity>()} ON {With<PersonEntity>(x => x.SysKey)} = {With<ContactEntity>(x => x.SysKey)}
                LEFT JOIN {Table<CompanyEntity>()} ON {With<CompanyEntity>(x => x.SysKey)} = {With<PersonEntity>(x => x.VendorSK)}";

                var query = $@"{c} 
                            WHERE {With<ClientShareEntity>(x => x.RefSK)} = '{RefSK}'
                            AND ({With<PersonEntity>(x => x.TransType)} = {(int)TransType.Client} OR {With<PersonEntity>(x => x.TransType)} = {(int)TransType.Lead})";

                query = $@"{query}
                         UNION 
                         {c}
                         LEFT JOIN {Table<LeadEntity>()} ON {With<PersonEntity>(x => x.SysKey)} = {With<LeadEntity>(x => x.SysKey)} 
                         WHERE {With<ClientShareEntity>(x => x.RefSK)} = '{RefSK}'
                         AND ({With<PersonEntity>(x => x.TransType)} = {(int)TransType.Client} OR {With<PersonEntity>(x => x.TransType)} = {(int)TransType.Lead})";


                query = $@"SELECT * FROM ({query}) [tbl] 
                        WHERE {where.ToSQL()}
                        ORDER BY [tbl].Name";

                return ExcutePagerResult<LeadAndContactDTO>(query, currentPage, pageSize);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IDataPagerResult<LeadAndSalespersonDTO> GetLeadAndSalesperson(int currentPage, int pageSize, Expression<Func<LeadAndSalespersonDTO, bool>> where = null)
        {
            try
            {
                var query = $@"SELECT 
                {With<ClientShareEntity>(x => x.SysKey)} [{nameof(LeadAndSalespersonDTO.LeadSK)}],
                {With<PersonEntity>(x => x.R1)} [{nameof(LeadAndSalespersonDTO.R1)}],
                {With<PersonEntity>(x => x.Name)} [{nameof(LeadAndSalespersonDTO.Name)}],
                {With<PersonEntity>(x => x.NRIC)} [{nameof(LeadAndSalespersonDTO.NRIC)}],
                {With<PersonEntity>(x => x.IsActive)} [{nameof(LeadAndSalespersonDTO.IsActive)}],
                {With<ClientShareEntity>(x => x.RefSK)} [{nameof(LeadAndSalespersonDTO.RefSK)}]
                FROM {Table<PersonEntity>()}
                INNER JOIN {Table<ClientShareEntity>()} ON {With<PersonEntity>(x => x.SysKey)} = {With<ClientShareEntity>(x => x.RefSK)}";

                query = $@"SELECT * FROM ({query}) [tbl] 
                WHERE {where.ToSQL()}
                ORDER BY [tbl].Name";

                return ExcutePagerResult<LeadAndSalespersonDTO>(query, currentPage, pageSize);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion

        #region Members

        public string RefSK
        {
            get
            {
                return DI.Current.Get<IFinanCityIdentity>().SPSK;
            }
        }

        #endregion
    }
}
