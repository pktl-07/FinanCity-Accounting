namespace FC.Domain.Sales.DAO
{
    using System;
    using System.Linq.Expressions;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;
    using FC.Domain.Sales.Entity;
   
    public class CountryDAO : DAO<CountryEntity>
    {
        #region Methods

        public IDataPagerResult<CountryEntity> GetPager(int currentPage, int pageSize, Expression<Func<CountryEntity, bool>> where = null)
        {
            try
            {
                var query = $@"SELECT * FROM {Table<CountryEntity>()}
                   WHERE {where.ToSQL()}
                   ORDER BY {With<CountryEntity>(x => x.Description)}";

                return ExcutePagerResult<CountryEntity>(query, currentPage, pageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
