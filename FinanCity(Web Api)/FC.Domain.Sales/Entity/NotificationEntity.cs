namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;

    [Table(Name = "JUN013")]
    public partial class NotificationEntity : EntityBase
    {
        public NotificationEntity() : base("FC.Biz") { }

        [Column(Name = "SenderSK")]
        public string SenderSK { get; set; }

        [Column(Name = "ReceiverSK")]
        public string ReceiverSK { get; set; }

        [Column(Name = "RefSK")]
        public string RefSK { get; set; }

        [Column(Name = "IsRead")]
        public bool IsRead { get; set; }

        [Column(Name = "TransType")]
        public int TransType { get; set; }

        [Column(Name = "Description")]
        public string Description { get; set; }
    }
}
