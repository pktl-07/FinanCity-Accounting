namespace FC.Core.Data.SQL
{
    using System.Linq;
    using System.Linq.Expressions;
    using FC.Core.Data.Infrastructure;
    using FC.Core.Data.Infrastructure.Implementation;

    public class Select : QueryComponentBase
    {
        public Select(QueryType type = QueryType.Select) : base(type) { }

        public void WriteSubQuery(string query)
        {
            var list = Field.RemoveLastChar(',').Split(',').ToList();

            list.RemoveAt(list.Count - 1);
            list.Add(query);
            Field = ""; // clear field;
            list.Each(field =>
            {
                Field += field.AddChar(',');
            });
        }
        
        public void WriteAggregate<TEntity>(Expression expression, AggregrateType type)
            where TEntity : IEntity
        {
            var fieldName = ((expression as LambdaExpression).Body as MemberExpression).Member.Name;
            var actual = (new ReaderExpressionVisitor<TEntity>(expression, false) as IExpressionSelector).FieldCollection.Single();
            Field += $"{type.ToDescription()}({actual}) [{fieldName}]".AddChar(',');
        }

        public void WriteHavingAppendix(string query)
        {
            Having += query.AddChar(',');
        }

        public void WriteOrderBy<TEntity>(Expression expression, OrderType type)
            where TEntity : IEntity
        {
            var actual = (new ReaderExpressionVisitor<TEntity>(expression, false) as IExpressionSelector).FieldCollection.Single();
            OrderBy += $"{actual} { type.ToDescription()}".AddChar(',');
        }
    }
}
