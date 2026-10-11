namespace FC.Domain.Accounts.DAO
{
    using System;
    using System.Linq.Expressions;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;
    using FC.Domain.Accounts.Entity;

    public class CashBookTransferDAO : DAO<CashBookTransferEntity>
    {
        #region Methods

        public IDataPagerResult<CashBookTransferEntity> GetPager(
            int currentPage, 
            int pageSize,
            Expression<Func<CashBookTransferEntity, bool>> where = null)
        {
            try
            {
                var query = $@"SELECT * FROM {Table<CashBookTransferEntity>()} 
                   WHERE {where.ToSQL()}
                   ORDER BY {With<CashBookTransferEntity>(x => x.Auto)}
                   DESC";

                return ExcutePagerResult<CashBookTransferEntity>(query, currentPage, pageSize);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
