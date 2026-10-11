namespace FC.Core.Data.Infrastructure
{
    /// <summary>
    /// Entity column metadata.
    /// </summary>
    public interface IColumnMetadata
    {
        string ColumnName { get; }

        string TableName { get; }
    }
}
