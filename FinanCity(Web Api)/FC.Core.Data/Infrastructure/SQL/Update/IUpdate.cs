namespace FC.Core.Data.SQL
{
    using System.Linq.Expressions;

    public interface IUpdate
    {
        ICommitableTable Update<T>(Expression predicate) where T : IEntity;
    }
}
