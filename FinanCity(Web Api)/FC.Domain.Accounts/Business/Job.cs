namespace FC.Domain.Accounts.Business
{
    using System;
    using System.Linq.Expressions;
    using FC.Core.Data;
    using FC.Domain.Accounts.DAO;
    using FC.Domain.Accounts.Entity;

    public class Job : BusinessLogic<JobEntity, JobDAO>, IDataPageable<JobEntity>
    {
    
        #region Methods

        public IDataPagerResult<JobEntity> GetPager(int currentPage, int pageSize, Expression<Func<JobEntity, bool>> where = null)
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
