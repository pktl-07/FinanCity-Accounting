namespace FC.Core.Data.SQL.Fluent
{
    using System;
    using System.Linq.Expressions;
    using FC.Core.Data.Infrastructure;

    public class CommitableWhereBuilder<TComponent> : QueryComponentBuilderBase<TComponent, CommitableTableBuilder<TComponent>>, ICommitableWhere
      where TComponent : QueryComponentBase
    {
        public CommitableWhereBuilder(TComponent component) : base(component) { }

        public IQueryEnd Where<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : IEntity
        {
            Component.WriteWhere<TEntity>(expression);

            return new QueryEndBuilder<TComponent>(Component);
        }
    }
}
