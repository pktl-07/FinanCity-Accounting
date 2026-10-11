namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;
    using System.ComponentModel.DataAnnotations;

    [Table(Name = "AMS103"), RecordIndexer(AutoGenerateIdReferencesOnPost = true)]
    public class CommissionReceivedEntity : EntityBase, IMultiCurrency
    {
        public CommissionReceivedEntity() : base("FC.Biz") { }

        [Column(Name = "InvoiceSK")]
        public string InvoiceSK { get; set; }

        [Column(Name = "ReceiptDate", IsDate = true), Display(Name = "Receipt Date")]
        public int? ReceiptDate { get; set; }

        [Column(Name = "PaymentType")]
        public int PaymentType { get; set; }

        [Column(Name = "CBSK"), Display(Name = "Cashbook")]
        public string CBSK { get; set; }

        [Column(Name = "AmountBeforeGST"), MultiCurrency, Display(Name = "Amount Before GST")]
        public decimal AmountBeforeGST { get; set; }

        [Column(Name = "ChequeNo")]
        public string ChequeNo { get; set; }

        [Column(Name = "GSTPercent"), Display(Name = "GST Percent")]
        public decimal GSTPercent { get; set; }

        [Column(Name = "GSTValue"), MultiCurrency]
        public decimal GSTValue { get; set; }

        [Column(Name = "TotalAmt"), MultiCurrency, Display(Name = "Total Amount")]
        public decimal TotalAmt { get; set; }

        [Column(Name = "Remarks")]
        public string Remarks { get; set; }

        [Column(Name = "CompletionDate", IsDate = true), Display(Name = "Completion Date")]
        public string CompletionDate { get; set; }

        [Column(Name = "PaymentMode")]
        public int? PaymentMode { get; set; }

        [Column(Name = "BankName")]
        public string BankName { get; set; }

        [Column(Name = "CURRSK")]
        public string CURRSK { get; set; }

        [Column(Name = "CURRATE")]
        [Display(Name = "Currency Rate")]
        public decimal CURRATE { get; set; }

        [Column(Name = "CURROP")]
        public string CURROP { get; set; }

        [Column(Name = "TransCURRSK")]
        public string TransCURRSK { get; set; }

        [Column(Name = "TransCURRATE")]
        [Display(Name = "Currency Rate")]
        public decimal TransCURRATE { get; set; }

        [Column(Name = "TransCURROP")]
        public string TransCURROP { get; set; }

        [Column(Name = "JRef")]
        public string JRef { get; set; }
    }
}
