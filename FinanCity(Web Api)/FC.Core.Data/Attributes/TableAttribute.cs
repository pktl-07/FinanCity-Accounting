namespace FC.Core
{
    using System;
    using  FC.Core.Infrastructure;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, AllowMultiple = true)]
    public class TableAttribute : Attribute
    {
        public TableAttribute() { }

        public TableAttribute(string name)
        {
            Guard.IsNotNullOrEmpty(name, "name");

            Name = name;
        }

        public string Name { get; set; }
    }
}
