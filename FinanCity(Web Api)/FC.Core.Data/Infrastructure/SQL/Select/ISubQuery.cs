namespace FC.Core.Data.SQL
{
    public interface ISubQuery<TEntity>
        where TEntity : IEntity
    {
        void SubQuery(string query);
    }
}
