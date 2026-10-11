namespace FC.Domain.Accounts.DAO
{
    using System;
    using System.Linq.Expressions;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;
    using FC.Domain.Accounts.Entity;

    public class JobDAO : DAO<JobEntity>
    {
        #region Methods

        public IDataPagerResult<JobEntity> GetPager(
            int currentPage,
            int pageSize,
            Expression<Func<JobEntity, bool>> where = null)
        {
            try
            {
                var query = $@"SELECT * FROM {Table<JobEntity>()} 
                   WHERE {where.ToSQL()}
                   ORDER BY {With<JobEntity>(x => x.Auto)}
                   DESC";

                return ExcutePagerResult<JobEntity>(query, currentPage, pageSize);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
