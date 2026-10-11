namespace FC.Core.Data.SQL
{
    using System;
    using System.Linq.Expressions;

    public interface IWhere
    {
        IHaving Where<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : IEntity;

        IHaving Where<TEntity>(Expression<Func<TEntity, bool>> expression, Action<IAppendix> action) where TEntity : IEntity;
    }
}
