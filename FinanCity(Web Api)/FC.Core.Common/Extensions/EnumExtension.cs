namespace FC.Core
{
    using System;

    public static class EnumExtenstions
    {
        public static string ToFolderName(this FileType value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = Attribute.GetCustomAttribute(field, typeof(FolderNameAttribute)) as FolderNameAttribute;

            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}
