namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;

    [Table(Name = "TNC")]
    public class TNC_Entity : EntityBase,
        IMultiCurrency
    {
        public TNC_Entity() : base("FC.Biz") { }

        [Column,MultiCurrency]
        public decimal LoanAmount { get; set; }

        [Column]
        public int? LoanTenure { get; set; }

        [Column]
        public int? LookInPeriod { get; set; }

        [Column]
        public bool LegalSubsidy { get; set; }

        [Column]
        public bool ValuationSubsidy { get; set; }

        [Column]
        public string OtherPromotion { get; set; }

        [Column]
        public string Remark { get; set; }

        [Column]
        public string TransactionTypeSK { get; set; }

        [Column]
        public string PropertyTypeSK { get; set; }

        [Column]
        public string InterestRateTypeSK { get; set; }

        [Column]
        public string PropertyPurposeSK { get; set; }

        [Column]
        public string RateTypeSK { get; set; }

        [Column]
        public string InterestRateTypeDurationSK { get; set; }

        [Column]
        public string RateTypeTimeFrameSK { get; set; }

        [Column]
        public decimal? RateTypeMargin { get; set; }

        [Column]
        public bool? TransactionTypeCompleteStatus { get; set; }

        [Column]
        public string CURRSK { get; set; }

        [Column]
        public decimal CURRATE { get; set; }

        [Column]
        public string CURROP { get; set; }
    }
}
