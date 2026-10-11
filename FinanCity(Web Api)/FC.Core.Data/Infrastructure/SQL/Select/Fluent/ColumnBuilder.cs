namespace FC.Core.Data.SQL.Fluent
{
    using FC.Core.Data.Infrastructure;

    public class ColumnBuilder<TComponent> : QueryComponentBuilderBase<TComponent, ColumnBuilder<TComponent>> , IColumn
        where TComponent : QueryComponentBase
    {
        public ColumnBuilder(TComponent compoent) : base(compoent) { }
       
        public ITable Table<T>() where T : IEntity
        {
            Component.WriteTableName<T>();

            return new TableBuilder<TComponent, T>(Component);
        }

        public IQueryEnd Value()
        {
            return new QueryEndBuilder<TComponent>(Component);
        }
    }
}
