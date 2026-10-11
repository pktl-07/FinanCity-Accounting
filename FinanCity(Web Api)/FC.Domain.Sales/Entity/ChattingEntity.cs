namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;

    [Table(Name = "JUN012")]
    public class ChattingEntity : EntityBase
    {
        public ChattingEntity() : base("FC.Biz") { }

        [Column(Name = "SenderSK")]
        public string SenderSK { get; set; }

        [Column(Name = "Type")]
        public int Type { get; set; }

        [Column(Name = "Message")]
        public string Message { get; set; }

        [Column(Name = "RefSK")]
        public string RefSK { get; set; }

        [Column(Name = "Size")]
        public int Size { get; set; }
    }
}
