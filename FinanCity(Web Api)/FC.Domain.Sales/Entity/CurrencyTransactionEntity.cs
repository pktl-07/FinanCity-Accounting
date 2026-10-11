using FC.Core;
namespace FC.Domain.Sales.Entity
{
    using FC.Core.Data;

    [Table(Name = "JUN009")]
    public class CurrencyTransactionEntity : EntityBase
    {
        public CurrencyTransactionEntity() : base("FC.Biz") { }

        [Column(Name = "ParentSK")]
        public string ParentSK { get; set; }

        [Column(Name = "CURRSK")]
        public string CURRSK { get; set; }

        [Column(Name = "CURRATE")]
        public decimal CURRATE { get; set; }

        [Column(Name = "CURROP")]
        public string CURROP { get; set; }
    }
}
