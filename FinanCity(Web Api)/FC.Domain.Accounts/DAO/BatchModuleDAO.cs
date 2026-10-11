namespace FC.Domain.Accounts.DAO
{
    using System;
    using FC.Domain.Accounts.Entity;
    using FC.Core;
    using FC.Core.Data;

    public class BatchModuleDAO : DAO<BatchModuleEntity>
    {
        #region Methods

        public bool IsValidEntry(int postDate, string moduleCode)
        {
            try
            {
                var query = $@"SELECT {With<BatchModuleEntity>(x => x.EntryStatus)} FROM {Table<BatchModuleEntity>()}
                WHERE {With<BatchModuleEntity>(x => x.ModuleCode)} = <# {moduleCode} #>
                AND {With<BatchModuleEntity>(x => x.YC)} = (SELECT {With<AccountingPeriodEntity>(x => x.YC)} FROM {Table<AccountingPeriodEntity>()} WHERE {With<AccountingPeriodEntity>(x => x.Date1)} <= {postDate} AND {With<AccountingPeriodEntity>(x => x.Date2)} >= {postDate} )    
                AND {With<BatchModuleEntity>(x => x.PC)} = (SELECT {With<AccountingPeriodEntity>(x => x.PC)} FROM {Table<AccountingPeriodEntity>()} WHERE {With<AccountingPeriodEntity>(x => x.Date1)} <= {postDate} AND {With<AccountingPeriodEntity>(x => x.Date2)} >= {postDate} )";

                return (int)DbExcutor.ExcuteScalar(query, CommonConstant.AccountConnection) == 1;
            }
            catch
            {
                throw new Exception($"Invalid entery date for current financial year."
                + $"\n Or you may need to create account yearing for {postDate.ToString().Substring(0, 4)}.");
            }
        }

        #endregion
    }
}
