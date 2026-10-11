namespace FC.Domain.Accounts.DAO
{
    using System;
    using System.Linq.Expressions;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;
    using FC.Domain.Accounts.Entity;

    public class TaxDAO : DAO<TaxEntity>
    {
        #region Methods

        public IDataPagerResult<TaxEntity> GetPager(int currentPage, int pageSize, Expression<Func<TaxEntity, bool>> where = null)
        {
            try
            {
                var query = $@"SELECT * FROM {Table<TaxEntity>()}
                WHERE {where.ToSQL()}
                ORDER BY {With<TaxEntity>(x => x.TaxCode)} DESC";

                return ExcutePagerResult<TaxEntity>(query, currentPage, pageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
