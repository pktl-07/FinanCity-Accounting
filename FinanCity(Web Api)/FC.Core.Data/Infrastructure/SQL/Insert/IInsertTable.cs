namespace FC.Core.Data.SQL
{
    public interface IInsertTable
    {
        IQueryEnd Table<T>(string table = "") where T : IEntity;
    }
}
