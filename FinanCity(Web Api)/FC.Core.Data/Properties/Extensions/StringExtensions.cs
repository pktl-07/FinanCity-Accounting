namespace ESS.Web.Data.Extensions
{
    using System;
    using System.Linq;
    using Infrastructure;
    using ESS.Web.Exception;
    using ESS.Web.Infrastructure;

    internal static class StringExtensions
    {
        internal static string EntityNamingConvention(this string value)
        {
            Guard.IsNotEndWith(value, "Entity", "value");
            return value.Replace("Entity", "");
        }
        
        internal static string GetFullSQLFieldName(this string column,object baseType)
        {
            Guard.IsNotNullOrEmpty(column, "column");

            //every entity assert is derived from IEntity base class. the metadata are diverse.
            //entity class metadata is achive via Db attribute and query assert metadata is comming from dictionary.
            //below is the codition to check to pick up correct column name from different comming source.
            if (baseType is IQueryAssert)
            {
                var metaData = (baseType as IQueryAssert).GetMetadata();
                if (metaData.ContainsKey(column))
                    if (!string.IsNullOrEmpty(metaData[column].TableName) && !string.IsNullOrEmpty(metaData[column].ColumnName))
                        return $"[{ metaData[column].TableName }].[{metaData[column].ColumnName}]";

                    else
                        return $"{ column }";

            }
            else if (baseType is IEntity)
            {
                var columnName = baseType.GetType().GetProperties().Where(x => x.Name.Equals(column, StringComparison.OrdinalIgnoreCase)).First().GetColumnAttribute().Name ?? column;
                return $"[{ (baseType as IEntity).GetTableName()}].[{ columnName }]";
            }
            throw new NotFoundException("column", "This column does not exists in IEntity or IQueryAssert base.");
        }
    }
}
