namespace FC.Domain.Version.Infrastructure
{
    using System;

    public class ESSVersionAttribute : Attribute
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
