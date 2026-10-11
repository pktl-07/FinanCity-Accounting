namespace FC.Core
{
    using System;
    using System.Globalization;
    using System.Text;

    public static class StringBuilderExtensions
    {
        /// <summary>
        /// Replaces the format item in a specified System.String with the text equivalent of the value of a corresponding System.Object instance in a specified array.
        /// </summary>
        /// <param name="instance">A string to format.</param>
        /// <param name="args">An System.Object array containing zero or more objects to format.</param>
        /// <returns>A copy of format in which the format items have been replaced by the System.String equivalent of the corresponding instances of System.Object in args.</returns>
        public static string FormatWith(this string instance, params object[] args)
        {
            return string.Format(CultureInfo.CurrentCulture, instance, args);
        }

        public static StringBuilder RemoveLastChar(this StringBuilder sb, char ch)
        {
            return new StringBuilder(sb.ToString().Trim().TrimEnd(','));
        }

         public static bool HasValue(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        public static T ToEnum<T>(this string value, T defaultValue)
        {
            if (!value.HasValue())
            {
                return defaultValue;
            }
            try
            {
                return (T)Enum.Parse(typeof(T), value, true);
            }
            catch (ArgumentException)
            {
                return defaultValue;
            }
        }
    }
}
