namespace FC.Core
{
    using System;
    using System.Linq;

    public static class EnumExtenstions
    {
        public static string ToDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());

            var attribute = Attribute.GetCustomAttribute(field, typeof(EnumDescriptionAttribute)) as EnumDescriptionAttribute;

            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static EnumInfo[] ToEnumCollection<T>(this Enum value) where T : struct
        {
            Func<T, string> convert = (x) =>
            {
                var field = x.GetType().GetField(x.ToString());

                var attribute = Attribute.GetCustomAttribute(field, typeof(EnumDescriptionAttribute)) as EnumDescriptionAttribute;

                return attribute == null ? x.ToString() : attribute.Description;
            };

            return Enum.GetValues(typeof(T)).Cast<T>().Select(x => new EnumInfo
            {
                Key = (int)((dynamic)x),
                Value = convert(x)
            }).ToArray();
        }
    }

    public class EnumInfo
    {
        public int Key { get; set; }

        public string Value { get; set; }
    }
}
