namespace FC.Core.Infrastructure.Implements.RecordIndexer
{
    using FC.Core;

    public sealed class RecordIndexrContext
    {
        public int VoucherId { get; set; } = CommonConstant.DefaultIndexing.VoucherId;

        public string Postfix { get; set; } = CommonConstant.DefaultIndexing.Postfix;

        public string Prefix { get; set; } = CommonConstant.DefaultIndexing.Prefix;

        public string R1 { get; set; } = CommonConstant.DefaultIndexing.R1;

        public int Format { get; set; }
    }
}
