namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;

    [Table(Name = "JUN011")]
    public partial class MarketingExpenseEntity : EntityBase
    {
        public MarketingExpenseEntity() : base("FC.Biz") { }

        [Column(Name = "PTOSK")]
        public string PTOSK { get; set; }

        [Column(Name = "ItemSK")]
        public string ItemSK { get; set; }

        [Column(Name = "Amount")]
        public decimal Amount { get; set; }
    }
}
