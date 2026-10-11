namespace FC.Domain.Sales.DAO
{
    using System;
    using System.Linq.Expressions;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;
    using FC.Domain.Sales.Entity;
   
    public class QualificationDAO : DAO<QualificationEntity>
    {
        #region Methods

        public IDataPagerResult<QualificationEntity> GetPager(int currentPage, int pageSize, Expression<Func<QualificationEntity, bool>> where = null)
        {
            try
            {
                var query = $@"SELECT * FROM {Table<QualificationEntity>()} WHERE {where.ToSQL()}
                    ORDER BY {With<QualificationEntity>(x => x.Description)}";

                return ExcutePagerResult<QualificationEntity>(query, currentPage, pageSize);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
