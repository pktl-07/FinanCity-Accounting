namespace FC.Core
{
    using FC.Core.Infrastructure.Attributes;
    using System;
    using System.Linq;

    public static class TransTypeEnumExtension
    {
        #region table 

        public static string ToTableName(this TransType value)
        {
            string tableName = "";

            value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(TableAttribute), false).Cast<TableAttribute>().Each(att =>
            {
                tableName = att.Name;
            });

            return tableName;
        }

        public static ModuleType ToModuleType(this TransType value)
        {
            var field = value.GetType().GetField(value.ToString());

            return (Attribute.GetCustomAttribute(field, typeof(ModuleTypeAttribute), false) as ModuleTypeAttribute).ModuleType;
        }

        #endregion

        public static string ToTaxDescription(this TransType value)
        {
            var field = value.GetType().GetField(value.ToString());

            var attribute = Attribute.GetCustomAttribute(field, typeof(TaxDescriptionAttribute), false) as TaxDescriptionAttribute;

            return attribute.Name;
        }
    }
}
