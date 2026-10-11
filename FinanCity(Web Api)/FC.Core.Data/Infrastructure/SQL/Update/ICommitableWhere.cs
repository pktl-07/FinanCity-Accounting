namespace FC.Core.Data.SQL
{
    using System;
    using System.Linq.Expressions;

    public interface ICommitableWhere
    {
        IQueryEnd Where<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : IEntity;
    }
}
