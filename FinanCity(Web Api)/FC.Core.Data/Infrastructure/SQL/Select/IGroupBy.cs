namespace FC.Core.Data.SQL
{
    using System;
    using System.Linq.Expressions;

    public interface IGroupBy : IOrderBy
    {
        IGroupBy GroupBy<T>(Expression<Func<T, dynamic>> predicate) where T : IEntity;
    }
}
