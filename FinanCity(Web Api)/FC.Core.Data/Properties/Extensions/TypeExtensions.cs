namespace ESS.Web.Data.Extensions
{
    using System;
    using System.Reflection;
    using ESS.Web.Data.Infrastructure;
    using ESS.Web.Infrastructure;

    internal static class TypeExtensions
    {
        internal static TableAttribute GetTableAttribute(this Type type)
        {
            return type.GetTypeInfo().GetCustomAttribute<TableAttribute>();
        }

        internal static ColumnAttribute GetColumnAttribute(this PropertyInfo property)
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
                    return entity.GetMetadata().TableName;

                return entity.GetType().GetTableAttribute().Name ?? $"{entity.GetType().Name.EntityNamingConvention()}";
            }

            throw new NotSupportedException();
        }

        internal static bool IsNullableType(this Type type)
        {
            return type.GetType().GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }
    }
}
