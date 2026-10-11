namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;

    [Table(Name = "AMS105")]
    public class BonusPayableEntity : EntityBase
    {
        public BonusPayableEntity() : base("FC.Biz") { }

        [Column(Name = "PayableSchemeSK")]
        public string PayableSchemeSK { get; set; }

        [Column(Name = "SPSK")]
        public string SPSK { get; set; }

        [Column(Name = "Tier")]
        public int Tier { get; set; }

        [Column(Name = "Rank")]
        public int Rank { get; set; }

        [Column(Name = "Percentage")]
        public decimal Percentage { get; set; }

        [Column(Name = "TransType")]
        public int TransType { get; set; }
    }
}
