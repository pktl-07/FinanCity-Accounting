namespace FC.Core.Data.SQL.Fluent
{
    using System;
    using FC.Core.Data.Infrastructure;

    public class HavingBuilder<TComponent> : QueryComponentBuilderBase<TComponent,HavingBuilder<TComponent>>, IHaving
        where TComponent : QueryComponentBase
    {
        public HavingBuilder(TComponent component) : base(component) { }
        
        public IGroupBy GroupBy<T>(System.Linq.Expressions.Expression<Func<T, dynamic>> predicate)
            where T : IEntity
        {
            return (new GroupByBuilder<TComponent>(Component) as IGroupBy).GroupBy<T>(predicate);
        }

        IGroupBy IHaving.Having(Action<IAppendix> action)
        {
            var template = new HavingAppendixBuilder(Component as Select);
            action(template);
            return new GroupByBuilder<TComponent>(Component);
        }

        public IOrderBy OrderBy<T>(System.Linq.Expressions.Expression<Func<T, dynamic>> predicate)
            where T : IEntity
        {
            return new OrderByBuilder(Component as Select).OrderBy<T>(predicate);
        }

        public IOrderBy OrderBy<T>(System.Linq.Expressions.Expression<Func<T, dynamic>> predicate, OrderType order)
            where T : IEntity
        {
            return new OrderByBuilder(Component as Select).OrderBy(predicate, order);
        }
    }
}
