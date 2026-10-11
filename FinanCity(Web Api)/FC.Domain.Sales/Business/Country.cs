namespace FC.Domain.Sales.Business
{
    using System;
    using System.Linq.Expressions;
    using FC.Core.Data;
    using FC.Domain.Sales.DAO;
    using FC.Domain.Sales.Entity;
  
    public class Country : BusinessLogic<CountryEntity, CountryDAO>
    {
        #region Methods

        public IDataPagerResult<CountryEntity> GetPager(int currentPage, int pageSize, Expression<Func<CountryEntity, bool>> where = null)
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
