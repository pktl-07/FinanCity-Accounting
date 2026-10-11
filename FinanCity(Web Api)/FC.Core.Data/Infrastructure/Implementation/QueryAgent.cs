namespace FC.Core.Data.Infrastructure.Implementation
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using Ninject;
    using FC.Core.Infrastructure;
    using Extensions;

    public class QueryAgent<T>
    {
        private IDictionary<string, IColumnMetadata> _dependencies;
        private IColumnMetadata _current;
        
        /// <summary>
        /// ORM generic tool to map with the table or criteria.
        /// </summary>
        /// <param name="dependencies"></param>
        public QueryAgent(IDictionary<string, IColumnMetadata> dependencies)
        {
            Guard.IsNotNull(dependencies, "dependencies");

            _dependencies = dependencies;
        }

        /// <summary>
        /// Initial setup ORM.
        /// </summary>
        /// <param name="field">Property.</param>
        /// <returns></returns>
        internal QueryAgent<T> Add(Expression<Func<T, object>> field)
        {
            Guard.IsNotNull(field, "field");
            Guard.IsNotDuplicateKey<string>(_dependencies.Keys, field.ToMemberExpression().Member.Name, "field");

            _current = Infrastructure.DependencyContainer.Current.Get<IColumnMetadata>();
            _dependencies.Add(field.ToMemberExpression().Member.Name, _current);

            return this;
        }

        /// <summary>
        /// References table and column.
        /// </summary>
        /// <typeparam name="TEntity">Type of table.</typeparam>
        /// <param name="reference">Type of column.</param>
        /// <returns></returns>
        public void Reference<TEntity>(Expression<Func<TEntity, object>> reference) where TEntity : class
        {
            Guard.IsNotNull(reference, "reference");
            var member = reference.ToMemberExpression().Member;

            (_current as ColumnMetadata).TableName = typeof(TEntity).GetTableAttribute().Name ?? typeof(TEntity).Name.EntityNamingConvention();
            (_current as ColumnMetadata).ColumnName = ((PropertyInfo)member).GetColumnAttribute().Name ?? member.Name;

            //return this;
        }

        /// <summary>
        /// Give custom table alias with column especially to use in union case.
        /// </summary>
        /// <param name="tbl_alias">Table alias</param>
        public void Custom(string tbl_alias = "tbl")
        {
            Guard.IsNotNull(tbl_alias, "table alias");

            (_current as ColumnMetadata).TableName = tbl_alias;
            (_current as ColumnMetadata).ColumnName = _dependencies.Keys.Last();
        }

        /// <summary>
        /// Give custom.
        /// </summary>
        public void Custom()
        {
            (_current as ColumnMetadata).TableName = null;
            (_current as ColumnMetadata).ColumnName = _dependencies.Keys.Last();
        }
    }
}
