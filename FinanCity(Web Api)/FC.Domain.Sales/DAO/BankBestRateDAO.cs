namespace FC.Domain.Sales.DAO
{
    using System;
    using System.Linq.Expressions;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;
    using FC.Domain.Sales.Entity;

    public class BankBestRateDAO : DAO<BankBestRateEntity>
    {
        #region Methods

        public IDataPagerResult<BankBestRateEntity> GetPager(int currentPage, int pageSize, Expression<Func<BankBestRateEntity, bool>> where = null)
        {
            try
            {
                var query = $@"SELECT * FROM {Table<BankBestRateEntity>()} WHERE {where.ToSQL()}
                    ORDER BY {With<BankBestRateEntity>(x => x.Rate)}";

                return ExcutePagerResult<BankBestRateEntity>(query, currentPage, pageSize);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
