namespace FC.Core.Data.Infrastructure.Implementation
{
    /// <summary>
    /// Annotatoin db parameter.  
    /// </summary>
    internal class DbParameterAnnotation
    {
        private const char symbol = '^';
        private const char seperator = '.';

        /// <summary>
        /// Convert to <##> symbol as parameter and ^ symbol as non parameter annotation. 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static string ConverTo(object parameter)
        {
            // Guard.IsNotNull(parameter, "parameters");
            if (parameter == null)
            {
                return $"<# {GeneralUtility.NullConstant} #>";
            }
            else if (parameter.ToString() == "NULL")
            {
                return parameter.ToString();
            }
            else if (parameter.ToString().StartsWith(symbol.ToString()))
            {
                return parameter.ToString().Trim().TrimStart(symbol);
            }
            else
            {
                return $"<# { parameter.ToString().Trim().RemoveLastChar(seperator)} #>";
            }
        }
    }
}
