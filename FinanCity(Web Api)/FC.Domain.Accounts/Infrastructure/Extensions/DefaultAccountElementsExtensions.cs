namespace FC.Core
{
    using System.Collections.Generic;

    public static class DefaultAccountElementsExtensions
    {
        public static IDictionary<string, string> GetAllElements(this DefaultAccountingCode value)
        {
            var result = new Dictionary<string, string>();
            var valueType = value;

            foreach (var prop in valueType.GetType().GetProperties())
            {
                var p = prop.GetValue(valueType);
                if (p is KeyValuePair<string, string>)
                {
                    var k = (KeyValuePair<string, string>)p;
                    result.Add(k.Key, k.Value);
                }
            }

            return result;
        }
    }
}
