namespace FC.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Linq;
    using FC.Core.Data.Extensions;
    using FC.Core.Infrastructure;

    public static class IEnumerableExtension
    {
        public static DataTable ToDataTable<T>(this IEnumerable<T> data, string tableName)
        {
            Guard.IsNotNullOrEmpty(tableName, "tableName");

            return data.ToList().ToDataTable(tableName);
        }

        public static DataTable ToDataTable<T>(this IList<T> data, string tableName)
        {
            Guard.IsNotNullOrEmpty(tableName,"tableName");

            var props = TypeDescriptor.GetProperties(typeof(T));
            var table = new DataTable();

            table.TableName = tableName;
            for (int i = 0; i < props.Count; i++)
            {
                var prop = props[i];
                var targetType = prop.PropertyType.IsNullableType() ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType;

                table.Columns.Add(prop.Name, targetType);
            }

            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                    values[i] = props[i].GetValue(item);

                table.Rows.Add(values);
            }
            return table;
        }
    }
}
