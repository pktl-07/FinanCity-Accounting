namespace FC.Core.Data.SQL.Fluent
{
    using System.Linq.Expressions;
    using FC.Core.Data.Infrastructure;

    public class UpdateBuilder : QueryComponentBuilderBase<Update, UpdateBuilder>, IUpdate
    {
        public UpdateBuilder(Update component) : base(component) { }

        public ICommitableTable Update<T>(Expression predicate) where T : IEntity
        {
            Component.WriteSelector<T>(predicate);

            return new CommitableTableBuilder<Update>(Component);
        }
    }
}
