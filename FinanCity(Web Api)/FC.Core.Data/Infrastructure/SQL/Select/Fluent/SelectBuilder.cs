namespace FC.Core.Data.SQL.Fluent
{
    using System;
    using System.Linq.Expressions;
    using FC.Core.Data.Infrastructure;

    public class SelectBuilder : QueryComponentBuilderBase<Select, SelectBuilder>, ISelect
    {
        public SelectBuilder(Select component) : 
            base(component) { }

        public IColumn Select<T>(Action<ColumnFacotry<T>> columns) where T : IEntity
        {
            columns(new ColumnFacotry<T>(Component));

            return new ColumnBuilder<Select>(Component);
        }

        public IColumn Select<T>(Expression<Func<T, dynamic>> predicate) where T : IEntity
        {
            Component.WriteSelector<T>(predicate);

            return new ColumnBuilder<Select>(Component);
        }

        public IColumn Select<T>(Expression<Func<T, dynamic>> predicate, Action<ColumnFacotry<T>> columns) where T : IEntity
        {
            columns(new ColumnFacotry<T>(Component));

            return new ColumnBuilder<Select>(Component);
        }
    }
}
