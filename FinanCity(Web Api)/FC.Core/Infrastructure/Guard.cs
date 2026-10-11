namespace FC.Core.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using FC.Core.Exception;

    public class Guard
    {
        /// <summary>
        /// Ensures the specified argument is not null.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        public static void IsNotNull(object parameter, string parameterName)
        {
            if (parameter == null)
                throw new ArgumentNullException(parameterName, Resources.TextResource.CannotBeNull.FormatWith(parameterName));
        }

        /// <summary>
        /// Ensures the specified string is not blank.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        public static void IsNotNullOrEmpty(string parameter, string parameterName)
        {
            if (string.IsNullOrEmpty((parameter ?? string.Empty)))
                throw new ArgumentException(Resources.TextResource.CannotBeNullOrEmpty.FormatWith(parameterName));
        }

        /// <summary>
        /// Ensures the specified array is not null or empty.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameter">The parameter.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        public static void IsNotNullOrEmpty<T>(T[] parameter, string parameterName)
        {
            IsNotNull(parameter, parameterName);

            if (parameter.Length == 0)
                throw new ArgumentException(Resources.TextResource.ArrayCannotBeEmpty.FormatWith(parameterName));
        }

        /// <summary>
        /// Ensures the specified collection is not null or empty.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameter">The parameter.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        public static void IsNotNullOrEmpty<T>(ICollection<T> parameter, string parameterName)
        {
            IsNotNull(parameter, parameterName);

            if (parameter.Count == 0)
                throw new ArgumentException(Resources.TextResource.CollectionCannotBeEmpty.FormatWith(parameterName), parameterName);
        }

        /// <summary>
        /// Ensures the specified value is a positive integer.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        public static void IsNotZeroOrNegative(int parameter, string parameterName)
        {
            if (parameter <= 0)
                throw new ArgumentOutOfRangeException(parameterName, Resources.TextResource.CannotBeNegativeOrZero.FormatWith(parameterName));
        }

        /// <summary>
        /// Ensures the specified value is not a negative integer.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        public static void IsNotNegative(int parameter, string parameterName)
        {
            if (parameter < 0)
                throw new ArgumentOutOfRangeException(parameterName, Resources.TextResource.CannotBeNegative.FormatWith(parameterName));
        }

        /// <summary>
        /// Ensures the specified value is not a negative integer.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        public static void IsNotNegative(decimal parameter, string parameterName)
        {
            if (parameter < 0)
                throw new ArgumentOutOfRangeException(parameterName, Resources.TextResource.CannotBeNegative.FormatWith(parameterName));
        }

        /// <summary>
        /// Ensures the specified value is not a negative float.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        public static void IsNotNegative(float parameter, string parameterName)
        {
            if (parameter < 0)
                throw new ArgumentOutOfRangeException(parameterName, Resources.TextResource.CannotBeNegative.FormatWith(parameterName));
        }

        /// <summary>
        /// Ensures the specified collection does not have duplicate key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters">The key collection</param>
        /// <param name="key">match key</param>
        /// <param name="parameterName">Name of the parameter</param>
        public static void IsNotDuplicateKey<T>(ICollection<T> parameters, T key, string parameterName)
        {
            if (parameters.Contains(key))
                throw new DupicateKeyException(parameterName, Resources.TextResource.DictionaryKeyShouldNotBeDuplicate.FormatWith(parameterName));
        }

        public static void IsNotEndWith(string key, string source, string parameterName)
        {
            if (!key.EndsWith(source))
                throw new NameConventionException(parameterName, Resources.TextResource.NotEndWith.FormatWith(parameterName));
        }

        /// <summary>
        /// Ensures the specified path is a virtual path which starts with ~/.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        public static void IsNotVirtualPath(string parameter, string parameterName)
        {
            IsNotNullOrEmpty(parameter, parameterName);

            if (!parameter.StartsWith("~/", StringComparison.Ordinal))
            {
                throw new ArgumentException(Resources.TextResource.SourceMustBeAVirtualPathWhichShouldStartsWithTileAndSlash, parameterName);
            }
        }

        public static void MustKeyContains<Key, Val>(IDictionary<Key, Val> dictionary, Key key)
        {
            if (!dictionary.ContainsKey(key))
            {
                throw new ArgumentException(Resources.TextResource.KeyNotContains, key.ToString());
            }
        }
    }
}
