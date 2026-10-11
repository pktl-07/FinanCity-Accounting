namespace FC.Core.Data.Infrastructure.Implementation
{
    /// <summary>
    /// Db column metadata. This class can not be override.
    /// </summary>
    public sealed class ColumnMetadata : IColumnMetadata
    {
        /// <summary>
        ///Db column name. 
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// Db table name.
        /// </summary>
        public string TableName { get; set; }
    }
}
