namespace FC.Core
{
    using System;
    using System.Linq;
    using System.Reflection;

    public static class TypeExtensions
    {
        public static bool TryGetAttribute<T>(this PropertyInfo propertyInfo, out T customAttribute) where T : Attribute
        {
            var attributes = propertyInfo.GetCustomAttributes(typeof(T), false).FirstOrDefault();
            if (attributes == null)
            {
                customAttribute = null;
                return false;
            }
            customAttribute = (T)attributes;
            return true;
        }
    }
}
