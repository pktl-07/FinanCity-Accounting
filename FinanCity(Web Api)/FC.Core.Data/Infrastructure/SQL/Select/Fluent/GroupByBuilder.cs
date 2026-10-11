namespace FC.Core.Data.SQL.Fluent
{
    using System;
    using FC.Core.Data.Infrastructure;

    public class GroupByBuilder<TComponent> : QueryComponentBuilderBase<TComponent,GroupByBuilder<TComponent>>, IGroupBy
        where TComponent : QueryComponentBase
    {
        public GroupByBuilder(TComponent component) : base(component) { }
        
        IGroupBy IGroupBy.GroupBy<T>(System.Linq.Expressions.Expression<Func<T, dynamic>> predicate)
        {
            Component.WriteGroupBy<T>(predicate);
            return this;
        }

        public IOrderBy OrderBy<T>(System.Linq.Expressions.Expression<Func<T, dynamic>> predicate)
            where T : IEntity 
        {
            return new OrderByBuilder(Component as Select).OrderBy(predicate, OrderType.ASC);
        }

        public IOrderBy OrderBy<T>(System.Linq.Expressions.Expression<Func<T, dynamic>> predicate, OrderType order)
            where T : IEntity
        {
            return new OrderByBuilder(Component as Select).OrderBy(predicate, order);
        }
    }
}
