namespace FC.Core.Data.SQL.Fluent
{
    using FC.Core.Data.Infrastructure;

    public class DeleteBuilder : QueryComponentBuilderBase<Delete, DeleteBuilder>, IDelete
    {
        public DeleteBuilder(Delete component) : base(component) { }

        public ICommitableTable Delete<T>() where T : IEntity
        {
            return new CommitableTableBuilder<Delete>(Component);
        }
    }
}
