namespace FC.Core.Data.Infrastructure
{
    /// <summary>
    /// Record creation date log. This interface can remove unless necessary.
    /// </summary>
    public interface ICreatedDate
    {
        int? CreatedDate { get; }
    }
}
