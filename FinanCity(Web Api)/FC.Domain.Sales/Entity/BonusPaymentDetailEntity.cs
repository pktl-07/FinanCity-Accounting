namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;

    [Table(Name = "AMS109")]
    public class BonusPaymentDetailEntity : EntityBase
    {
        public BonusPaymentDetailEntity() : base("FC.Biz") { }

        [Column(Name = "BonusPayableSK")]
        public string BonusPayableSK { get; set; }

        [Column(Name = "BonusPaymentSK")]
        public string BonusPaymentSK { get; set; }
    }
}
