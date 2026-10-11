namespace FC.Domain.Auth.DAO
{
    using System;
    using System.Linq.Expressions;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;
    using FC.Domain.Auth.Entity;

    public class UserAccountDAO : DAO<UserAccountEntity>
    {
        #region Methods

        public IDataPagerResult<UserAccountEntity> GetPager(int currentPage, int pageSize, Expression<Func<UserAccountEntity, bool>> where = null)
        {
            try
            {
                var query = $@"SELECT * FROM {Table<UserAccountEntity>()}
                WHERE {where.ToSQL()}
                ORDER BY {With<UserAccountEntity>(x => x.PasswordLoginUserName)} DESC";

                return ExcutePagerResult<UserAccountEntity>(query, currentPage, pageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
