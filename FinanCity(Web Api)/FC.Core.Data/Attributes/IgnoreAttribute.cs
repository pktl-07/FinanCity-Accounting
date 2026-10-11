namespace FC.Core
{
    using System;

    /// <summary>
    /// Ignore entity field to ignore on db submit.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method)]
    public class IgnoreAttribute : Attribute { }
}
