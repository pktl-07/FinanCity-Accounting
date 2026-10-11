namespace FC.Core.Data.SQL.Fluent
{
    using FC.Core.Data.Infrastructure;

    public class CommitableTableBuilder<TComponent> : QueryComponentBuilderBase<TComponent, CommitableTableBuilder<TComponent>>, ICommitableTable
      where TComponent : QueryComponentBase
    {
        public CommitableTableBuilder(TComponent component) : base(component) { }

        public ICommitableWhere Table<T>(string table) where T : IEntity
        {
            if (!string.IsNullOrWhiteSpace(table))
                Component.WriteTableName(table);
            else
                Component.WriteTableName<T>();

            return new CommitableWhereBuilder<TComponent>(Component);
        }
    }
}
