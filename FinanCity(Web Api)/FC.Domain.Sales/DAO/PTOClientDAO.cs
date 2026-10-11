namespace FC.Domain.Sales.DAO
{
    using System;
    using System.Linq.Expressions;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;
    using FC.Domain.Sales.Entity;

    public class PTOClientDAO : DAO<PTOClientEntity>
    {
        #region Methods

        public IDataPagerResult<PTOClientEntity> GetPager(int currentPage, int pageSize, Expression<Func<PTOClientEntity, bool>> where = null)
        {
            try
            {
                var query = $@"SELECT * FROM {Table<PTOClientEntity>()} WHERE {where.ToSQL()}
                    ORDER BY {With<PTOClientEntity>(x => x.IsMain)} DESC";

                return ExcutePagerResult<PTOClientEntity>(query, currentPage, pageSize);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
