namespace FC.Core.Data.SQL.Fluent
{
    using System.Linq.Expressions;
    using FC.Core.Data.Infrastructure;
   
    public class InsertBuilder : QueryComponentBuilderBase<Insert, InsertBuilder> , IInsert
    {
        public InsertBuilder(Insert component) : base(component) { }

        public IInsertTable Insert<T>(Expression predicate) where T : IEntity
        {
            Component.WriteSelector<T>(predicate);

            return new InsertTableBuilder<Insert>(Component);
        }
    }
}
