namespace FC.Core.Data.SQL
{
    using System.Linq.Expressions;
    using FC.Core.Data.Infrastructure;
    using FC.Core.Data.Infrastructure.Implementation;
    using FC.Core.Data.Extensions;

    public class Update : QueryComponentBase
    {
        public delegate void FetchQuery(string query);
        public event FetchQuery OnFetchQuery;

        public Update(QueryType type = QueryType.Update) : base(type) { }

        public override void WriteSelector<T>(Expression expr)
        {
           
            var fieldCollection = (new WriterExpressionVisitor<T>(expr) as IExpressionSelector).FieldCollection;

            fieldCollection.UpdateValidate();
            fieldCollection.Each(field =>
            {
                Field += field.AddChar(',');
            });
            Field = Field.RemoveLastChar(',');
        }

        public override string ToString()
        {
            var query = QueryContext.Append($" { Table } SET { Field } WHERE { Where.Replace("[].", $"[{Table}].") } ").ToString();

            if (OnFetchQuery != null)
                OnFetchQuery(query);

            return query;
        }
    }
}
