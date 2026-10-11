namespace FC.Core
{
    using Newtonsoft.Json;

    public class KendoGridFilterModel
    {
        [JsonProperty(PropertyName = "filters")]
        public KendoCopositeFilterModel[] Filters { get; set; }

        [JsonProperty(PropertyName = "logic")]
        public string Logic { get; set; }
    }

    public class KendoCopositeFilterModel
    {
        [JsonProperty(PropertyName = "filters")]
        public TransalatorFieldInfo[] Filters { get; set; }

        [JsonProperty(PropertyName = "logic")]
        public string Logic { get; set; }
    }

    public class TransalatorFieldInfo
    {
        [JsonProperty(PropertyName = "field")]
        public string Field { get; set; }

        [JsonProperty(PropertyName = "operator")]
        public string Operator { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }
}