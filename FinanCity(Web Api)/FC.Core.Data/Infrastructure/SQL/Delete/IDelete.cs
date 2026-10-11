namespace FC.Core.Data.SQL
{
    public interface IDelete
    {
        ICommitableTable Delete<T>() where T : IEntity;
    }
}
