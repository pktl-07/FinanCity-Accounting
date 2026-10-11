namespace FC.Core.Data
{
    using System;
    using System.Linq.Expressions;

    public interface IDataPageable<T> where T : IEntity
    {
        IDataPagerResult<T> GetPager(int currentPage, int pageSize, Expression<Func<T, bool>> where = null);
    }
}
