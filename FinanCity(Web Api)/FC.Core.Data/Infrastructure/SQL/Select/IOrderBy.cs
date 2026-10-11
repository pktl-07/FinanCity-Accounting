namespace FC.Core.Data.SQL
{
    using System;
    using System.Linq.Expressions;

    public interface IOrderBy
    {
        IOrderBy OrderBy<T>(Expression<Func<T, dynamic>> predicate) where T : IEntity;

        IOrderBy OrderBy<T>(Expression<Func<T, dynamic>> predicate, OrderType order) where T : IEntity;
    }
}
