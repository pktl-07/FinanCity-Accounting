namespace FC.Core.Data.SQL
{
    using System;
    using System.Linq.Expressions;

    public interface IColumnFactory<TEntity> : IAggregrate<TEntity> 
        where TEntity : IEntity 
    {
        ISubQuery<TEntity> Column(Expression<Func<TEntity, dynamic>> expression);
    }
}
