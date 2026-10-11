namespace FC.Core.Data.Infrastructure
{
    /// <summary>
    /// Record modified date log. This interface can remove unless necessary.
    /// </summary>
    public interface IModifiedDate
    {
        int? ModifiedDate { get; }
    }
}
