namespace FC.Domain.Sales.Business
{
    using System;
    using System.Linq.Expressions;
    using FC.Core.Data;
    using FC.Domain.Sales.DAO;
    using FC.Domain.Sales.DTO;

    public class DataList : BusinessLogic<DataListDAO>
    {
        #region Methods

        public IDataPagerResult<SalespersonCommissionDTO> GetSalespersonCommissionPager(
            int currentPage,
            int pageSize,
            Expression<Func<SalespersonCommissionDTO, bool>> where = null)
        {
            try
            {
                return DAO.GetSalespersonCommissionPager(currentPage: currentPage,
                    pageSize: pageSize,
                    where: where);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IDataPagerResult<LeadAndContactDTO> GetClientAndLeadPager(int currentPage, int pageSize, Expression<Func<LeadAndContactDTO, bool>> where = null)
        {
            try
            {
                return DAO.GetClientAndLeadPager(currentPage: currentPage,
                    pageSize: pageSize,
                    where: where);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IDataPagerResult<LeadAndSalespersonDTO> GetLeadAndSalesperson(int currentPage, int pageSize, Expression<Func<LeadAndSalespersonDTO, bool>> where = null)
        {
            try
            {
                return DAO.GetLeadAndSalesperson(currentPage: currentPage,
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
