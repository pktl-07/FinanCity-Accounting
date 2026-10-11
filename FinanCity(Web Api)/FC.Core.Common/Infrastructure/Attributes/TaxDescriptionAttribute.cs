namespace FC.Core.Infrastructure.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, AllowMultiple = true)]
    public sealed class TaxDescriptionAttribute : Attribute
    {
        public string Name
        {
            get;
            set;
        }
    }
}
