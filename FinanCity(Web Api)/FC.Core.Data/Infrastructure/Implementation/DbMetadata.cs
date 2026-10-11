namespace FC.Core.Data.Infrastructure.Implementation
{
    /// <summary>
    /// Db meta data. This class cann't be override.
    /// </summary>
    public sealed class DbMetadata : IDbMetadata
    {
        /// <summary>
        /// Connection key.
        /// </summary>
        public string KEY { get; internal set; }

        /// <summary>
        /// Connection string.
        /// </summary>
        public string ConnectionString { get; internal set; }

        /// <summary>
        /// Db provider.
        /// </summary>
        public DbProvider Provider { get; internal set; }

        /// <summary>
        /// Table Name unless you use table attribute.
        /// </summary>
        public string TableName { get; set; }
    }
}
