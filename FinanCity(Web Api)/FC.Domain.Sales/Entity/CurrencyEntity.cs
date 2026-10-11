namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;
    using System.ComponentModel.DataAnnotations;

    [Table(Name = "Currency")]
    public class CurrencyEntity : EntityBase
    {
        public CurrencyEntity() : base("FC.Biz") { }

        [Column(Name = "Country")]
        public string Country { get; set; }

        [Column(Name = "Currency_Name")]
        [Display(Name = "Currency Name")]
        public string Currency_Name { get; set; }

        [Column(Name = "Currency_AlphaCode")]
        [Display(Name = "Currency Code")]
        public string Currency_AlphaCode { get; set; }

        [Column(Name = "Currency_NumberCode")]
        public string Currency_NumberCode { get; set; }

        [Column(Name = "Currency_Symbol")]
        public string Currency_Symbol { get; set; }

        [Column(Name = "Currency_Minorunit")]
        public int Currency_Minorunit { get; set; }
    }
}
