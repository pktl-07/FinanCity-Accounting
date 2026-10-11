namespace FC.Domain.Sales.Business
{
    using System;
    using System.Linq.Expressions;
    using FC.Core.Data;
    using FC.Domain.Sales.DAO;
    using FC.Domain.Sales.Entity;

    public class Qualification : BusinessLogic<QualificationEntity, QualificationDAO>, IDataPageable<QualificationEntity>
    {
        #region Methods

        public IDataPagerResult<QualificationEntity> GetPager(int currentPage, int pageSize, Expression<Func<QualificationEntity, bool>> where = null)
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
