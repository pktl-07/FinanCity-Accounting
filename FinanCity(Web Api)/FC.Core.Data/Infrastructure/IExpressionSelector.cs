namespace FC.Core.Data.Infrastructure
{
    using System.Collections.Generic;

    /// <summary>
    /// Select or conjunction checker interface.
    /// </summary>
    public interface IExpressionSelector
    {
        ICollection<string> FieldCollection { get; set; }
    }
}
