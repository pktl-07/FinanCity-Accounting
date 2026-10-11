namespace FC.Core.Data.SQL
{
    public interface ICommitableTable
    {
        ICommitableWhere Table<T>(string table = "") where T : IEntity;
    }
}
