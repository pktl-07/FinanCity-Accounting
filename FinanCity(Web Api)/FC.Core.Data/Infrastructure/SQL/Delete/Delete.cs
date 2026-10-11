namespace FC.Core.Data.SQL
{
    using System.Linq.Expressions;
    using FC.Core.Data.Infrastructure;

    public class Delete : QueryComponentBase
    {
        public Delete(QueryType type = QueryType.Delete) : base(type) { }

        public override string ToString()
        {
            return QueryContext.Append($" { Table } WHERE { Where } ").ToString();
        }
    }
}
