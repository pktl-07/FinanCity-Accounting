namespace FC.Domain.Sales.DAO
{
    using System;
    using System.Linq.Expressions;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;
    using FC.Domain.Sales.Entity;
   
    public class IndustryExamDAO : DAO<IndustryExamEntity>
    {
        #region Methods

        public IDataPagerResult<IndustryExamEntity> GetPager(int currentPage, int pageSize, Expression<Func<IndustryExamEntity, bool>> where = null)
        {
            try
            {
                var query = $@"SELECT * FROM {Table<IndustryExamEntity>()} WHERE {where.ToSQL()}
                    ORDER BY {With<IndustryExamEntity>(x => x.Description)}";

                return ExcutePagerResult<IndustryExamEntity>(query, currentPage, pageSize);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
