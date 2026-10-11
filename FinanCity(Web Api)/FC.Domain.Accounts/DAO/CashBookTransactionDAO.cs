namespace FC.Domain.Accounts.DAO
{
    using System;
    using System.Data.SqlClient;
    using System.Linq.Expressions;
    using Ninject;
    using FC.Core;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;
    using FC.Domain.Accounts.DTO;
    using FC.Domain.Accounts.Entity;
   
    public class CashBookTransactionDAO : DAO<CashBookTransactionEntity>
    {
        #region Methods

        public IDataPagerResult<CashbookTransactionDTO> GetPager(int currentPage,
               int pageSize,
               Expression<Func<CashbookTransactionDTO, bool>> where = null)
        {
            try
            {
                var r = DI.Current.Get<IEssDataRepository>();
                var mkdb = "[" + new SqlConnectionStringBuilder(r.DbConnection["FC.Biz"].ConnectionString).InitialCatalog + "]";

                var query = $@"SELECT {With<CashBookTransactionEntity>(x => x.R1)} [{nameof(CashbookTransactionDTO.ReferenceNo)}],
                {With<CashBookTransactionEntity>(x => x.Description)} [{nameof(CashbookTransactionDTO.TransDescription)}],
                {With<CashBookTransactionEntity>(x => x.TransactionDate)} [{nameof(CashbookTransactionDTO.TransactionDate)}],
                CASE 
                    WHEN {With<CashbookSetupEntity>(x => x.CBType)} = {(int)CashBookType.Cash} THEN 'CASH'
                    WHEN {With<CashbookSetupEntity>(x => x.CBType)}= {(int)CashBookType.Bank} THEN 'BANK'
                END [{nameof(CashbookTransactionDTO.CashbookType)}],
                {With<CashbookSetupEntity>(x => x.CashbookName)} [{nameof(CashbookTransactionDTO.CashbookName)}],
                {With<CashbookSetupEntity>(x => x.Description)} [{nameof(CashbookTransactionDTO.CashbookDescription)}],
                {With<CashBookTransactionEntity>(x => x.JRef)} [{nameof(CashbookTransactionDTO.JRef)}],
                SUM({With<CashBookTransactionEntity>(x => x.TotalAmount)}) [{nameof(CashbookTransactionDTO.TotalNetAmount)}],
                {With<CashBookTransactionEntity>(x => x.RecordStatus)} [{nameof(CashbookTransactionDTO.RecordStatus)}],
                {With<CashBookTransactionEntity>(x => x.SecondRef)} [{nameof(CashbookTransactionDTO.SecondRef)}],
                {With<CashBookTransactionEntity>(x => x.ParentId)} [{nameof(CashbookTransactionDTO.ParentId)}],
                {With<CashBookTransactionEntity>(x => x.RefTransactionType)} [{nameof(CashbookTransactionDTO.RefTransType)}],
                {With<CashBookTransactionEntity>(x => x.TransType)} [{nameof(CashbookTransactionDTO.TransType)}],
                {With<CashBookTransactionEntity>(x => x.CURRSK)} [{nameof(CashbookTransactionDTO.CURRSK)}],
                {With<CashBookTransactionEntity>(x => x.CURROP)} [{nameof(CashbookTransactionDTO.CURROP)}],
                {With<CashBookTransactionEntity>(x => x.CURRATE)} [{nameof(CashbookTransactionDTO.CURRATE)}],
                {mkdb}..[Currency].Currency_AlphaCode [{nameof(CashbookTransactionDTO.CurrencyCode)}]
                FROM {Table<CashBookTransactionEntity>()}
                INNER JOIN {Table<CashbookSetupEntity>()} ON {With<CashbookSetupEntity>(x => x.SysKey)}={With<CashBookTransactionEntity>(x => x.CBSK)}
                INNER JOIN {mkdb}..[Currency] ON {mkdb}..[Currency].SysKey={With<CashBookTransactionEntity>(x => x.CURRSK)}
                GROUP BY
                     {With<CashBookTransactionEntity>(x => x.JRef)},
                     {With<CashBookTransactionEntity>(x => x.R1)},
                     {With<CashBookTransactionEntity>(x => x.Description)},
                     {With<CashBookTransactionEntity>(x => x.TransactionDate)},
                     {With<CashbookSetupEntity>(x => x.CashbookName)},
                     {With<CashbookSetupEntity>(x => x.CBType)},
                     {With<CashbookSetupEntity>(x => x.Description)},
                     {With<CashBookTransactionEntity>(x => x.RecordStatus)},
                     {With<CashBookTransactionEntity>(x => x.SecondRef)},
                     {With<CashBookTransactionEntity>(x => x.ParentId)},
                     [Currency].Currency_AlphaCode,       
                     {With<CashBookTransactionEntity>(x => x.RefTransactionType)},
                     {With<CashBookTransactionEntity>(x => x.CURRSK)},
                     {With<CashBookTransactionEntity>(x => x.CURROP)},
                     {With<CashBookTransactionEntity>(x => x.CURRATE)},
                     {With<CashBookTransactionEntity>(x => x.TransType)}";

                query = $@"SELECT * FROM ({query}) [tbl] WHERE {where.ToSQL()}
                      ORDER BY 
                      [tbl].RecordStatus, 
                      [tbl].TransactionDate DESC";
                return ExcutePagerResult<CashbookTransactionDTO>(query, currentPage, pageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
