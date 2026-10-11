namespace FC.Domain.Accounts.Business
{
    using System;
    using System.Linq.Expressions;
    using FC.Core.Data;
    using FC.Domain.Accounts.DAO;
    using FC.Domain.Accounts.Entity;
  
    public class Department : BusinessLogic<DepartmentEntity, DepartmentDAO>
    {
        #region Methods

        public IDataPagerResult<DepartmentEntity> GetPager(int currentPage, int pageSize, Expression<Func<DepartmentEntity, bool>> where = null)
        {
            try
            {
                return DAO.GetPager(currentPage, pageSize, where);
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        #endregion
    }
}
