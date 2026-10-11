namespace FC.Domain.Sales.Business
{
    using System;
    using System.Linq.Expressions;
    using FC.Core.Data;
    using FC.Domain.Sales.DAO;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Entity;

    public class SalesOrderType : BusinessLogic<SalesOrderTypeEntity, SalesOrderTypeDAO>
    {
        #region Methods

        public IDataPagerResult<SalesOrderTypeDTO> GetPager(int currentPage, int pageSize, Expression<Func<SalesOrderTypeDTO, bool>> filter = null)
        {
            try
            {
                return DAO.GetPager(currentPage: currentPage,
                    pageSize: pageSize,
                    filter: filter);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
