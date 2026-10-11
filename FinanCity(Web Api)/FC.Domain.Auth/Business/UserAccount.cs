namespace FC.Domain.Auth.Business
{
    using System;
    using System.Linq.Expressions;
    using FC.Core.Data;
    using FC.Domain.Auth.DAO;
    using FC.Domain.Auth.Entity;
   
    public class UserAccount : BusinessLogic<UserAccountEntity, UserAccountDAO>
    {
        #region Methods

        public IDataPagerResult<UserAccountEntity> GetPager(int currentPage, int pageSize, Expression<Func<UserAccountEntity, bool>> where = null)
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
