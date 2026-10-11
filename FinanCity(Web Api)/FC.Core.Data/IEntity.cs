namespace FC.Core.Data
{
    using FC.Core.Data.Infrastructure;

    /// <summary>
    /// Base entity.
    /// </summary>
    public interface IEntity 
    {
        /// <summary>
        /// Get metadata context.
        /// </summary>
        IDbMetadata GetMetadata();
    }
}
