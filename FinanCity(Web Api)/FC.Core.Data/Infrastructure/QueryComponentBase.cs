namespace FC.Core.Data.Infrastructure
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using FC.Core.Data.Extensions;
    using FC.Core.Data.Infrastructure.Implementation;

    /// <summary>
    /// Base SQL statement.
    /// </summary>
    public abstract class QueryComponentBase
    {
        #region Fields

        private ExceptorStrategy<string> _exceptor;
        protected string Join { get; set; }
        protected string Field { get; set; }
        protected string Group { get; set; }
        protected string Table { get; set; }
        protected string Where { get; set; }
        protected string Having { get; set; }
        protected string OrderBy { get; set; }
        protected StringBuilder QueryContext { get; set; }
        public QueryComponentBase(QueryType queryType)
        {
            QueryContext = new StringBuilder(queryType.ToDescription());
        }

        #endregion

        #region Methods

        /// <summary>
        /// Write appendix where.
        /// </summary>
        /// <param name="query"></param>
        public virtual void WrieAppendixWhere(string query)
        {
            // make sure to prevent error if starting value like 'AND' or 'OR' when WHERE caluse start with empty.
            if (string.IsNullOrWhiteSpace(Where))
            {
                query.Trim().TrimStart('A', 'N', 'D');
                query.Trim().TrimStart('O', 'R');
                Where += query;
                return;
            }
            else
            {
                if (query.Trim().StartsWith("AND") || query.Trim().StartsWith("OR"))
                {
                    Where += query;
                    return;
                }
            }
            // if there is no satify condition put default 'AND' conjuntion.
            Where += " AND " + query;
        }

        /// <summary>
        /// Write select statement.
        /// </summary>
        /// <typeparam name="T">IEntity</typeparam>
        /// <param name="expr"></param>
        public virtual void WriteSelector<T>(Expression expr) where T : IEntity
        {
            var fields = (new ReaderExpressionVisitor<T>(expr ?? defaultMember<T>(), true) as IExpressionSelector).FieldCollection;
            fields.Each(field =>
            {
                Field += field.AddChar(',');
            });
        }

        /// <summary>
        /// Write table name.
        /// </summary>
        /// <typeparam name="T">IEntity</typeparam>
        public virtual void WriteTableName(string table)
        {
            Table = table;
            _exceptor = new ExceptorStrategy<string>(true, Table);
        }

        /// <summary>
        /// Write table statement.
        /// </summary>
        /// <typeparam name="T">IEntity</typeparam>
        public virtual void WriteTableName<T>() where T : IEntity
        {
            WriteTableName(Activator.CreateInstance<T>().GetTableName());
        }

        /// <summary>
        /// Write table statement.
        /// </summary>
        /// <typeparam name="T">IEntity</typeparam>
        public virtual void WriteTableName<T>(IEntity table) where T : IEntity
        {
            WriteTableName(table.GetMetadata().TableName);
        }

        /// <summary>
        /// Write where statement.
        /// </summary>
        /// <typeparam name="T">IEntity</typeparam>
        /// <param name="expr"></param>
        public virtual void WriteWhere<T>(Expression expr) where T : IEntity
        {
            var where = (new FilterExpressionVistor<T>(expr) as IExpressionFilter).WHERE.ToString();
            if (!string.IsNullOrWhiteSpace(where))
                Where = where;
        }

        /// <summary>
        /// Write Join statement.
        /// </summary>
        /// <typeparam name="T">Entity.</typeparam>
        /// <typeparam name="U">Entity.</typeparam>
        /// <param name="joinType">Join type enum.</param>
        /// <param name="expr">Expression.</param>
        public virtual void WriteJoin<T, U>(JoinType joinType, Expression expr)
            where T : IEntity
            where U : IEntity
        {
            var type = $" {joinType.ToDescription()} ";
            var tableName = _exceptor.GetCompareResult(Activator.CreateInstance<T>().GetTableName(), Activator.CreateInstance<U>().GetTableName());
            var how = (new FilterExpressionVistor<T>(expr) as IExpressionFilter).WHERE.ToString();
            Join += $" {type} {tableName.First()} ON {how}";
        }

        /// <summary>
        /// Write group by statement.
        /// </summary>
        /// <typeparam name="T">IEntity</typeparam>
        /// <param name="expr"></param>
        public virtual void WriteGroupBy<T>(Expression expr) where T : IEntity
        {
            (new ReaderExpressionVisitor<T>(expr, false) as IExpressionSelector).FieldCollection.Each(gr =>
            {
                Group += gr.AddChar(',');
            });
        }

        /// <summary>
        /// Runtime auto detact when all complete built statement and pass it to final ToString().
        /// </summary>
        /// <returns></returns>
        public virtual string ToQueryComponent()
        {
            return ToString();
        }

        /// <summary>
        /// Consturct SQL statement.
        /// </summary>
        /// <returns>SQL statement.</returns>
        public override string ToString()
        {
            if (QueryContext.ToString().Contains("SELECT FROM"))
                QueryContext.Append(" * ");
            else
                QueryContext.Append($" {Field.RemoveLastChar(',')}");

            QueryContext.Append(" FROM ");
            QueryContext.Append(Table);

            if (!string.IsNullOrEmpty(Join))
                QueryContext.Append(Join);

            if (!string.IsNullOrEmpty(Where))
                QueryContext.Append($" WHERE {Where}");

            if (!string.IsNullOrEmpty(Having))
                QueryContext.Append($" HAVING {Having.RemoveLastChar(',')}");

            if (!string.IsNullOrEmpty(Group))
                QueryContext.Append($" GROUP BY { Group.RemoveLastChar(',') }");

            if (!string.IsNullOrWhiteSpace(OrderBy))
                QueryContext.Append($" ORDER BY { OrderBy.RemoveLastChar(',') }");

            return QueryContext.ToString();
        }

        #endregion

        #region Utilities

        private Expression<Func<T, dynamic>> defaultMember<T>()
        {
            var type = typeof(T);
            var arg = Expression.Parameter(type, "x");
            return Expression.Lambda<Func<T, dynamic>>(arg, arg);
        }

        #endregion
    }
}
