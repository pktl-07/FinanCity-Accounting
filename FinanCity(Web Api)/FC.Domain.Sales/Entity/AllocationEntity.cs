namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    [Table(Name = "AMS107"), Description("Allocation"), RecordIndexer(AutoGenerateIdReferencesOnPost = true)]
    public class AllocationEntity : EntityBase
    {
        public AllocationEntity() : base(CommonConstant.SalesConnection) { }

        [Column(Name = "SPSK"), Display(Name = "Salesperson")]
        public string SPSK { get; set; }

        [Column(Name = "HeaderSK")]
        public string HeaderSK { get; set; }

        [Column(Name = "DetailSK")]
        public string DetailSK { get; set; }

        [Column(Name = "TransType")]
        public int TransType { get; set; }

        [Column(Name = "AllocationDate", IsDate = true), Display(Name = "Allocation Date")]
        public int? AllocationDate { get; set; }

        [Column(Name = "AmountBeforeGST"), Display(Name = "Amount Before GST") , MultiCurrency]
        public decimal AmountBeforeGST { get; set; }

        [Column(Name = "GSTPercent")]
        public decimal GSTPercent { get; set; }

        [Column(Name = "GSTValue"), MultiCurrency]
        public decimal GSTValue { get; set; }

        [Column(Name = "AllocateAmount"), Display(Name = "Allocate Amount.") , MultiCurrency]
        public decimal AllocateAmount { get; set; }

        [Column(Name = "Remarks")]
        public string Remarks { get; set; }

        //[Column(Name = "BatchNo")]
        //public string BatchNo { get; set; }

        [Column(Name = "MarketingExpense"), MultiCurrency]
        public decimal MarketingExpense { get; set; }

        [Column(Name = "GrossToPromote")]
        public bool GrossToPromote { get; set; }

        [Column(Name = "Description")]
        public string Description { get; set; }

        [Column(Name = "JRef")]
        public string JRef { get; set; }
    }
}
