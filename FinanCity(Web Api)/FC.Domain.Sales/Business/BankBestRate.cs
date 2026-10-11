namespace FC.Domain.Sales.Business
{
    using System;
    using System.Linq.Expressions;
    using FC.Core.Data;
    using FC.Domain.Sales.DAO;
    using FC.Domain.Sales.Entity;
  
    public class BankBestRate : BusinessLogic<BankBestRateEntity, BankBestRateDAO>
    {
        public IDataPagerResult<BankBestRateEntity> GetPager(int currentPage, int pageSize, 
            Expression<Func<BankBestRateEntity, bool>> where = null)
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
    }
}
