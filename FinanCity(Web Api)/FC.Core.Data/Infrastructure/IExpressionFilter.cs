namespace FC.Core.Data.Infrastructure
{
    using System.Text;

    /// <summary>
    /// Where conjunction cheker interface.
    /// </summary>
    public interface IExpressionFilter
    {
        StringBuilder WHERE { get; set; }
    }
}
