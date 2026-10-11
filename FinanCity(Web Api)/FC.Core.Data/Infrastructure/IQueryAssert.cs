namespace FC.Core.Data.Infrastructure
{
    using System.Collections.Generic;

    /// <summary>
    /// Base query assert interface for both criteria and datatable.
    /// </summary>
    public interface IQueryAssert : IEntity
    {
        new IDictionary<string, IColumnMetadata> GetMetadata();
    }
}
