namespace FC.Domain.Accounts.Entity
{
    using FC.Core;
    using FC.Core.Data;
    using System.ComponentModel.DataAnnotations;

    [Table(Name = "CBM002")]
    public class CashBookTransactionEntity : EntityBase 
        , IMultiCurrency
    {
        public CashBookTransactionEntity() : base(CommonConstant.AccountConnection) { }

        [Column(Name = "ManualRef")]
        public string ManualRef { get; set; }

        [Column(Name = "SecondRef")]
        public string SecondRef { get; set; }

        [Column(Name = "JRef")]
        public string JRef { get; set; }

        [Column(Name = "TransactionDate", IsDate = true)]
        public int? TransactionDate { get; set; }

        [Column(Name = "PaidPerson")]
        public string PaidPerson { get; set; }

        [Column(Name = "ChequeNo")]
        public string ChequeNo { get; set; }

        [Column(Name = "BankInfo")]
        public string BankInfo { get; set; }

        [Column(Name = "Remark")]
        public string Remark { get; set; }

        [Column(Name = "CBSK")]
        public string CBSK { get; set; }

        [Column(Name = "RefTransactionType")]
        public int RefTransactionType { get; set; }

        [Column(Name = "ParentId")]
        public string ParentId { get; set; }

        [Column(Name = "SourceHeaderTransSK")]
        public string SourceHeaderTransSK { get; set; }

        [Column(Name = "Inclusive")]
        public bool Inclusive { get; set; }

        [Column(Name = "AccSK")]
        public string AccSK { get; set; }

        [Column(Name = "AccCode")]
        public string AccCode { get; set; }

        [Column(Name = "TaxSK")]
        public string TaxSK { get; set; }

        [Column(Name = "TaxPercent")]
        public decimal TaxPercent { get; set; }

        [Column(Name = "TaxAmount")]
        [MultiCurrency]
        public decimal TaxAmount { get; set; }

        [Column(Name = "TransactionAmount")]
        [MultiCurrency]
        public decimal TransactionAmount { get; set; }

        [Column(Name = "TotalAmount")]
        [MultiCurrency]
        public decimal TotalAmount { get; set; }

        [Column(Name = "TransType")]
        public int TransType { get; set; }

        [Column(Name = "JobSK")]
        public string JobSK { get; set; }

        [Column(Name = "Memo")]
        public string Memo { get; set; }

        [Column(Name = "Description")]
        public string Description { get; set; }

        [Column(Name = "CURRSK")]
        public string CURRSK { get; set; }

        [Column(Name = "CURRATE"), Display(Name = "Currency")]
        public decimal CURRATE { get; set; }

        [Column(Name = "CURROP")]
        public string CURROP { get; set; }

        [Column(Name = "DepartmentSK")]
        public string DepartmentSK { get; set; }
    }
}
