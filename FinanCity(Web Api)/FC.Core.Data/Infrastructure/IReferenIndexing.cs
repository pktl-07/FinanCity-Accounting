namespace FC.Core.Data.Infrastructure
{
    public interface IReferenIndexing
    {
        string R1 { get; set; }

        string Prefix { get; set; }

        string Postfix { get; set; }

        int VoucherId { get; set; }
    }
}
