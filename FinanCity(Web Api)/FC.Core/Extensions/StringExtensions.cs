namespace FC.Core
{
    using System;
    using System.Text.RegularExpressions;

    public static class StringExtensions
    {
        public static string AddChar(this string value, char ch)
        {
            return value.Trim() + ch.ToString();
        }

        public static string RemoveLastChar(this string str, char ch)
        {
            return str.TrimStart().TrimEnd(ch);
        }

        public static string AddComma(this string value)
        {
            return AddChar(value, ',');
        }

        /// <summary>
        /// Determines whether this instance and another specified System.String object have the same value.
        /// </summary>
        /// <param name="instance">The string to check equality.</param>
        /// <param name="comparing">The comparing with string.</param>
        /// <returns>
        /// <c>true</c> if the value of the comparing parameter is the same as this string; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsCaseInsensitiveEqual(this string instance, string comparing)
        {
            return string.Compare(instance, comparing, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public static string RemoveAllWhiteSpace(this string value)
        {
            return Regex.Replace(value, @"\s+", "");
        }

        public static string Appand(this string value, string appendText)
        {
            return $"{value} {appendText}";
        }
    }
}
