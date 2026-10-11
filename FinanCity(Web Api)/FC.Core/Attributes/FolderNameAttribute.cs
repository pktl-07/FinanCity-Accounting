namespace FC.Core
{
    using System;
    using Infrastructure;

    [AttributeUsage(AttributeTargets.Field)]
    public class FolderNameAttribute : Attribute
    {
        public FolderNameAttribute() { }

        public FolderNameAttribute(string description)
        {
            Guard.IsNotNullOrEmpty(description, "name");

            Description = description;
        }

        public string Description { get; private set; }
    }
}
