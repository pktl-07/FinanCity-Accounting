namespace FC.Domain.Version.DAO
{
    using System.Linq;
    using FC.Core;
    using FC.Core.Data;
    using FC.Domain.Version.Entity;
    using FC.Domain.Version.Infrastructure;
    using FC.Domain.Accounts.Business;
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.Entity;

    [ESSVersion(Name = "1.0.0.9", Description = @"")]
    public class V_1_0_0_9_DAO : DAO<VersionEntity>, IEssVersion
    {
        #region Fields
        
        BatchControl _period;
        SalesInvoice _salesInvoice;
        CommissionReceived _received;
        CommissionAllocation _allocation;
        AccountingPeriod _accountingPeriod;
        BatchControl _batchControl;
        OverridingProfile _overrideProfile;
        IEssAppRepository _repository;

        #endregion

        #region Ctor

        public V_1_0_0_9_DAO(BatchControl period,
            SalesInvoice salesInvoice,
            CommissionReceived received,
            CommissionAllocation allocation,
            AccountingPeriod accountingPeriod,
            OverridingProfile overrideProfile,
            IEssAppRepository repository,
            BatchControl batchControl) 
        {
            _repository = repository;
            _period = period;
            _allocation = allocation;
            _overrideProfile = overrideProfile;
            _salesInvoice = salesInvoice;
            _received = received;
            _accountingPeriod = accountingPeriod;
            _batchControl = batchControl;
        }

        #endregion

        #region Methods

        public void Excute()
        {
            _period.Finds().Each(period =>
            {
                DbExcutor.ExcuteNonQuery($@"
                ALTER TABLE {period.BatchTableName} ALTER COLUMN CreatedTime varchar(10)", CommonConstant.AccountConnection);

                DbExcutor.ExcuteNonQuery($@"
                ALTER TABLE {period.BatchTableName} ALTER COLUMN ModifiedTime varchar(10)", CommonConstant.AccountConnection);
            });

            var tables = new string[] {
                "AMS100",
                "AMS101",
                "AMS102",
                "AMS102",
                "AMS103",
                "AMS104",
                "AMS105",
                "AMS106",
                "AMS107",
                "AMS108",
                "AMS109",
                "AMS110",
                "AMS111",
                "BankBestRate",
                "Building",
                "CareerManagement",
                "CommissionStructure",
                "CommissionStructureBase",
                "CommissionStructureDetail",
                "CommissionType",
                "Company",
                "CompanyGST",
                "Contact",
                "Country",
                "Currency",
                "CurrencyRate",
                "EMP_History",
                "Employment",
                "IDIndexing",
                "IndustryExam",
                "JUN001",
                "JUN002",
                "JUN003",
                "JUN004",
                "JUN005",
                "JUN006",
                "JUN008",
                "JUN009",
                "JUN010",
                "JUN011",
                "JUN012",
                "JUN013",
                "JUN014",
                "Lead",
                "Logger",
                "OtherPaymentDescription",
                "PaymentTerm",
                "PayNow",
                "PermissionSetting",
                "Person",
                "PersonEducation",
                "PersonIndustryExam",
                "PersonShare",
                "PostalCode",
                "PromotionItem",
                "Qualification",
                "SalesOrderType",
                "Streets",
                "TNC",
                "TNC_Duration",
                "TNC_InterestRateType",
                "TNC_PropertyPurpose",
                "TNC_PropertyType",
                "TNC_RateType",
                "TNC_TimeFrame",
                "TNC_TransactionType",
                "Upload",
                "Version",
            };

            tables.Each(table =>
            {
                DbExcutor.ExcuteNonQuery($@"
                ALTER TABLE {table} ALTER COLUMN CreatedTime varchar(10)", CommonConstant.SalesConnection);

                DbExcutor.ExcuteNonQuery($@"
                ALTER TABLE {table} ALTER COLUMN ModifiedTime varchar(10)", CommonConstant.SalesConnection);
            });

            if (_repository.DomainUrl == "https://financityapi.peas.sg")
            {
                DbExcutor.ExcuteNonQuery(@"
                UPDATE B0000009 SET CreatedTime = '170727' WHERE R1 = 'GL-000026'", CommonConstant.AccountConnection);

                var invoiceNo = "INV-100007";
                var oldKey = _salesInvoice.Finds(filter: f => f.R1 == invoiceNo).Single().SysKey;
                var newKey = GeneralUtility.Key;

                DbExcutor.ExcuteNonQuery($@"
                 UPDATE {Table<PropertyTransactionEntity>()} SET {With<PropertyTransactionEntity>(x => x.SysKey)} = '{newKey}' WHERE {With<PropertyTransactionEntity>(x => x.SysKey)} = '{oldKey}'    
                 UPDATE {Table<TNC_Entity>()} SET {With<TNC_Entity>(x => x.SysKey)} = '{newKey}' WHERE {With<TNC_Entity>(x => x.SysKey)} = '{oldKey}'
                 UPDATE {Table<MarketingExpenseEntity>()} SET {With<MarketingExpenseEntity>(x => x.PTOSK)} = '{newKey}' WHERE {With<MarketingExpenseEntity>(x => x.PTOSK)} = '{oldKey}'
                 UPDATE {Table<ContactEntity>()} SET {With<ContactEntity>(x => x.SysKey)} = '{newKey}' WHERE {With<ContactEntity>(x => x.SysKey)} = '{oldKey}'
                 UPDATE {Table<PTOClientEntity>()} SET {With<PTOClientEntity>(x => x.PTOSK)} = '{newKey}' WHERE {With<PTOClientEntity>(x => x.PTOSK)} = '{oldKey}'                      
                 UPDATE {Table<PropertyTransactionDetailEntity>()} SET {With<PropertyTransactionDetailEntity>(x => x.PTOSK)} = '{newKey}' WHERE {With<PropertyTransactionDetailEntity>(x => x.PTOSK)} = '{oldKey}'   
                 UPDATE {Table<SalesInvoiceEntity>()} SET {With<SalesInvoiceEntity>(x => x.SysKey)} = '{newKey}' WHERE {With<SalesInvoiceEntity>(x => x.SysKey)} = '{oldKey}'
                 UPDATE {Table<NotificationEntity>()} SET {With<NotificationEntity>(x => x.RefSK)} = '{newKey}' WHERE {With<NotificationEntity>(x => x.RefSK)} = '{oldKey}'
                 UPDATE {Table<CurrencyTransactionEntity>()} SET {With<CurrencyTransactionEntity>(x => x.ParentSK)} = '{newKey}' WHERE {With<CurrencyTransactionEntity>(x => x.ParentSK)} = '{oldKey}'
                ", CommonConstant.SalesConnection);

                var profilesk = _overrideProfile.Finds(filter: f => f.PTOSK == oldKey).Last().SysKey;

                DbExcutor.ExcuteNonQuery($@"
                UPDATE {Table<OverridingProfileEntity>()} SET {With<OverridingProfileEntity>(x => x.PTOSK)} = '{newKey}' WHERE {With<OverridingProfileEntity>(x => x.SysKey)}='{profilesk}'",
                CommonConstant.SalesConnection);

                var receivedsk = _received.Finds(filter: f => f.InvoiceSK == oldKey).Last().SysKey;
                var alosk = _allocation.Finds(filter: f => f.HeaderSK == oldKey).Last().SysKey;

                DbExcutor.ExcuteNonQuery($@"
                UPDATE {Table<AllocationEntity>()} SET {With<AllocationEntity>(x => x.HeaderSK)}='{newKey}' WHERE {With<AllocationEntity>(x => x.SysKey)}='{alosk}'
                UPDATE {Table<CommissionReceivedEntity>()} SET {With<CommissionReceivedEntity>(x => x.InvoiceSK)}='{newKey}' WHERE {With<CommissionReceivedEntity>(x => x.SysKey)}='{receivedsk}'
                UPDATE {Table<SalesInvoiceLogEntity>()} SET {With<SalesInvoiceLogEntity>(x => x.PTOSK)} = '{newKey}' WHERE {With<SalesInvoiceLogEntity>(x => x.SysKey)} = '202103011348340085'
                ", CommonConstant.SalesConnection);
            
                var yc = _accountingPeriod.CurrentAccountingYearCode();
                var bSales = _batchControl.GetBatchTableName(yearCode: yc, moduleCode: CommonConstant.Module.MODULE_SALE);
                var bPurchase = _batchControl.GetBatchTableName(yearCode: yc, moduleCode: CommonConstant.Module.MODULE_PURCHASE);

                DbExcutor.ExcuteNonQuery($@"
                 UPDATE {bSales} SET SourceHeaderTransSK='{newKey}' WHERE SecondRef='{invoiceNo}' 
                 UPDATE {bPurchase} SET SourceHeaderTransSK='{newKey}' WHERE SecondRef='{invoiceNo}' 
                ", CommonConstant.AccountConnection);
            }

            DbExcutor.ExcuteNonQuery(@"DROP FUNCTION FindSeniorByRank", CommonConstant.SalesConnection);
        }

        #endregion
    }
}

