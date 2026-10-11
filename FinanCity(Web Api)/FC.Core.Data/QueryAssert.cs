namespace FC.Core.Data
{
    using Ninject;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using FC.Core.Data.Infrastructure;
    using FC.Core.Data.Infrastructure.Implementation;
  
    /// <summary>
    /// Data table and criteria base for query.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class QueryAssert<T> : IQueryAssert 
        where T : QueryAssert<T> 
    {
        private QueryAgent<T> _rule;
        private IDictionary<string, IColumnMetadata> _columnMetadata;
        private IDbMetadata _dbMetadata;
        
        public QueryAssert(string conn) : this(conn, DependencyContainer.Current.Get<IDbMetadata>()) { }

        internal QueryAssert(string conn, IDbMetadata metadata)
        {
            _dbMetadata = metadata;
            (_dbMetadata as DbMetadata).KEY = conn;
            (_dbMetadata as DbMetadata).ConnectionString = DependencyContainer.Current.Get<IEssDataRepository>().DbConnection[conn].ConnectionString;
            (_dbMetadata as DbMetadata).Provider = DependencyContainer.Current.Get<IEssDataRepository>().DbConnection[conn].Provider;
            _columnMetadata = new Dictionary<string, IColumnMetadata>();
            _rule = new QueryAgent<T>(_columnMetadata);
        }

        /// <summary>
        /// To map object relation.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        protected QueryAgent<T> RuleFor(Expression<Func<T, object>> field)
        {
            return _rule.Add(field);
        }

        /// <summary>
        /// Read only db column meta data.
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, IColumnMetadata> GetMetadata()
        {
            return _columnMetadata;
        }

        /// <summary>
        /// Read only db meta data such as connection register key, connection string and its provider.
        /// </summary>
        /// <returns></returns>
        IDbMetadata IEntity.GetMetadata()
        {
            return _dbMetadata;
        }
    }
}
