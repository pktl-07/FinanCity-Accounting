namespace FC.Domain.Sales.Business
{
    using System;
    using System.Linq.Expressions;
    using FC.Core.Data;
    using FC.Domain.Sales.DAO;
    using FC.Domain.Sales.Entity;

    public class MarketingItem : BusinessLogic<MarketingItemEntity, MarketingItemDAO>, IDataPageable<MarketingItemEntity>
    {
        #region Methods

        public IDataPagerResult<MarketingItemEntity> GetPager(int currentPage, int pageSize, Expression<Func<MarketingItemEntity, bool>> where = null)
        {
            try
            {
                return DAO.GetPager(currentPage, pageSize, where);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
