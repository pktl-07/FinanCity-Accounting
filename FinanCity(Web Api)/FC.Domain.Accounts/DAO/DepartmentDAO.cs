namespace FC.Domain.Accounts.DAO
{
    using System;
    using System.Linq.Expressions;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;
    using FC.Domain.Accounts.Entity;
  
    public class DepartmentDAO : DAO<DepartmentEntity>
    {
        #region Methods

        public IDataPagerResult<DepartmentEntity> GetPager(int currentPage, int pageSize, Expression<Func<DepartmentEntity, bool>> where = null)
        {
            try
            {
                var query = $@"SELECT * FROM {Table<DepartmentEntity>()}
                WHERE {where.ToSQL()}
                ORDER BY {With<DepartmentEntity>(x => x.Description)} DESC";

                return ExcutePagerResult<DepartmentEntity>(query, currentPage, pageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
