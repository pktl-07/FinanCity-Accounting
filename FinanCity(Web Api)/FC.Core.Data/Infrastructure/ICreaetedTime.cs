namespace FC.Core.Data.Infrastructure
{
    /// <summary>
    /// Record create time log. This interface can remove unless necessary.
    /// </summary>
    public interface ICreatedTime
    {
        string CreatedTime { get; }
    }
}
