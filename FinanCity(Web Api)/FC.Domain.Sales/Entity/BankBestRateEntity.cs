namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;
 
    [Table(Name = "BankBestRate")]
    public class BankBestRateEntity : EntityBase
    {
        public BankBestRateEntity() : base("FC.Biz") { }

        [Column]
        public decimal Rate { get; set; }

        [Column]
        public string Description { get; set; }

        [Column]
        public int? EffectiveDate { get; set; }

        [Column]
        public bool IsActive { get; set; }
    }
}
