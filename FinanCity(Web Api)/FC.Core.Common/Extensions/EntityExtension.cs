namespace FC.Core
{
    using System;
    using System.Linq;
    using System.Reflection;
    using FC.Core.Data;

    public static class EntityExtension
    {
        public static RecordIndexerAttribute GetIndexerAttributes(this IEntity entity)
        {
            return (RecordIndexerAttribute)entity.GetType().GetCustomAttribute(typeof(RecordIndexerAttribute), false);
        }
        
        public static int GetTableType(this IEntity entity)
        {
            foreach (var tableField in entity.GetType().GetProperties())
            {
                var dbColumn = (ColumnAttribute)tableField.GetCustomAttribute(typeof(ColumnAttribute), false);
                if (dbColumn != null)
                {
                    var columnName = dbColumn.Name ?? tableField.Name;
                    if (columnName == "TransType")
                    {
                        return (int)entity.GetType().InvokeMember(columnName, BindingFlags.GetProperty, null, entity, null);
                    }
                }
            }

            var tableTransType = Enum.GetValues(TransType.InternalSalesperson.GetType()).Cast<TransType>().Select(x => new { TransType = x, TableName = x.ToTableName() })
                 .Where(x => x.TableName == entity.GetType().GetCustomAttribute<TableAttribute>().Name);

            if (tableTransType.Count() > 0)
                return (int)tableTransType.First().TransType;

            return 0;
        }
    }
}
