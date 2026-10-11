namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;
    using System.ComponentModel.DataAnnotations;

    [Table(Name = "AMS111"), RecordIndexer(AutoGenerateIdReferencesOnSave = true)]
    public class SalesInvoiceLogEntity : EntityBase
    {
        public SalesInvoiceLogEntity() : base("FC.Biz") { }

        [Column(Name = "InvoiceDate", IsDate = true), Display(Name = "Invoice Date")]
        public int? InvoiceDate { get; set; }

        [Column(Name = "Remark")]
        public string Remark { get; set; }

        [Column(Name = "MailingType")]
        public int MailingType { get; set; }

        [Column(Name = "SecondRef")]
        public string SecondRef { get; set; }

        [Column(Name = "CompletionDate", IsDate = true), Display(Name = "Completion Date")]
        public int? CompletionDate { get; set; }

        [Column(Name = "ReceivableStatus")]
        public int ReceivableStatus { get; set; } = (int)SalesViewType.Pending;

        [Column(Name = "PayableStatus")]
        public int PayableStatus { get; set; } = (int)SalesViewType.Pending;

        [Column(Name = "ActualAmt"), MultiCurrency]
        public decimal ActualAmt { get; set; }

        [Column(Name = "ActualGST")]
        public decimal ActualGST { get; set; }

        [Column(Name = "TAXSK")]
        public string TAXSK { get; set; }

        [Column(Name = "PaymentTermSK"), Display(Name = "Payment Term")]
        public string PaymentTermSK { get; set; }

        [Column(Name = "CURRSK")]
        public string CURRSK { get; set; }

        [Column(Name = "CURRATE")]
        public decimal CURRATE { get; set; }

        [Column(Name = "CURROP")]
        public string CURROP { get; set; }

        [Column(Name = "JRef")]
        public string JRef { get; set; }

        [Column(Name = "PTOSK")]
        public string PTOSK { get; set; }
    }
}
