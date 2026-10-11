namespace FC.Domain.Sales.Business
{
    using System;
    using System.Linq.Expressions;
    using FC.Core;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;
    using FC.Domain.Sales.DAO;
    using FC.Domain.Sales.Entity;

    public class PTOClient : BusinessLogic<PTOClientEntity, PTOClientDAO>
    {
        #region Methods

        public IDataPagerResult<PTOClientEntity> GetPager(int currentPage, int pageSize, Expression<Func<PTOClientEntity, bool>> where = null)
        {
            try
            {
                return DAO.GetPager(currentPage: currentPage,
                    pageSize: pageSize,
                    where: where);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
