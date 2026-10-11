namespace FC.Domain.Accounts.Business
{
    using System;
    using System.Linq.Expressions;
    using FC.Core.Data;
    using FC.Domain.Accounts.DAO;
    using FC.Domain.Accounts.Entity;

    public class Tax : BusinessLogic<TaxEntity, TaxDAO>, IDataPageable<TaxEntity>
    {
        #region Methods

        public IDataPagerResult<TaxEntity> GetPager(int currentPage, int pageSize, Expression<Func<TaxEntity, bool>> where = null)
        {
            try
            {
                return DAO.GetPager(currentPage: currentPage, pageSize: pageSize, where: where);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
