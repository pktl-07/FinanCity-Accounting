namespace FC.Domain.Sales.Entity
{
    using System.ComponentModel.DataAnnotations;
    using FC.Core;
    using FC.Core.Data;

    [Table(Name = "AMS100")]
    [RecordIndexer(AutoGenerateIdReferencesOnPost = true)]
    public class PropertyTransactionEntity 
        : EntityBase, IMultiCurrency
    {
        public PropertyTransactionEntity() : base("FC.Biz") { }

        [Column(Name = "SPDate", IsDate = true), Display(Name = "Submission Date")]
        public int? SPDate { get; set; }

        [Column(Name = "ReasonForReject")]
        public string ReasonForReject { get; set; }

        [Column(Name = "ParentId")]
        public string ParentId { get; set; }

        [Column(Name = "TransactedPrice"), MultiCurrency, Display(Name = "Transaction Price")]
        public decimal TransactedPrice { get; set; }

        [Column(Name = "IsCheck")]
        public bool IsCheck { get; set; }

        [Column(Name = "IsCoporateSales")]
        public bool IsCoporateSales { get; set; }

        [Column(Name = "Remarks")]
        public string Remarks { get; set; }

        [Column(Name = "ExclusiveListing")]
        public bool ExclusiveListing { get; set; }

        [Column(Name = "BillToCoBroke")]
        public bool BillToCoBroke { get; set; }

        [Column(Name = "SecondRef")]
        public string SecondRef { get; set; }

        [Column(Name = "ClientTransType"), Display(Name = "Vendor Type")]
        public int? ClientTransType { get; set; }

        [Column(Name = "MailingClientSK")]
        public string MailingClientSK { get; set; }

        [Column(Name = "MailingSPSK")]
        public string MailingSPSK { get; set; }

        [Column(Name = "NotifyStartDate", IsDate = true), Display(Name = "Notify Start Date")]
        public int? NotifyStartDate { get; set; }

        [Column(Name = "NotifyEndDate", IsDate = true), Display(Name = "Notify End Date")]
        public int? NotifyEndDate { get; set; }

        [Column(Name = "TransactionDate", IsDate = true), Display(Name = "Transaction Date")]
        public int? TransactionDate { get; set; }

        [Column(Name = "TransType")]
        public int TransType { get; set; }

        [Column(Name = "TaxAmount"),MultiCurrency]
        public decimal TaxAmount { get; set; }

        [Column(Name = "TotalAmount"), MultiCurrency]
        public decimal TotalAmount { get; set; }

        [Column(Name = "BankerSK")]
        public string BankerSK { get; set; }

        [Column(Name = "GrossCommission"), MultiCurrency]
        public decimal GrossCommission { get; set; }

        [Column(Name = "ORBatchNo")]
        public string ORBatchNo { get; set; }

        [Column(Name = "TLBatchNo")]
        public string TLBatchNo { get; set; }

        [Column(Name = "TaxSK")]
        public string TaxSK { get; set; }

        [Column(Name = "TaxPercent")]
        public decimal TaxPercent { get; set; }

        [Column(Name = "CURRSK")]
        public string CURRSK { get; set; }

        [Column(Name = "CURRATE")]
        [Display(Name = "Currency Rate")]
        public decimal CURRATE { get; set; }

        [Column(Name = "CURROP")]
        public string CURROP { get; set; }

        [Column(Name = "CommissionRate")]
        [Display(Name = "Commission Rate")]
        public decimal CommissionRate { get; set; }

        [Column]
        public string SaleOrderTypeSK { get; set; }
    }
}
