namespace FC.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using FC.Core.Data;

    public interface IUniqueFieldAnalyzer<TEntity> where TEntity : IEntity
    {
        void Register(Expression<Func<TEntity, object[]>> fields);

        void ValidateOnSave(TEntity entity);

        void ValidateOnUpdate<T>(IDictionary<string, object> fields,
            string filter) where T : IEntity;
    }
}
