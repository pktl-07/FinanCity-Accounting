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
   
    public class CashbookSetupDAO : DAO<CashbookSetupEntity>
    {
        #region Methods

        public IDataPagerResult<CashbookSetupDTO> GetPager(int currentPage, int pageSize, Expression<Func<CashbookSetupDTO, bool>> where = null)
        {
            try
            {
                var r = DI.Current.Get<IEssDataRepository>();
                var mkdb = "[" + new SqlConnectionStringBuilder(r.DbConnection["FC.Biz"].ConnectionString).InitialCatalog + "]";

                var query = $@"SELECT
                     {With<CashbookSetupEntity>(x => x.SysKey)} [{nameof(CashbookSetupDTO.CBSK)}],
                     {With<AccountSetupEntity>(x => x.AccCode)} [{nameof(CashbookSetupDTO.AccountCode)}],
                     {With<CashbookSetupEntity>(x => x.Description)} [{nameof(CashbookSetupDTO.BankAccountName)}],   
                     {mkdb}..[Currency].Currency_AlphaCode [{nameof(CashbookSetupDTO.CurrencyCode)}],
                     {With<AccountSetupEntity>(x => x.AccDescription)} [{nameof(CashbookSetupDTO.AccountDescription)}],                     
                     {With<CashbookSetupEntity>(x => x.IsActive)} [{nameof(CashbookSetupDTO.IsActive)}],
                     {With<CashbookSetupEntity>(x => x.CashbookName)} [{nameof(CashbookSetupDTO.CashbookName)}],
                     CASE
                     WHEN {With<CashbookSetupEntity>(x => x.CBType)} = {(int)CashBookType.Cash} THEN  '{CashBookType.Cash.ToDescription()}'
                     WHEN {With<CashbookSetupEntity>(x => x.CBType)} = {(int)CashBookType.Bank} THEN '{CashBookType.Bank.ToDescription()}'
                     END [{nameof(CashbookSetupDTO.CashbookType)}]

                     FROM {Table<CashbookSetupEntity>()} 
                     LEFT JOIN {Table<AccountSetupEntity>()} ON {With<AccountSetupEntity>(x => x.SysKey)}={With<CashbookSetupEntity>(x => x.AccSK)}
                     INNER JOIN {mkdb}..[Currency] ON {mkdb}..[Currency].SysKey={With<CashbookSetupEntity>(x => x.CURRSK)}";

                query = $@"SELECT * FROM ({ query}) [tbl] WHERE {where.ToSQL()}
                            ORDER BY [tbl].BankAccountName";

                return ExcutePagerResult<CashbookSetupDTO>(query, currentPage, pageSize);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
