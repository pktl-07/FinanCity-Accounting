namespace FC.Core.Data.Extensions
{
    using System;
    using System.Reflection;
    using FC.Core.Data.Infrastructure;
    using FC.Core.Infrastructure;

    public static class TypeExtensions
    {
        public static TableAttribute GetTableAttribute(this Type type)
        {
            return type.GetTypeInfo().GetCustomAttribute<TableAttribute>();
        }

        public static ColumnAttribute GetColumnAttribute(this PropertyInfo property)
        {
            Guard.IsNotNull(property, "property");

            return property.GetCustomAttribute<ColumnAttribute>();
        }

        internal static string GetTableName<T>(this T entity) where T : IEntity
        {
            if (entity is IQueryAssert)
                return null;
            if (entity is IEntity)
            {
                if (entity.GetType().GetTableAttribute() == null)
                {
                    var table = entity.GetMetadata().TableName;
                    return table;
                }

                return entity.GetType().GetTableAttribute().Name ?? $"{entity.GetType().Name.EntityNamingConvention()}";
            }

            throw new NotSupportedException();
        }

        public static bool IsNullableType(this Type type)
        {
            return type.GetType().GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }
    }
}
