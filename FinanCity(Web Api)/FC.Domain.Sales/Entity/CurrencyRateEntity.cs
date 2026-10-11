namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;
    using System.ComponentModel.DataAnnotations;

    [Table(Name = "CurrencyRate")]
    public class CurrencyRateEntity : EntityBase
    {
        public CurrencyRateEntity() : base("FC.Biz") { }

        [Column(Name = "Operator")]
        public string Operator { get; set; }

        [Column(Name = "Symbol")]
        [Display(Name = "Symbol")]
        public string Symbol { get; set; }

        [Column(Name = "Rate")]
        [Display(Name = "Rate")]
        public decimal Rate { get; set; }

        [Column(Name = "Description")]
        [Display(Name = "Currency Name")]
        public string Description { get; set; }

        [Column(Name = "Remark")]
        public string Remark { get; set; }
    }
}
