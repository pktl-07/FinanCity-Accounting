namespace FC.Core.Data.SQL
{
    using System;
    using System.Linq.Expressions;

    public interface IAggregrate<TEntity> where TEntity : IEntity
    {
        void Sum<TValue>(Expression<Func<TEntity, TValue>> expression) where TValue : struct;

        void Avg<TValue>(Expression<Func<TEntity, TValue>> expression) where TValue : struct;

        void Min<TValue>(Expression<Func<TEntity, TValue>> expression) where TValue : struct;

        void Max<TValue>(Expression<Func<TEntity, TValue>> expression) where TValue : struct;

        void Count<TValue>(Expression<Func<TEntity, TValue>> expression);
    }
}
