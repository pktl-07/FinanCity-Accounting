namespace FC.Core.Data.SQL.Fluent
{
    using FC.Core.Data.Infrastructure;

    public class QueryEndBuilder<TComponent> : QueryComponentBuilderBase<TComponent, WhereBuilder<TComponent>>, IQueryEnd
        where TComponent : QueryComponentBase
    {
        public QueryEndBuilder(TComponent component) : base(component) { }
    }
}
