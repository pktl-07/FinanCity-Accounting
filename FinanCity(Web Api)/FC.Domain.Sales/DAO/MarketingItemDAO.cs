namespace FC.Domain.Sales.DAO
{
    using System;
    using System.Linq.Expressions;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;
    using FC.Domain.Sales.Entity;

    public class MarketingItemDAO : DAO<MarketingItemEntity>
    {
        #region Methods

        public IDataPagerResult<MarketingItemEntity> GetPager(int currentPage, int pageSize, Expression<Func<MarketingItemEntity, bool>> where = null)
        {
            try
            {
                var query = $@"SELECT * FROM {Table<MarketingItemEntity>()} WHERE {where.ToSQL()}
                    ORDER BY {With<MarketingItemEntity>(x => x.Description)}";

                return ExcutePagerResult<MarketingItemEntity>(query, currentPage, pageSize);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
