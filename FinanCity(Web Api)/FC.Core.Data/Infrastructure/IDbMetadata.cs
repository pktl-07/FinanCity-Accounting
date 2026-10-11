namespace FC.Core.Data.Infrastructure
{
    public interface IDbMetadata
    {
        /// <summary>
        /// Get connection key.
        /// </summary>
        string KEY { get; }

        /// <summary>
        /// Get Db connection string.
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// Get Db provider type.
        /// </summary>
        DbProvider Provider { get; }

        /// <summary>
        /// Get Table name unless you use table attribute.
        /// </summary>
        string TableName { get; set; }
    }
}
