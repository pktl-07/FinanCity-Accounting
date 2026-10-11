namespace FC.Core.Data.SQL
{
    using System;
    using System.Linq.Expressions;

    public interface IJoin : IWhere
    {
        IJoin InnerJoin<T3, T4>(Expression<Func<T3, T4, bool>> predicate)
            where T3 : IEntity
            where T4 : IEntity;

        IJoin LeftJoin<T3, T4>(Expression<Func<T3, T4, bool>> predicate)
            where T3 : IEntity
            where T4 : IEntity;

        IJoin RightJoin<T3, T4>(Expression<Func<T3, T4, bool>> predicate)
            where T3 : IEntity
            where T4 : IEntity;

        IJoin CrossJoin<T3, T4>(Expression<Func<T3, T4, bool>> predicate)
            where T3 : IEntity
            where T4 : IEntity;
    }
}
