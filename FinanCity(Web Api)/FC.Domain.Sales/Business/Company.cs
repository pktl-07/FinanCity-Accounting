namespace FC.Domain.Sales.Business
{
    using System;
    using System.Linq.Expressions;
    using FC.Core.Data;
    using FC.Domain.Sales.DAO;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Entity;

    public class Company : BusinessLogic<CompanyEntity, CompanyDAO>, IDataPageable<CompanyAndContactDTO>
    {
        #region Methods

        public IDataPagerResult<CompanyAndContactDTO> GetPager(int currentPage, int pageSize, Expression<Func<CompanyAndContactDTO, bool>> where = null)
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
