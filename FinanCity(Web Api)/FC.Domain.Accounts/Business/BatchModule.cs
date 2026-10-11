namespace FC.Domain.Accounts.Business
{
    using System;
    using FC.Core.Data;
    using FC.Core.Infrastructure;
    using FC.Domain.Accounts.DAO;
    using FC.Domain.Accounts.Entity;

    public class BatchModule : BusinessLogic<BatchModuleEntity, BatchModuleDAO>
    {
   
        #region Methods

        public bool IsValidEntry(int postDate, string moduleCode)
        {
            try
            {
                Guard.IsNotNullOrEmpty(moduleCode, "moduleCode");

                return DAO.IsValidEntry(postDate, moduleCode);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
