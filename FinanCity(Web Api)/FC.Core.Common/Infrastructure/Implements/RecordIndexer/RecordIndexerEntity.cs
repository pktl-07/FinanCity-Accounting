namespace FC.Core.Infrastructure.Implements.RecordIndexer
{
    public class RecordIndexerEntity 
    {
        public string Prefix { get; set; }
        public string Postfix { get; set; }
        public int StartValue { get; set; }
        public int EndValue { get; set; }
        public int TransType { get; set; }
        public string Description { get; set; }
        public int Format { get; set; }
        public string BaseDate { get; set; }
    }
}
