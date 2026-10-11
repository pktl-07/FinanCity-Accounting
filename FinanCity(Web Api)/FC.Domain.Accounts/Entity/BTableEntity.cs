namespace FC.Domain.Accounts.Entity
{
    using FC.Core;
    using FC.Core.Data;
  
    public class BTableEntity : EntityBase 
        , IMultiCurrency
    {
        public BTableEntity() : base(CommonConstant.AccountConnection) { }

        [Column(Name = "SourceHeaderTransSK")]
        public string SourceHeaderTransSK { get; set; }

        [Column(Name = "TaxAmount")]
        [MultiCurrency]
        public decimal TaxAmount { get; set; }

        [Column(Name = "IsTaxInclusive")]
        public bool IsTaxInclusive { get; set; }

        [Column(Name = "TaxPercent")]
        public decimal TaxPercent { get; set; }

        [Column(Name = "TaxSK")]
        public string TaxSK { get; set; }

        [Column(Name = "TransactionAmount")]
        [MultiCurrency]
        public decimal TransactionAmount { get; set; }

        [Column(Name = "AccSK")]
        public string AccSK { get; set; }

        [Column(Name = "BatchNo")]
        public string BatchNo { get; set; }

        [Column(Name = "YCPC")]
        public string YCPC { get; set; }

        [Column(Name = "SecondRef")]
        public string SecondRef { get; set; }

        [Column(Name = "ParentId")]
        public string ParentId { get; set; }

        [Column(Name = "EntryDescription")]
        public string EntryDescription { get; set; }

        [Column(Name = "LineDescription")]
        public string LineDescription { get; set; }

        [Column(Name = "PostDate", IsDate = true)]
        public int? PostDate { get; set; }
        
        [Column(Name = "ReversalRefNo")]
        public string ReversalRefNo { get; set; }

        [Column(Name = "TransType")]
        public int TransType { get; set; }

        [Column(Name = "IsAutoOpening")]
        public bool IsAutoOpening { get; set; }

        [Column(Name = "ModuleCode")]
        public string ModuleCode { get; set; }

        [Column(Name = "JobSK")]
        public string JobSK { get; set; }

        [Column(Name = "CURRSK")]
        public string CURRSK { get; set; }

        [Column(Name = "CURRATE")]
        public decimal CURRATE { get; set; }

        [Column(Name = "CURROP")]
        public string CURROP { get; set; }

        [Column(Name = "DepartmentSK")]
        public string DepartmentSK { get; set; }

        [Column(Name = "HasTaxLine")]

        public bool HasTaxLine { get; set; }

        [Column(Name = "JobTransactionAmount")]
        public decimal JobTransactionAmount { get; set; }

        [Column(Name = "TransTypeDescription")]
        public string TransTypeDescription
        {
            get;
            set;
        }
    }
}
