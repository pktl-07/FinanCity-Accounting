namespace FC.Domain.Sales.Entity
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using FC.Core;
    using FC.Core.Data;

    [Table(Name = "AMS106"), Description("Commission Payment"), RecordIndexer(AutoGenerateIdReferencesOnPost = true)]
    public class CommissionPaymentEntity : EntityBase
        , IMultiCurrency
    {
        public CommissionPaymentEntity() : base("FC.Biz") { }

        [Column(Name = "ALOSK")]
        public string ALOSK
        {
            get;
            set;
        }

        [Column(Name = "CBSK"), Display(Name = "Cashbook")]
        public string CBSK
        {
            get;
            set;
        }

        [Column(Name = "DatePaid", IsDate = true), Display(Name = "Payment Date")]
        public int? DatePaid
        {
            get;
            set;
        }

        [Column(Name = "CBInvoiceRef")]
        public string CBInvoiceRef
        {
            get;
            set;
        }

        [Column(Name = "PaymentType")]
        public int PaymentType
        {
            get;
            set;
        }

        [Column(Name = "ChequeNo")]
        public string ChequeNo
        {
            get;
            set;
        }

        [Column(Name = "PaymentDescriptionSysKey")]
        public string PaymentDescriptionSysKey
        {
            get;
            set;
        }

        [Column(Name = "NetPaid"), Display(Name = "Total Amt."), MultiCurrency]
        public decimal NetPaid
        {
            get;
            set;
        }

        [Column(Name = "TransType")]
        public int TransType
        {
            get;
            set;
        }

        [Column(Name = "JRef")]
        public string JRef
        {
            get;
            set;
        }

        [Column(Name = "MarketingExpenseAllocate")]
        public decimal MarketingExpenseAllocate { get; set; }

        [Column(Name = "CURRSK")]
        public string CURRSK
        {
            get;
            set;
        }

        [Column(Name = "CURRATE")]
        public decimal CURRATE
        {
            get;
            set;
        }

        [Column(Name = "CURROP")]
        public string CURROP
        {
            get;
            set;
        }
    }
}
