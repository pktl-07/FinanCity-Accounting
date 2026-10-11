namespace FC.Core
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public class MultiCurrencyAttribute : Attribute
    {
        public MultiCurrencyAttribute()
        {
        }
    }
}
