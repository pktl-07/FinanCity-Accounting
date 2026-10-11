namespace FC.Domain.Sales.Entity
{
    using FC.Core;
    using FC.Core.Data;
    using System.ComponentModel.DataAnnotations;

    [Table(Name = "AMS108")]
    public class BonusPaymentEntity : EntityBase
    {
        public BonusPaymentEntity() : base("FC.Biz") { }

        [Column(Name = "PaymentDate", IsDate = true), Display(Name = "Payment Date")]
        public int? PaymentDate { get; set; }

        [Column(Name = "CBSK")]
        public string CBSK { get; set; }

        [Column(Name = "FromDate", IsDate = true), Display(Name = "From Date")]
        public int? FromDate { get; set; }

        [Column(Name = "ToDate", IsDate = true), Display(Name = "To Date")]
        public int? ToDate { get; set; }

        [Column(Name = "CBType"), Display(Name = "Cashbook Type")]
        public int CBType { get; set; }

        [Column(Name = "ChequeNo"), Display(Name = "Cheque No.")]
        public string ChequeNo { get; set; }

        [Column(Name = "TotalPayment"), MultiCurrency]
        public decimal TotalPayment { get; set; }

        [Column(Name = "JRef")]
        public string JRef
        {
            get;
            set;
        }

        [Column(Name = "TransType")]
        public int TransType { get; set; }
    }
}
