namespace FC.Domain.Accounts.DAO
{
    using System;
    using System.Collections.Generic;
    using FC.Core.Data;
    using FC.Domain.Accounts.DTO;
    using FC.Domain.Accounts.Entity;
   
    public class AccountSetupDAO : DAO<AccountSetupEntity>
    {
        #region Methods

        public IEnumerable<ChartOfAccountDTO> FindCOATable()
        {
            try
            {
                var query = $@"SELECT 
                    {With<AccountSetupEntity>(x => x.SysKey)} [{nameof(ChartOfAccountDTO.AccountSK)}],
                    {With<AccountSetupEntity>(x => x.AccDescription)} [{nameof(ChartOfAccountDTO.AccountName)}],
                    {With<AccountSetupEntity>(x => x.AccCode)} [{nameof(ChartOfAccountDTO.AccountCode)}],
                    {With<AccountSetupEntity>(x => x.AccountType)} [{nameof(ChartOfAccountDTO.AccountType)}],
                    {With<AccountCategoryEntity>(x => x.CategoryDescription)} [{nameof(ChartOfAccountDTO.CategoryDescription)}],
                    CASE 
                        WHEN {With<AccountSetupEntity>(x => x.CategorySK)} >= 10000 AND {With<AccountSetupEntity>(x => x.CategorySK)} <= 19999 THEN 10000
                        WHEN {With<AccountSetupEntity>(x => x.CategorySK)} >= 20000 AND {With<AccountSetupEntity>(x => x.CategorySK)} <= 29999 THEN 20000
                        WHEN {With<AccountSetupEntity>(x => x.CategorySK)} >= 30000 AND {With<AccountSetupEntity>(x => x.CategorySK)} <= 39999 THEN 30000
                        WHEN {With<AccountSetupEntity>(x => x.CategorySK)} >= 40000 AND {With<AccountSetupEntity>(x => x.CategorySK)} <= 49999 THEN 40000
                        WHEN {With<AccountSetupEntity>(x => x.CategorySK)} >= 50000 AND {With<AccountSetupEntity>(x => x.CategorySK)} <= 59999 THEN 50000
                        WHEN {With<AccountSetupEntity>(x => x.CategorySK)} >= 60000 AND {With<AccountSetupEntity>(x => x.CategorySK)} <= 69999 THEN 60000
                        WHEN {With<AccountSetupEntity>(x => x.CategorySK)} >= 70000 AND {With<AccountSetupEntity>(x => x.CategorySK)} <= 79999 THEN 70000
                        WHEN {With<AccountSetupEntity>(x => x.CategorySK)} >= 80000 AND {With<AccountSetupEntity>(x => x.CategorySK)} <= 89999 THEN 80000
                        WHEN {With<AccountSetupEntity>(x => x.CategorySK)} >= 90000 AND {With<AccountSetupEntity>(x => x.CategorySK)} <= 99999 THEN 90000
                    END  [{nameof(ChartOfAccountDTO.GroupHeaderType)}],
                    {With<AccountSetupEntity>(x => x.IsActive)} [{nameof(ChartOfAccountDTO.IsActive)}],
                    0 [{nameof(ChartOfAccountDTO.Debit)}],
                    0 [{nameof(ChartOfAccountDTO.Credit)}],
                    0 [{nameof(ChartOfAccountDTO.Index)}]
                    FROM {Table<AccountSetupEntity>()}
                    LEFT JOIN {Table<AccountCategoryEntity>()} ON {With<AccountCategoryEntity>(x => x.SysKey)} = { With<AccountSetupEntity>(x => x.CategorySK)}";

                query = $@"SELECT * FROM ({query}) [tbl] ORDER BY 
                      [tbl].AccountSK";

                return DbExcutor<ChartOfAccountDTO>.ExcuteReader(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
