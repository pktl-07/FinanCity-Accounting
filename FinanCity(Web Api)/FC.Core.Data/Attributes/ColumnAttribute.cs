namespace FC.Core
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        public ColumnAttribute()
        {
        }

        public ColumnAttribute(string name)
        {
            Name = name;
        }

        public bool IsDate { get; set; }

        public string Name { get; set; }
    }
}
