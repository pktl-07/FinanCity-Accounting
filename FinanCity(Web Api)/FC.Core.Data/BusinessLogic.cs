namespace FC.Core.Data
{
    using System;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Ninject;
    using FC.Core.Infrastructure;

    public abstract class BusinessLogic<TDAO>
        where TDAO : DAO
    {
        #region Fields

        protected TDAO DAO { get; private set; } = DI.Current.Get<TDAO>();// Activator.CreateInstance<TDAO>();

        #endregion

    }

    public abstract class BusinessLogic<TEntity, TDAO> : ILayerServices<TEntity>, IHideObjectMembers
        where TEntity : IEntity
        where TDAO : DAO<TEntity>
    {

        #region Fields

        protected TDAO DAO { get; private set; } = DI.Current.Get<TDAO>(); // Activator.CreateInstance<TDAO>();

        #endregion

        #region Methods

        /// <summary>
        /// Db insert.
        /// </summary>
        /// <param name="entity">IEntity</param>
        /// <returns>Async result.</returns>
        public virtual TEntity Insert(TEntity entity)
        {
            try
            {
                return DAO.Insert(entity);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Db insert.
        /// </summary>
        /// <param name="entity">IEntity</param>
        /// <param name="action">Additional value to bind</param>
        /// <returns></returns>
        public virtual TEntity Insert(TEntity entity, Action<TEntity> action)
        {
            try
            {
          

                action(entity);

                return DAO.Insert(entity);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Db insert async.
        /// </summary>
        /// <param name="entity">IEntity</param>
        /// <returns>Async result.</returns>
        public virtual Task<TEntity> InsertAsync(TEntity entity)
        {
            try
            {
          

                return DAO.InsertAsync(entity);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Db insert async.
        /// </summary>
        /// <param name="entity">IEntity</param>
        /// <param name="action">Additional value to bind</param>
        /// <returns>Async result.</returns>
        public virtual Task<TEntity> InsertAsync(TEntity entity, Action<TEntity> action)
        {
            try
            {
          

                action(entity);

                return DAO.InsertAsync(entity);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Db insert async
        /// </summary>
        /// <param name="selectorexpr">Selector.</param>
        /// <returns>Async result.</returns>
        public virtual TEntity Insert(Expression<Func<TEntity, TEntity>> selector)
        {
            try
            {
                return DAO.Insert(selector);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Async Db insert async
        /// </summary>
        /// <param name="selectorexpr">Selector.</param>
        /// <returns>Async result.</returns>
        public virtual Task<TEntity> InsertAsync(Expression<Func<TEntity, TEntity>> selectorexpr)
        {
            try
            {
                return DAO.InsertAsync(selectorexpr);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Bulk insert async.
        /// </summary>
        /// <param name="entities">Enumerable IEntity</param>
        /// <returns>Async result.</returns>
        public virtual void BulkInsert(IEnumerable<TEntity> entities)
        {
            try
            {
                DAO.BulkInsert(entities);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Bulk insert async.
        /// </summary>
        /// <param name="entities">Enumerable IEntity</param>
        /// <returns>Async result.</returns>
        public virtual Task BulkInsertAsync(IEnumerable<TEntity> entities)
        {
            try
            {
                return DAO.BulkInsertAsync(entities);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Db update async.
        /// </summary>
        /// <param name="entity">IEntity</param>
        /// <param name="filter">Filter.</param>
        /// <returns>Async result.</returns>
        public virtual void Update(TEntity entity, Expression<Func<TEntity, bool>> filter)
        {
            try
            {
                Guard.IsNotNull(filter, "filter");

                DAO.Update(entity, filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Db update async.
        /// </summary>
        /// <param name="entity">IEntity</param>
        /// <param name="action">Addional value to bind</param>
        /// <param name="filter">Filter.</param>
        /// <returns>Async result.</returns>
        public virtual void Update(TEntity entity, Action<TEntity> action, Expression<Func<TEntity, bool>> filter)
        {
            try
            {
                Guard.IsNotNull(filter, "filter");

          

                action(entity);

                DAO.Update(entity, filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///  Async Db update async.
        /// </summary>
        /// <param name="entity">IEntity</param>
        /// <param name="filter">Filter.</param>
        /// <returns>Async result.</returns>
        public virtual Task UpdateAsync(TEntity entity, Expression<Func<TEntity, bool>> filter)
        {
            try
            {
                Guard.IsNotNull(filter, "filter");

          

                return DAO.UpdateAsync(entity, filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///  Async Db update async.
        /// </summary>
        /// <param name="entity">IEntity</param>
        /// <param name="action">Addional value to bind</param>
        /// <param name="filter">Filter.</param>
        /// <returns>Async result.</returns>
        public virtual Task UpdateAsync(TEntity entity, Action<TEntity> action, Expression<Func<TEntity, bool>> filter)
        {
            try
            {
                Guard.IsNotNull(filter, "filter");

                action(entity);

                return DAO.UpdateAsync(entity, filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Db update async.
        /// </summary>
        /// <param name="selector">Selector</param>
        /// <param name="filter">Filter</param>
        /// <returns>Async result.</returns>
        public virtual void Update(Expression<Func<TEntity, TEntity>> selector, Expression<Func<TEntity, bool>> filter)
        {
            try
            {
                Guard.IsNotNull(filter, "filter");

                DAO.Update(selector, filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Async Db update async.
        /// </summary>
        /// <param name="selector">Selector</param>
        /// <param name="filter">Filter</param>
        /// <returns>Async result.</returns>
        public virtual Task UpdateAsync(Expression<Func<TEntity, TEntity>> selector, Expression<Func<TEntity, bool>> filter)
        {
            try
            {
                Guard.IsNotNull(filter, "filter");

                return DAO.UpdateAsync(selector, filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Db update async.
        /// </summary>
        /// <param name="entity">IEntity</param>
        /// <param name="filter">Filter.</param>
        /// <returns>Async result.</returns>
        //public virtual void Post(TEntity entity, Expression<Func<TEntity, bool>> filter)
        //{
        //    try
        //    {
        //        Guard.IsNotNull(filter, "filter");
               
        //        entity.GetType().GetProperty("RecordStatus").SetValue(entity, (int)RecordStatus.Post);

        //        Update(entity, filter);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        /// <summary>
        /// Db delete async.
        /// </summary>
        /// <param name="filter">Filter.</param>
        /// <returns>Async result.</returns>
        public virtual void Delete(Expression<Func<TEntity, bool>> filter, string table = "")
        {
            try
            {
                Guard.IsNotNull(filter, "filter");

                DAO.Delete(filter, table);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Async Db delete async.
        /// </summary>
        /// <param name="filter">Filter.</param>
        /// <returns>Async result.</returns>
        public virtual Task DeleteAsync(Expression<Func<TEntity, bool>> filter, string table = "")
        {
            try
            {
                Guard.IsNotNull(filter, "filter");

                return DAO.DeleteAsync(filter, table);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Finds for data with both projection and filtering.
        /// </summary>
        /// <param name="selector">Selector.</param>
        /// <param name="filter">Filter.</param>
        /// <returns>Async result.</returns>
        public virtual IEnumerable<TEntity> Finds(Expression<Func<TEntity, dynamic>> selector = null, Expression<Func<TEntity, bool>> filter = null)
        {
            try
            {
                return DAO.Finds(selector, filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Async Finds for data with both projection and filtering.
        /// </summary>
        /// <param name="selector">Selector.</param>
        /// <param name="filter">Filter.</param>
        /// <returns>Async result.</returns>
        public virtual Task<IEnumerable<TEntity>> FindsAsync(Expression<Func<TEntity, dynamic>> selector = null, Expression<Func<TEntity, bool>> filter = null)
        {
            try
            {
                return DAO.FindsAsync(selector, filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual IEnumerable<TEntity> Finds(Expression<Func<TEntity, bool>> filter)
        {
            try
            {
                return DAO.Finds(filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual IEnumerable<TEntity> Finds(string expression)
        {
            try
            {
                return DAO.Finds(expression: expression);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual Task<IEnumerable<TEntity>> FindsAsync(string expression)
        {
            try
            {
                return DAO.FindsAsync(expression: expression);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual Expression<Func<T, bool>> FilterExpr<T>(string expression)
        {
            try
            {
                return DAO.FilterExpr<T>(expression: expression);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual IEnumerable<TEntity> Query(string query)
        {
            try
            {
                return DAO.Query(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Func<string, bool> CanReverseOrVoid;

        #endregion
    }

    public abstract class EntityLoggerBase<T> where T : EntityBase
    {
        public abstract void Subscribe(T OrigninSource, T UpdateSource, Action<LoggerContext> action);

        public class LoggerContext
        {
            public object OriginData { get; set; }
            public object NewData { get; set; }
            public string Description { get; set; }
            public string TableName { get; set; }
            public int LoggerType { get; set; }
        }
    }
}

