namespace FC.Core.Data.Infrastructure
{
    /// <summary>
    /// Entiy readonly logger.
    /// </summary>
    public interface IQueryMetadata : IDbMetadata
    {
        string Key { get; }
        
        string FieldName { get; }
    }
}
