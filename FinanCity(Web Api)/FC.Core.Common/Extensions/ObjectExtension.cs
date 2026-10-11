namespace FC.Core
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public static class ObjectExtension
    {
        internal static object ToReplaceDbNull(this object value)
        {
            if (value == DBNull.Value)
                return null;

            return value;
        }

        public static T CloneJson<T>(this T source)
        {
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);
        }

        public static IDictionary<string, string> ToJournalPreventAccountPostingDictionary(this DefaultAccountingCode source)
        {
            var result = new Dictionary<string, string>
            {
                [source.SalespersonAccurals.Key] = source.SalespersonAccurals.Value,
                [source.CobrokerAccurals.Key] = source.CobrokerAccurals.Value,
                [source.CommisionExpense.Key] = source.CommisionExpense.Value,
                [source.CommisionReceivable.Key] = source.CommisionReceivable.Value,
                [source.CompanyCapital.Key] = source.CompanyCapital.Value,
                [source.MarketingPromotionPayable.Key] = source.MarketingPromotionPayable.Value,
                [source.MarketingPromotionExpense.Key] = source.MarketingPromotionExpense.Value,
            };

           return result;
        }
    }
}
