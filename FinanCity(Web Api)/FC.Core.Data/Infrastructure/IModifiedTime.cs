namespace FC.Core.Data.Infrastructure
{
    /// <summary>
    /// Record modified time log. This interface can remove unless necessary.
    /// </summary>
    public interface IModifiedTime
    {
        string ModifiedTime { get; }
    }
}
