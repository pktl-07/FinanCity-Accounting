namespace FC.Core.Data.SQL
{
    public interface IColumn
    {
        ITable Table<T>() where T : IEntity;
    }
}
