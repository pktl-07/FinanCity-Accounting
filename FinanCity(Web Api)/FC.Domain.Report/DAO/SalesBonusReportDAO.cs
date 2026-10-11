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

    public class SalesBonusReportDAO : DAO
    {
        #region Methods

        public DataTable RecruitmentIncentiveReport(Expression<Func<RecruitmentIncentiveReportFilter, bool>> filter)
        {
            try
            {
                var query = $@" SELECT 
                {With<CommissionReceivedEntity>(x => x.ReceiptDate)} [{nameof(RecruitmentIncentiveReportFilter.SalesDate)}],
                {With<PersonEntity>(x => x.SysKey)} [{nameof(RecruitmentIncentiveReportFilter.SPSK)}],
                {With<PersonEntity>(x => x.SysKey)} [ObjectSPSK],
                {With<PersonEntity>(x => x.Name)} [ObjectSPName],
                {With<PersonEntity>(x => x.CEANumber)} [ObjectCEANmber],
                (SELECT  {With<PersonEntity>(x => x.SysKey)}  FROM  {Table<PersonEntity>()}  WHERE  {With<PersonEntity>(x => x.SysKey)} = {With<PayableSchemeEntity>(x => x.SPSK)}) [SubjectSPSK],
                (SELECT  {With<PersonEntity>(x => x.Name)}  FROM  {Table<PersonEntity>()}  WHERE  {With<PersonEntity>(x => x.SysKey)} = {With<PayableSchemeEntity>(x => x.SPSK)}) [SubjectSPName],
                (SELECT  {With<PersonEntity>(x => x.CEANumber)}  FROM  {Table<PersonEntity>()}  WHERE  {With<PersonEntity>(x => x.SysKey)} = {With<PayableSchemeEntity>(x => x.SPSK)}) [SubjectCEANumber],
                {With<SalesInvoiceEntity>(x => x.R1)} [InvoiceNo],
                {With<PropertyTransactionEntity>(x => x.R1)} [TransNo],
                'T' + CAST({With<BonusPayableEntity>(x => x.Tier)}  AS varchar(50)) [Tire],
                dbo.ConvertCustomDateFormat({With<SalesInvoiceEntity>(x => x.InvoiceDate)}) [InvoiceDate],
                dbo.ConvertCustomDateFormat({With<PropertyTransactionEntity>(x => x.TransactionDate)}) [TransDate],
                dbo.AddressFrm({With<ContactEntity>(x => x.BuildingHouseNo)}, {With<ContactEntity>(x => x.Address1)},{With<ContactEntity>(x => x.Address2)}, {With<ContactEntity>(x => x.FloorUnit)}, {With<ContactEntity>(x => x.PostalCode)}) [Address],
                {With<ContactEntity>(x => x.Address1)} [Street],
                CAST({With<CommissionStructureDetailEntity>(x => x.AgencyPercent)}  AS varchar) +'/ '+CAST( {With<CommissionStructureDetailEntity>(x => x.AssociatedPercent)}  AS varchar) [CommScheme],
                {With<PayableSchemeEntity>(x => x.GrossComm)} [GrossComm],
                {With<BonusPayableEntity>(x => x.Percentage)} [MBPercent],
                {With<PayableSchemeEntity>(x => x.GrossComm)} / 100 *  {With<BonusPayableEntity>(x => x.Percentage)} [Amount]
                FROM  {Table<PayableSchemeEntity>()}
                INNER JOIN  {Table<BonusPayableEntity>()}  ON  {With<PayableSchemeEntity>(x => x.SysKey)} = {With<BonusPayableEntity>(x => x.PayableSchemeSK)}
                INNER JOIN  {Table<OverridingProfileEntity>()}  ON  {With<OverridingProfileEntity>(x => x.SysKey)}  =  {With<PayableSchemeEntity>(x => x.OverrideSchemeProfileSK)}
                INNER JOIN  {Table<OverridingSchemeEntity>()}  ON  {With<OverridingSchemeEntity>(x => x.SysKey)}  =  {With<PayableSchemeEntity>(x => x.OverrideSchemeSK)}
                INNER JOIN  {Table<PropertyTransactionEntity>()}  ON  {With<PropertyTransactionEntity>(x => x.SysKey)} = {With<OverridingProfileEntity>(x => x.PTOSK)}
                INNER JOIN  {Table<AllocationEntity>()}  ON  {With<AllocationEntity>(x => x.SysKey)} = {With<PayableSchemeEntity>(x => x.ALOSK)}
                INNER JOIN  {Table<CommissionReceivedEntity>()}  ON  {With<CommissionReceivedEntity>(x => x.InvoiceSK)} = {With<AllocationEntity>(x => x.HeaderSK)}
                INNER JOIN  {Table<ContactEntity>()}  ON  {With<ContactEntity>(x => x.SysKey)} = {With<PropertyTransactionEntity>(x => x.SysKey)}
                INNER JOIN  {Table<SalesInvoiceEntity>()}  ON  {With<SalesInvoiceEntity>(x => x.SysKey)} = {With<PropertyTransactionEntity>(x => x.SysKey)}
                INNER JOIN  {Table<PersonEntity>()}  ON  {With<PersonEntity>(x => x.SysKey)} = {With<BonusPayableEntity>(x => x.SPSK)}
                INNER JOIN  {Table<CommissionStructureDetailEntity>()}  ON  {With<CommissionStructureDetailEntity>(x => x.SysKey)} = {With<OverridingSchemeEntity>(x => x.SchemeSK)}
                WHERE 1=1 
                AND {With<BonusPayableEntity>(x => x.TransType)} = {(int)TransType.OverridingBonusPayable}
                AND {With<AllocationEntity>(x => x.TransType)} = {(int)TransType.AllocationCommission}";

                query = $@"SELECT * FROM ({query}) [tbl]
                    WHERE {filter.ToSQL()}
                    ORDER BY InvoiceNo";

                return DbExcutor.ExcuteDataTable(
                    query: query,
                    connection: "FC.Biz",
                    name: "AMS_TeamManagerBonusReportDataTable");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable RecruitmentIncentivePayableReport(Expression<Func<RecruitmentIncentiveReportFilter, bool>> filter)
        {
            try
            {
                var query = $@"SELECT 
                  { With<CommissionReceivedEntity>(x => x.ReceiptDate) }  [{nameof(RecruitmentIncentiveReportFilter.SalesDate)}],
                  { With<PersonEntity>(x => x.SysKey) } [{nameof(RecruitmentIncentiveReportFilter.SPSK)}],
                  { With<CompanyEntity>(x => x.SysKey) } [FranchiseSK],
                  { With<PersonEntity>(x => x.SysKey) } [ObjectSPSK],
                  dbo.ConvertCustomDateFormat({With<CommissionReceivedEntity>(x => x.ReceiptDate)}) [ReceiptDate],
                  (SELECT { With<PersonEntity>(x => x.NRIC)}  FROM  { Table<PersonEntity>()}  WHERE  { With<PersonEntity>(x => x.SysKey) } = { With<PayableSchemeEntity>(x => x.SPSK)}) [NRIC],
                  { With<ContactEntity>(x => x.Address1)} [PropertyAddress],
                  { With<PersonEntity>(x => x.Name)} [ObjectName],
                  { With<PersonEntity>(x => x.CEANumber)} [ObjectCEANumber],
                  (  SELECT  { With<PersonEntity>(x => x.Name)}  FROM  { Table<PersonEntity>()}  WHERE  { With<PersonEntity>(x => x.SysKey) } = { With<PayableSchemeEntity>(x => x.SPSK)} ) [SubjectName],
                  (  SELECT  { With<PersonEntity>(x => x.CEANumber)}  FROM  { Table<PersonEntity>()}  WHERE  { With<PersonEntity>(x => x.SysKey) } = { With<PayableSchemeEntity>(x => x.SPSK)} ) [SubjectCEANumber],
                  { With<SalesInvoiceEntity>(x => x.R1)} [InvoiceNo],
                  { With<PayableSchemeEntity>(x => x.GrossComm)} [GrossComm],
                  { With<BonusPayableEntity>(x => x.Tier)} [Tier],
                  { With<BonusPayableEntity>(x => x.Percentage)} [Percentage]
                   FROM  { Table<PayableSchemeEntity>()}
                   INNER JOIN  { Table<BonusPayableEntity>()}  ON  { With<PayableSchemeEntity>(x => x.SysKey) } = { With<BonusPayableEntity>(x => x.PayableSchemeSK)}
                   INNER JOIN  { Table<PersonEntity>()}  ON  { With<PersonEntity>(x => x.SysKey) } = { With<BonusPayableEntity>(x => x.SPSK)}
                   LEFT JOIN  { Table<CompanyEntity>()}  ON  { With<CompanyEntity>(x => x.SysKey) } = { With<PersonEntity>(x => x.VendorSK)}
                   INNER JOIN  { Table<AllocationEntity>()}  ON  { With<AllocationEntity>(x => x.SysKey) } = { With<PayableSchemeEntity>(x => x.ALOSK)}
                   INNER JOIN  { Table<CommissionReceivedEntity>()}  ON  { With<CommissionReceivedEntity>(x => x.InvoiceSK) } = { With<AllocationEntity>(x => x.HeaderSK)}
                   INNER JOIN  { Table<SalesInvoiceEntity>()}  ON  { With<SalesInvoiceEntity>(x => x.SysKey) } = { With<CommissionReceivedEntity>(x => x.InvoiceSK)}
                   INNER JOIN  { Table<PropertyTransactionEntity>()}  ON  { With<PropertyTransactionEntity>(x => x.SysKey) } = { With<SalesInvoiceEntity>(x => x.SysKey) }
                   INNER JOIN  { Table<ContactEntity>()}  ON  { With<ContactEntity>(x => x.SysKey) } = { With<PropertyTransactionEntity>(x => x.SysKey) }
                   WHERE 
                   { With<BonusPayableEntity>(x => x.SysKey) }  NOT IN  (  SELECT  { With<BonusPaymentDetailEntity>(x => x.BonusPayableSK)}  FROM  { Table<BonusPaymentDetailEntity>()}  WHERE  { With<BonusPaymentDetailEntity>(x => x.RecordStatus)} >= {(int)RecordStatus.Post} )
                        AND  { With<BonusPayableEntity>(x => x.Percentage)} > 0
                        AND  { With<SalesInvoiceEntity>(x => x.RecordStatus)} = {(int)RecordStatus.Post}
                        AND {With<AllocationEntity>(x => x.TransType)} = {(int)TransType.AllocationCommission}";

                query = $@"SELECT * FROM ({query}) [tbl]
                    WHERE {filter.ToSQL()}
                    ORDER BY InvoiceNo";

                return DbExcutor.ExcuteDataTable(
                    query: query,
                    connection: "FC.Biz",
                    name: "BonusPayablePaidReportDataTable");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable RecruitmentIncentivePaidReport(Expression<Func<RecruitmentIncentiveReportFilter, bool>> filter)
        {
            try
            {
                var query = $@" SELECT 
                {With<BonusPaymentEntity>(x => x.PaymentDate)} [{nameof(RecruitmentIncentiveReportFilter.SalesDate)}],
                { With<PersonEntity>(x => x.SysKey)} [{nameof(RecruitmentIncentiveReportFilter.SPSK)}],   
                { With<CompanyEntity>(x => x.SysKey)} [FranchiseSK],
                dbo.ConvertCustomDateFormat({With<CommissionReceivedEntity>(x => x.ReceiptDate)}) [ReceiptDate],
                { With<ContactEntity>(x => x.Address1)} [PropertyAddress],
                { With<PersonEntity>(x => x.SysKey)} [ObjectSPSK],
                { With<PersonEntity>(x => x.Name)} [ObjectName],
                { With<PersonEntity>(x => x.CEANumber)} [ObjectCEANumber],
                ( SELECT  { With<PersonEntity>(x => x.NRIC)}  FROM  { Table<PersonEntity>()}  WHERE  { With<PersonEntity>(x => x.SysKey)} = { With<PayableSchemeEntity>(x => x.SPSK)} ) [NRIC],
                ( SELECT  { With<PersonEntity>(x => x.Name)}  FROM  { Table<PersonEntity>()}  WHERE  { With<PersonEntity>(x => x.SysKey)} = { With<PayableSchemeEntity>(x => x.SPSK)}) [SubjectName],
                ( SELECT  { With<PersonEntity>(x => x.CEANumber)}  FROM  { Table<PersonEntity>()}  WHERE  {With<PersonEntity>(x => x.SysKey)} = { With<PayableSchemeEntity>(x => x.SPSK)}) [SubjectCEANumber],
                { With<SalesInvoiceEntity>(x => x.R1)} [InvoiceNo],
                { With<PayableSchemeEntity>(x => x.GrossComm)} [GrossComm],
                { With<BonusPayableEntity>(x => x.Tier)} [Tier],
                { With<BonusPayableEntity>(x => x.Percentage)} [Percentage]
                FROM  { Table<PayableSchemeEntity>()}
                INNER JOIN  { Table<BonusPayableEntity>()}  ON  { With<PayableSchemeEntity>(x => x.SysKey)} = { With<BonusPayableEntity>(x => x.PayableSchemeSK)}
                INNER JOIN  { Table<AllocationEntity>()}  ON  { With<AllocationEntity>(x => x.SysKey)} = { With<PayableSchemeEntity>(x => x.ALOSK)}
                INNER JOIN  { Table<CommissionReceivedEntity>()}  ON  { With<CommissionReceivedEntity>(x => x.InvoiceSK)} = { With<AllocationEntity>(x => x.HeaderSK)}
                INNER JOIN  { Table<SalesInvoiceEntity>()}  ON  { With<SalesInvoiceEntity>(x => x.SysKey)} = { With<AllocationEntity>(x => x.HeaderSK)}
                INNER JOIN  { Table<ContactEntity>()}  ON  { With<ContactEntity>(x => x.SysKey)} = { With<SalesInvoiceEntity>(x => x.SysKey)}
                INNER JOIN  { Table<PersonEntity>()}  ON  { With<PersonEntity>(x => x.SysKey)} = { With<BonusPayableEntity>(x => x.SPSK)}
                LEFT JOIN  { Table<CompanyEntity>()}  ON  { With<CompanyEntity>(x => x.SysKey)} = { With<PersonEntity>(x => x.VendorSK)}
                INNER JOIN  { Table<BonusPaymentDetailEntity>()}  ON  { With<BonusPayableEntity>(x => x.SysKey)} = { With<BonusPaymentDetailEntity>(x => x.BonusPayableSK)}
                INNER JOIN  { Table<BonusPaymentEntity>()}  ON  { With<BonusPaymentEntity>(x => x.SysKey)} = { With<BonusPaymentDetailEntity>(x => x.BonusPaymentSK)}
                WHERE  { With<BonusPaymentEntity>(x => x.RecordStatus)} >=  {(int)RecordStatus.Post}
                AND  { With<SalesInvoiceEntity>(x => x.RecordStatus)} = {(int)RecordStatus.Post}
                AND {With<AllocationEntity>(x => x.TransType)} = {(int)TransType.AllocationCommission}";

                query = $@"SELECT * FROM ({query}) [tbl]
                    WHERE {filter.ToSQL()}
                    ORDER BY InvoiceNo";

                return DbExcutor.ExcuteDataTable(
                    query: query,
                    connection: "FC.Biz",
                    name: "BonusPayablePaidReportDataTable");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable SalesBonusDetailReport(Expression<Func<SalesBonusReportFilter, bool>> filter)
        {
            try
            {
                var query = $@"
                    {SalesBonusDetailQuery(RankType.DD)}
                    UNION ALL
                    {SalesBonusDetailQuery(RankType.TM)}";
              
                query = $@"SELECT * FROM ({query}) [tbl]
                    WHERE {filter.ToSQL()}
                    ORDER BY InvoiceNo";

                return DbExcutor.ExcuteDataTable(
                    query: query,
                    connection: "FC.Biz",
                    name: "SalesBonusDetailDataTable");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion

        #region Utilities

        private string SalesBonusDetailQuery(RankType rank)
        {
            var query = "";

            switch (rank)
            {
                case RankType.DD:
                    query = $@"SELECT 
                        {With<OverridingProfileEntity>(x => x.DDSK)} [T1], 
                        (SELECT 'T'+ CAST(Level AS VARCHAR(50)) FROM dbo.DownTreeLevel({With<OverridingProfileEntity>(x => x.DDSK)}) WHERE SPSK = {With<PersonEntity>(x => x.SysKey)}) [Level],
                        {With<OverridingProfileEntity>(x => x.DDPayoutPercent)} [Payout],
                        {(int)RankType.DD} [Rank],";
                    break;

                case RankType.TM:
                    query = $@"SELECT 
                        {With<OverridingProfileEntity>(x => x.MGRSK)} [T1],
                        (SELECT 'T'+ CAST(Level AS VARCHAR(50)) FROM dbo.DownTreeLevel({With<OverridingProfileEntity>(x => x.MGRSK)}) WHERE SPSK = {With<PersonEntity>(x => x.SysKey)}) [Level],
                        {With<OverridingProfileEntity>(x => x.MGRPayoutPercent)} [Payout],
                        {(int)RankType.TM} [Rank],";
                    break;
            }

            query += $@"
                    {With<PropertyTransactionEntity>(x => x.R1)} [TransNo],
                    {With<SalesInvoiceEntity>(x => x.R1)} [InvoiceNo],
                    {With<PersonEntity>(x => x.SysKey)} [SPSK],
                    {With<PersonEntity>(x => x.Name)} [Name],
                    {With<PersonEntity>(x => x.NRIC)} [NRIC],
                    {With<ContactEntity>(x => x.Address1)} [Address],
                    dbo.ConvertCustomDateFormat({With<AllocationEntity>(x => x.AllocationDate)}) [InvoiceDate],
                    {With<AllocationEntity>(x => x.AllocationDate)} [{nameof(SalesBonusReportFilter.SalesDate)}],
                    {With<PayableSchemeEntity>(x => x.AssociateComm)} [NetComm]
                FROM {Table<PropertyTransactionEntity>()}
                INNER JOIN {Table<SalesInvoiceEntity>()} ON {With<SalesInvoiceEntity>(x => x.SysKey)} = {With<PropertyTransactionEntity>(x => x.SysKey)}
                INNER JOIN {Table<AllocationEntity>()} ON {With<SalesInvoiceEntity>(x => x.SysKey)} = {With<AllocationEntity>(x => x.HeaderSK)} 
                INNER JOIN {Table<PayableSchemeEntity>()} ON {With<PayableSchemeEntity>(x => x.ALOSK)} = {With<AllocationEntity>(x => x.SysKey)}
                INNER JOIN {Table<PersonEntity>()} ON {With<PersonEntity>(x => x.SysKey)} = {With<PayableSchemeEntity>(x => x.SPSK)}
                INNER JOIN {Table<OverridingProfileEntity>()} ON {With<OverridingProfileEntity>(x => x.PTOSK)} = {With<SalesInvoiceEntity>(x => x.SysKey)} AND {With<OverridingProfileEntity>(x => x.SPSK)} = {With<PersonEntity>(x => x.SysKey)}
                INNER JOIN {Table<ContactEntity>()} ON {With<ContactEntity>(x => x.SysKey)} = {With<PropertyTransactionEntity>(x => x.SysKey)}
                WHERE {With<AllocationEntity>(x => x.RecordStatus)} = <# {(int)RecordStatus.Post} #>
                AND {With<AllocationEntity>(x => x.TransType)} = {(int)TransType.AllocationCommission}";

            return query;
        }

        #endregion
    }
}
