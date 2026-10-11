namespace FC.Domain.Accounts.Business
{
    using System;
    using System.Linq.Expressions;
    using FC.Core.Data;
    using FC.Domain.Accounts.DAO;
    using FC.Domain.Accounts.DTO;
    using FC.Domain.Accounts.Entity;
   
    public class CashbookSetup : BusinessLogic<CashbookSetupEntity, CashbookSetupDAO>
    {
        #region Methods

        public IDataPagerResult<CashbookSetupDTO> GetPager(int currentPage, int pageSize, Expression<Func<CashbookSetupDTO, bool>> where = null)
        {
            try
            {
                return DAO.GetPager(currentPage, pageSize, where: where);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
