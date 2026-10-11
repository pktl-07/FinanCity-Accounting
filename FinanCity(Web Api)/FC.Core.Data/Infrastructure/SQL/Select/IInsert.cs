namespace FC.Core.Data.SQL
{
    using System.Linq.Expressions;

    public interface IInsert
    {
        IInsertTable Insert<T>(Expression predicate) where T : IEntity ;
    }
}
