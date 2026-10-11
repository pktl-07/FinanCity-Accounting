namespace FC.Core
{
    using System;
    using System.Linq.Expressions;
    using FC.Core.Data;

    public interface IReferenceFieldAnalyzer
    {
        void Register<TEntity>(Expression<Func<TEntity, object>> depends) where TEntity : IEntity;
        void ValidateOnDelete<TEntity>(string table, string where) where TEntity : IEntity;
    }
}
