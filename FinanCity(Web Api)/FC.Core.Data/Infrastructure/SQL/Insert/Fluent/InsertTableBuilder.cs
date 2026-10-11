namespace FC.Core.Data.SQL.Fluent
{
    using FC.Core.Data.Infrastructure;

    public class InsertTableBuilder<TComponent> : QueryComponentBuilderBase<TComponent, InsertTableBuilder<TComponent>>, IInsertTable
      where TComponent : QueryComponentBase
    {
        public InsertTableBuilder(TComponent comoponent) : base(comoponent) { }

        public IQueryEnd Table<T>(string table = "") where T : IEntity
        {
            if (!string.IsNullOrWhiteSpace(table))
                Component.WriteTableName(table);
            else
                Component.WriteTableName<T>();

            return new QueryEndBuilder<TComponent>(Component);
        }
    }
}
