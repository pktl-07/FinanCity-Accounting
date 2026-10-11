namespace FC.Domain.Sales.Business
{
    using System;
    using System.Linq.Expressions;
    using FC.Core.Data;
    using FC.Domain.Sales.DAO;
    using FC.Domain.Sales.Entity;

    public class IndustryExam : BusinessLogic<IndustryExamEntity, IndustryExamDAO>, IDataPageable<IndustryExamEntity>
    {
        #region Methods

        public IDataPagerResult<IndustryExamEntity> GetPager(int currentPage, int pageSize, Expression<Func<IndustryExamEntity, bool>> where = null)
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
