namespace FC.Core.Data.SQL.Fluent
{
    using System;
    using System.Linq.Expressions;
    using FC.Core.Data.Infrastructure;

    public class WhereBuilder<TComponent> : QueryComponentBuilderBase<TComponent, WhereBuilder<TComponent>>, IWhere
        where TComponent : QueryComponentBase
    {
        public WhereBuilder(TComponent component) : base(component) { }

        IHaving IWhere.Where<TEntity>(Expression<Func<TEntity, bool>> predicate)
        {
            Component.WriteWhere<TEntity>(predicate);

            return new HavingBuilder<TComponent>(Component);
        }

        IHaving IWhere.Where<TEntity>(Expression<Func<TEntity, bool>> expression, Action<IAppendix> action)
        {
            Component.WriteWhere<TEntity>(expression);

            var template = new WhereAppendixBuilder(Component);

            action(template);

            return new HavingBuilder<TComponent>(Component);
        }
    }
}
