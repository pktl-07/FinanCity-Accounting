namespace FC.Core
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public static class EnumerableExtensions
    {
        public static void Each<T>(this IEnumerable<T> instance, Action<T, int> action)
        {
            int index = 0;
            foreach (T item in instance)
                action(item, index++);
        }

        public static void Each<T>(this IEnumerable<T> instance, Action<T> action)
        {
            if (instance != null)
            {
                foreach (T item in instance)
                    action(item);
            }
        }

        internal static IEnumerable<T> BatchItemsWithoutKey<T>(this IEnumerable<T> source, Expression<Func<T, object>> configurator)
        {
            return source.Where(item =>
            {
                foreach (var key in GetKeyFields(configurator))
                {
                    if (item.GetType().GetProperty(key).GetValue(item) == null)
                        return true;
                }
                return false;
            }).ToList();
        }

        internal static IEnumerable<T> BatchItemsWithKey<T>(this IEnumerable<T> source, Expression<Func<T, object>> configurator)
        {
            return source.Where(item =>
            {
                foreach (var key in GetKeyFields(configurator))
                {
                    if (item.GetType().GetProperty(key).GetValue(item) == null)
                        return false;
                }
                return true;
            }).ToList();
        }

        internal static T BatchItemsWithSingle<T>(this IEnumerable<T> value, T matchValue, Expression<Func<T, object>> configurator)
        {
            var keys = GetKeyFields(configurator);

            var result = value.Where((item) =>
            {
                foreach (var key in GetKeyFields(configurator))
                {
                    var i = (string)item.GetType().GetProperty(key).GetValue(item);
                    var m = (string)matchValue.GetType().GetProperty(key).GetValue(matchValue);
                    if (m != i)
                        return false;
                }
                return true;
            }).SingleOrDefault();

            return result;
        }

        internal static IEnumerable<T> BatchItemsNotMatch<T>(this IEnumerable<T> source, IEnumerable<T> match, Expression<Func<T, object>> configurator)
        {
            return source.Where(item =>
            {
                var singleMatch = match.BatchItemsWithSingle(item, configurator);

                if (singleMatch == null)
                    return false;

                foreach (var prop in singleMatch.GetType().GetProperties())
                {
                    if (!prop.GetType().IsArray)
                    {
                        var m = prop.GetValue(singleMatch);
                        var i = item.GetType().GetProperty(prop.Name).GetValue(item);

                        if (prop.Name == "Auto" || prop.Name == "CreatedDate" || prop.Name == "CreatedTime" ||
                        prop.Name == "ModifiedDate" || prop.Name == "ModifiedTime" || prop.Name == "RecordStatus" ||
                        prop.Name == "ModifiedUser" || prop.Name == "LoginUser" || prop.Name == "CreatedUser")
                            continue;

                        if (prop.PropertyType == typeof(string))
                        {
                            if ((string)m != (string)i)
                                return true;
                        }
                        else if (prop.PropertyType == typeof(bool))
                        {
                            if ((bool)m != (bool)i)
                                return true;
                        }
                        else if (prop.PropertyType == typeof(bool))
                        {
                            if ((float)m != (float)i)
                                return true;
                        }
                        else if (prop.PropertyType == typeof(decimal))
                        {
                            if ((decimal)m != (decimal)i)
                                return true;
                        }
                        else if (prop.PropertyType == typeof(int))
                        {
                            if ((int)m != (int)i)
                                return true;
                        }
                    }
                }
                return false;
            }).ToList();
        }

        private static string[] GetKeyFields<T>(Expression<Func<T, object>> expression)
        {
            var members = new List<string>();

            if (expression.Body is MemberExpression)
            {
                var member = (expression.Body as MemberExpression).Member;
                members.Add(member.Name);
            }
            if (expression.Body is NewExpression)
            {
                foreach (var member in (expression.Body as NewExpression).Members)
                {
                    members.Add(member.Name);
                }
            }
            return members.ToArray();
        }
    }
}
