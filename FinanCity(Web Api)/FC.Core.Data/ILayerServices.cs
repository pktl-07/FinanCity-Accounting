namespace FC.Core.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    /// <summary>
    /// Basic entity features.
    /// </summary>
    public interface ILayerServices<TEntity> where TEntity : IEntity
    {
        TEntity Insert(TEntity entity);

        TEntity Insert(Expression<Func<TEntity, TEntity>> selector);

        Task<TEntity> InsertAsync(TEntity entity);

        Task<TEntity> InsertAsync(TEntity entity, Action<TEntity> action);

        Task<TEntity> InsertAsync(Expression<Func<TEntity, TEntity>> selector);

        void BulkInsert(IEnumerable<TEntity> entities);

        Task BulkInsertAsync(IEnumerable<TEntity> entities);

        void Update(TEntity entity, Expression<Func<TEntity, bool>> filter);

        void Update(TEntity entity, Action<TEntity> action, Expression<Func<TEntity, bool>> filter);

        Task UpdateAsync(TEntity entity, Expression<Func<TEntity, bool>> filter);

        Task UpdateAsync(TEntity entity, Action<TEntity> action, Expression<Func<TEntity, bool>> filter);

        void Update(Expression<Func<TEntity, TEntity>> selector, Expression<Func<TEntity, bool>> filter);
        
        Task UpdateAsync(Expression<Func<TEntity, TEntity>> selector, Expression<Func<TEntity, bool>> filter);

        void Delete(Expression<Func<TEntity, bool>> filter, string table = "");

        Task DeleteAsync(Expression<Func<TEntity, bool>> filter, string table = "");

        IEnumerable<TEntity> Finds(Expression<Func<TEntity, dynamic>> selector = null, Expression<Func<TEntity, bool>> filter = null);

        IEnumerable<TEntity> Finds(Expression<Func<TEntity, bool>> filter);

        Task<IEnumerable<TEntity>> FindsAsync(Expression<Func<TEntity, dynamic>> selector = null, Expression<Func<TEntity, bool>> filter = null);

        IEnumerable<TEntity> Finds(string expression);

        Task<IEnumerable<TEntity>> FindsAsync(string expression);

        Expression<Func<T, bool>> FilterExpr<T>(string expression);

        IEnumerable<TEntity> Query(string query);
    }
}
