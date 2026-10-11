namespace FC.Core.Data.SQL.Fluent
{
    using System;
    using System.Linq.Expressions;
    using FC.Core.Data.Infrastructure;

    public class OrderByBuilder : QueryComponentBuilderBase<Select,OrderByBuilder> , IOrderBy
    {
        public OrderByBuilder(Select component) : base(component) { }

        public IOrderBy OrderBy<T>(Expression<Func<T, dynamic>> predicate)
            where T : IEntity
        {
            OrderBy(predicate, OrderType.ASC);
            return this;
        }

        public IOrderBy OrderBy<T>(Expression<Func<T, dynamic>> predicate, OrderType order)
            where T : IEntity
        {
            Component.WriteOrderBy<T>(predicate, order);
            return this;
        }
    }
}
