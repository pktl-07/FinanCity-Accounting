namespace FC.Core.Data
{
    using System;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using FC.Core.Infrastructure;
    using FC.Core.Data.SQL;
    using Extensions;

    public abstract class DAO
    {
        /// <summary>
        /// To get field name.
        /// </summary>
        /// <typeparam name="T">IEntity</typeparam>
        /// <param name="expression">Expression selector.</param>
        /// <returns>Field name.</returns>
        protected static string With<T>(Expression<Func<T, object>> expression) where T : IEntity
        {
            return $" {expression.ToMemberExpression().Member.Name.GetFullSQLFieldName(Activator.CreateInstance<T>())} ";
        }

        /// <summary>
        /// To get table name.
        /// </summary>
        /// <typeparam name="T">IEntity.</typeparam>
        /// <returns>Table name.</returns>
        protected static string Table<T>() where T : IEntity
        {
            return $" [{Activator.CreateInstance<T>().GetTableName()}] ";
        }

        /// <summary>
        /// Find command for specific table.
        /// </summary>
        /// <param name="selector">Selector.</param>
        /// <param name="filter">Filter.</param>
        /// <returns>Async result.</returns>
        public IEnumerable<TEntity> Finds<TEntity>(Expression<Func<TEntity, dynamic>> selector = null,
            Expression<Func<TEntity, bool>> filter = null)
            where TEntity : IEntity
        {
            try
            {
                return DbExcutor<TEntity>.ExcuteReader(XQuery.Select(selector).Table<TEntity>().Where(filter).ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        /// <summary>
        /// Async Find command for specific table.
        /// </summary>
        /// <param name="selector">Selector.</param>
        /// <param name="filter">Filter.</param>
        /// <returns>Async result.</returns>
        public Task<IEnumerable<TEntity>> FindsAsync<TEntity>(Expression<Func<TEntity, dynamic>> selector = null, Expression<Func<TEntity, bool>> filter = null)
            where TEntity : IEntity
        {
            try
            {
                return DbExcutor<TEntity>.ExcuteReaderAsync(XQuery.Select(selector).Table<TEntity>().Where(filter).ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
      
        public Expression<Func<T, bool>> FilterExpr<T>(string expression)
        {
            if (!string.IsNullOrWhiteSpace(expression) && !expression.Equals("''"))
            {
                var replace = expression.Replace("'", "\"").Replace("'", "\"").Trim();

                return System.Linq.Dynamic.DynamicExpression.ParseLambda<T, bool>(replace, new[] { Expression.Parameter(typeof(T), typeof(T).Name) });
            }

            return null;
        }

        /// <summary>
        /// Get specific pager from depending on T type.
        /// </summary>
        /// <typeparam name="T">IEntity</typeparam>
        /// <param name="query">SQL Query</param>
        /// <param name="currentPage">Current page</param>
        /// <param name="pageSize">Total page size</param>
        /// <returns></returns>
        public IDataPagerResult<T> ExcutePagerResult<T>(string query, int currentPage, int pageSize)
            where T : IEntity
        {
            try
            {
                if (string.IsNullOrEmpty(query))
                    Guard.IsNotNullOrEmpty(query, "query");

                return new DataPager<T>(query, currentPage, pageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<TEntity> Query<TEntity>(string query)
            where TEntity : IEntity
        {
            try
            {
                Guard.IsNotNullOrEmpty(query, "query");

                return DbExcutor<TEntity>.ExcuteReader(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private class DataPager<TEntity> : IDataPagerResult<TEntity> where TEntity : IEntity
        {
            private string _query;
            private int _currentPage, _pageSize;

            public DataPager(string query, int currentPage, int pageSize)
            {
                _query = query;
                _currentPage = currentPage;
                _pageSize = pageSize;
            }

            private IEnumerable<TEntity> _pagingData;
            public IEnumerable<TEntity> Data
            {
                get
                {
                    try
                    {
                        if (_pagingData != null)
                            return _pagingData;
                        else
                        {
                            Validate();
                            var query = _query + " OFFSET " + ((_currentPage - 1) * _pageSize) + " ROWS FETCH NEXT " + _pageSize + " ROWS ONLY";
                            return DbExcutor<TEntity>.ExcuteReader(query);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                set
                {
                    _pagingData = value;
                }
            }

            public int Total
            {
                get
                {
                    try
                    {
                        Validate();

                        var removeOrderClause = _query.Split(new string[] { "ORDER" }, StringSplitOptions.None)[0];
                        var query = " SELECT COUNT(*) FROM (" + removeOrderClause + ") [PagerTbl]";

                        return (int)DbExcutor<TEntity>.ExcuteScalar(query);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }

            private void Validate()
            {
                if (string.IsNullOrWhiteSpace(_query) || string.IsNullOrEmpty(_query))
                    Guard.IsNotNullOrEmpty(_query, "_query");

                if (!_query.Contains("ORDER"))
                    throw new Exception("ORDER BY clause is required for pager list");
            }
        }
    }

    /// <summary>
    /// Base Data Access Object.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class DAO<TEntity> : DAO, ILayerServices<TEntity>,
        IHideObjectMembers where TEntity : IEntity
    {
        #region Methods

        /// <summary>
        /// Insert command for specific table.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Async result.</returns>
        public virtual TEntity Insert(TEntity entity)
        {
            try
            {
                Expression<Func<TEntity, TEntity>> expr = (x) => entity;

                return DbExcutor<TEntity>.ExcuteNonQuery(XQuery.Insert(expr).Table<TEntity>(entity.GetMetadata().TableName).ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Async Insert command for specific table.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Async result.</returns>
        public virtual Task<TEntity> InsertAsync(TEntity entity)
        {
            try
            {
                Expression<Func<TEntity, TEntity>> expr = (x) => entity;

                return DbExcutor<TEntity>.ExcuteNonQueryAsync(XQuery
                    .Insert(expr)
                    .Table<TEntity>(entity.GetMetadata().TableName)
                    .ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Async Insert command for specific table.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Async result.</returns>
        public Task<TEntity> InsertAsync(TEntity entity, Action<TEntity> action)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Insert command specific table. 
        /// </summary>
        /// <param name="selector">Selector.</param>
        /// <returns>Async result.</returns>
        public TEntity Insert(Expression<Func<TEntity, TEntity>> selector)
        {
            try
            {
                var table = RetrieveTableName(selector);

                return DbExcutor<TEntity>.ExcuteNonQuery(XQuery.Insert(selector)
                    .Table<TEntity>(table: table)
                    .ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Async Insert command for specific table. 
        /// </summary>
        /// <param name="selector">Selector.</param>
        /// <returns>Async result.</returns>
        public virtual Task<TEntity> InsertAsync(Expression<Func<TEntity, TEntity>> selector)
        {
            try
            {
                return DbExcutor<TEntity>.ExcuteNonQueryAsync(XQuery.Insert(selector).Table<TEntity>().ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Bulk insert command for specific table.
        /// </summary>
        /// <param name="entities">IEnumerable IEntity</param>
        /// <returns></returns>
        public virtual void BulkInsert(IEnumerable<TEntity> entities)
        {
            try
            {
                foreach (var entity in entities)
                {
                    Insert(x => entity);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Async Bulk command insert for specific table.
        /// </summary>
        /// <param name="entities">IEnumerable IEntity</param>
        /// <returns></returns>
        public virtual Task BulkInsertAsync(IEnumerable<TEntity> entities)
        {
            try
            {
                foreach (var entity in entities)
                {
                    InsertAsync(x => entity);
                }
                return Task.FromResult(0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Update command for specific table.
        /// </summary>
        /// <param name="entity">IEntity</param>
        /// <param name="filter">Filter</param>
        /// <returns></returns>
        public virtual void Update(TEntity entity, Expression<Func<TEntity, bool>> filter)
        {
            try
            {
                Expression<Func<TEntity, TEntity>> expr = (x) => entity;

                DbExcutor<TEntity>.ExcuteNonQuery(XQuery.Update(expr)
                    .Table<TEntity>(entity.GetMetadata().TableName)
                    .Where(filter).ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual void Update(TEntity entity, Action<TEntity> action, Expression<Func<TEntity, bool>> filter)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Async Update command for specific table.
        /// </summary>
        /// <param name="entity">IEntity</param>
        /// <param name="filter">Filter</param>
        /// <returns></returns>
        public virtual Task UpdateAsync(TEntity entity, Expression<Func<TEntity, bool>> filter)
        {
            try
            {
                Expression<Func<TEntity, TEntity>> expr = (x) => entity;

                return DbExcutor<TEntity>
                    .ExcuteNonQueryAsync(XQuery.Update(expr)
                    .Table<TEntity>(entity.GetMetadata().TableName)
                    .Where(filter).ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual Task UpdateAsync(TEntity entity, Action<TEntity> action, Expression<Func<TEntity, bool>> filter)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Update command for specific table.
        /// </summary>
        /// <param name="selector">Selector.</param>
        /// <param name="filter">Filter.</param>
        /// <returns></returns>
        public void Update(Expression<Func<TEntity, TEntity>> selector,
            Expression<Func<TEntity, bool>> filter)
        {
            try
            {
                Guard.IsNotNull(selector, "selector");
                Guard.IsNotNull(filter, "filter");

                DbExcutor<TEntity>.ExcuteNonQuery(XQuery.Update(selector).Table<TEntity>().Where(filter).ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Async Update command for specific table.
        /// </summary>
        /// <param name="selector">Selector.</param>
        /// <param name="filter">Filter.</param>
        /// <returns></returns>
        public virtual Task UpdateAsync(Expression<Func<TEntity, TEntity>> selector, Expression<Func<TEntity, bool>> filter)
        {
            try
            {
                Guard.IsNotNull(selector, "selector");
                Guard.IsNotNull(filter, "filter");

                return DbExcutor<TEntity>.ExcuteNonQueryAsync(XQuery.Update(selector).Table<TEntity>().Where(filter).ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Delete command for specific table.
        /// </summary>
        /// <param name="filter">Filter.</param>
        /// <returns>Async result.</returns>
        public virtual void Delete(Expression<Func<TEntity, bool>> filter, string table = "")
        {
            try
            {
                Guard.IsNotNull(filter, "filter");

                DbExcutor<TEntity>.ExcuteNonQuery(XQuery.Delete<TEntity>().Table<TEntity>(table).Where(filter).ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Delete command for specific table.
        /// </summary>
        /// <param name="filter">Filter.</param>
        /// <returns>Async result.</returns>
        public virtual Task DeleteAsync(Expression<Func<TEntity, bool>> filter, string table = "")
        {
            try
            {
                Guard.IsNotNull(filter, "filter");

                return DbExcutor<TEntity>.ExcuteNonQueryAsync(XQuery.Delete<TEntity>().Table<TEntity>(table).Where(filter).ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual IEnumerable<TEntity> Finds(Expression<Func<TEntity, dynamic>> selector = null, 
            Expression<Func<TEntity, bool>> filter = null)
        {
            try
            {
                return base.Finds(selector: selector, filter: filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual Task<IEnumerable<TEntity>> FindsAsync(Expression<Func<TEntity, dynamic>> selector = null, 
            Expression<Func<TEntity, bool>> filter = null)
        {
            try
            {
                return base.FindsAsync(selector: selector, filter: filter);
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
                return Finds(selector: null, filter: filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Find from expression criteria.
        /// </summary>
        /// <param name="filter">Filter expression string.</param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> Finds(string expression)
        {
            try
            {
                if (string.IsNullOrEmpty(expression))
                    return Finds(selector: null, filter: null);

                return Finds(filter: FilterExpr<TEntity>(expression));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Async Find from expression criteria.
        /// </summary>
        /// <param name="filter">Filter expression string.</param>
        /// <returns></returns>
        public virtual Task<IEnumerable<TEntity>> FindsAsync(string expression)
        {
            try
            {
                if (string.IsNullOrEmpty(expression))
                    return FindsAsync();

                return FindsAsync(filter: FilterExpr<TEntity>(expression));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual void ExcuteProc(string procedureName, string connection, IDictionary<string, object> dbparams)
        {
            try
            {
                Guard.IsNotNullOrEmpty(dbparams, "dbparams");

                DbExcutor.ExcuteProc(procedureName, connection, dbparams.ToDbParameters(connection));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual void ExcuteProc(string procedureName, string connection)
        {
            try
            {
                DbExcutor.ExcuteProc(procedureName, connection);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual IEnumerable<TEntity> Query(string query)
        {
            return Query<TEntity>(query: query);
        }

        #endregion

        #region Utilities

        private string RetrieveTableName(Expression<Func<TEntity, TEntity>> selector)
        {
            try
            {
                var instance = Expression.Lambda<Func<TEntity, TEntity>>((selector as LambdaExpression).Body, (selector as LambdaExpression).Parameters).Compile()(Activator.CreateInstance<TEntity>());
                var table = instance.GetMetadata().TableName;

                if (!string.IsNullOrWhiteSpace(table))
                {
                    return table;
                }

                return instance.GetTableName();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
