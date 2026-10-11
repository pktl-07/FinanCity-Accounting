namespace FC.Core
{
    using System;
    using Infrastructure;

    [AttributeUsage(AttributeTargets.Field)]
    public class EnumDescriptionAttribute : Attribute
    {
        public EnumDescriptionAttribute() { }

        public EnumDescriptionAttribute(string description)
        {
            Guard.IsNotNullOrEmpty(description, "name");

            Description = description;
        }

        public string Description { get; private set; }
    }
}
